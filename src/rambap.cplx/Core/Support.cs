using rambap.cplx.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;

// Unit testing access some internal properties : 
[assembly: InternalsVisibleTo("rambap.cplx.UnitTests")]

namespace rambap.cplx.Core;

/// <summary>
/// Support functions for <see cref="cplx.Core"/>
/// </summary>
internal static class Support
{
    public enum PropertyOrFieldType
    {
        Field,
        BackedProperty,
        UnbackedProperty,
    }

    internal record PropertyOrFieldInfo
    {
        public required string Name { get; init; }
        public required IEnumerable<ComponentDescriptionAttribute> Comments { get; init; }

        // see https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.isassembly
        public required bool IsPublicOrAssembly { get; init; }

        public required PropertyOrFieldType Type { get; init; }

        public required bool IsFromAndEnumerable { get; init; }

        public required int IndexInEnumerable { get ; init; }
    }

    /// <summary> Define <see cref="ScanObjectContentFor"/> execution behavior </summary>
    internal enum AutoContent
    {
        /// <summary> Skip encountered null values </summary>
        IgnoreNulls,
        /// <summary> Process encountered null value </summary>
        AcceptNulls,
        /// <summary> Create and assign a new() value before processing null values </summary>
        ConstructIfNulls
    }

    /// <summary>
    /// Iterate all properties and fields of type T or IEnumerableT of an object. 
    /// </summary>
    /// <typeparam name="T">Type to look for</typeparam>
    /// <param name="obj">Object looked into</param>
    /// <param name="onData">Action to call : void (value, property_or_field_info)</param>
    /// <param name="contentMode">Behavior if the value is null. <br/>
    /// <param name="acceptUnbacked">Behavior when encountering unbacked properties. If true Unbacked
    /// (eg : expression defined, sucj as var a => b ;) properties may be scanned </param>
    /// When <see cref="AutoContent.ConstructIfNulls"/>, a parameterless constructor is called </param>
    /// <param name="ignoredDerivedTypes">Derived types of T must NOT be iterated </param>
    internal static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        AutoContent contentMode = AutoContent.IgnoreNulls,
        bool acceptUnbacked = false,
        IEnumerable<Type>? ignoredDerivedTypes = null)
        where T : class
        => ScanObjectContentFor(
            obj,
            onData,
            contentMode,
            (y, s) => (T)Activator.CreateInstance(y)!,
            acceptUnbacked,
            ignoredDerivedTypes);

    /// <summary>
    /// Iterate all properties and fields of type T or IEnumerableT of an object.
    /// </summary>
    /// <typeparam name="T">Type to look for</typeparam>
    /// <param name="obj">Object looked into</param>
    /// <param name="constructor">Constructor called in case the value is null :
    /// T (property_or_field_type, property_or_field_info) </param>
    /// <param name="onData">Action to call : void (value, property_or_field_info) <br/>
    /// Called on all value, even ones just constructed</param>
    /// <param name="acceptUnbacked">Behavior when encountering unbacked properties. If true Unbacked
    /// (eg : expression defined, sucj as var a => b ;) properties may be scanned </param>
    /// <param name="ignoredDerivedTypes">Derived types of T must NOT be iterated </param>
    internal static void ScanObjectContentFor<T>(
        object obj,
        Func<Type, PropertyOrFieldInfo, T> constructor,
        Action<T, PropertyOrFieldInfo> onData,
        bool acceptUnbacked = false,
        IEnumerable<Type>? ignoredDerivedTypes = null)
        where T : class
        => ScanObjectContentFor(
            obj,
            onData,
            AutoContent.ConstructIfNulls,
            constructor,
            acceptUnbacked,
            ignoredDerivedTypes);

    private static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        AutoContent contentMode,
        Func<Type, PropertyOrFieldInfo, T> constructor,
        bool includeUnbackedProperties,
        IEnumerable<Type>? ignoredDerivedTypes)
        where T : class
    {
        var typeStack = new Stack<Type>();
        var objt = obj.GetType()!;
        while (objt != typeof(object))
        {
            typeStack.Push(objt);
            objt = objt.BaseType!;
        }

        bool constructNulls = contentMode == AutoContent.ConstructIfNulls;
        bool acceptNulls = contentMode == AutoContent.AcceptNulls;
        bool acceptEnumerables = true; // TODO / TBD : make this a parameter ?

        // Object's field and properties are evaluated, starting with the parent classes.
        foreach (var evalType in typeStack)
        {
            // Search type and parent for all fields assignable to T that does not have the CplxIgnore Attribute
            var relevantFields = GetRelevantFields<T>(evalType, ignoredDerivedTypes);
            foreach (var rf in relevantFields)
            {
                var f = rf.Item1;
                bool isPublicOrAssembly = f.IsPublic || f.IsAssembly;
                var rename = f.GetCustomAttribute<RenameAttribute>()?.Name;
                var isEnumerable = rf.Item2.IsEnumerable;
                PropertyOrFieldInfo info = new()
                {
                    Name = rename ?? f.Name,
                    Comments = f.GetCustomAttributes<ComponentDescriptionAttribute>(),
                    IsPublicOrAssembly = isPublicOrAssembly,
                    Type = PropertyOrFieldType.Field,
                    IsFromAndEnumerable = isEnumerable,
                    IndexInEnumerable = 0,
                };
                if (isEnumerable)
                {
                    int idx = 0;
                    var val = f.GetValue(obj) as IEnumerable<T>;
                    if (val != null)
                        foreach(var i in val)
                            onData(i!, info with { IndexInEnumerable = idx ++ });

                } else
                {
                    var val = f.GetValue(obj) as T;
                    if (val is null && constructNulls)
                    {
                        // Construct non enumerable properties
                        val = constructor(f.FieldType, info);
                        f.SetValue(obj, val);
                    }
                    if (val != null || acceptNulls)
                        onData(val!, info); // TBD : Throw on null, even if accept nulls ?
                }
            }

            // Search type and parent for all properties assignable to T that does not have the CplxIgnore Attribute
            var relevantProperties = GetRelevantProperties<T>(evalType, ignoredDerivedTypes);
            if (!includeUnbackedProperties)
            {
                // Avoid matching expression bodied properties, such as "Part Name => Other ;"/
                // Expression bodied properties have a get accessor, no set, and cannot be initialised
                relevantProperties = relevantProperties.Where(v => v.Item2.IsBacked);
            }

            foreach (var rp in relevantProperties)
            {
                var p = rp.Item1;
                var isBacked = rp.Item2.IsBacked;
                bool isPublicOrAssembly = p.GetMethod!.IsPublic || p.GetMethod!.IsAssembly;
                var rename = p.GetCustomAttribute<RenameAttribute>()?.Name;
                var isEnumerable = rp.Item2.IsEnumerable;
                PropertyOrFieldInfo info = new()
                {
                    Name = rename ?? p.Name,
                    Comments = p.GetCustomAttributes<ComponentDescriptionAttribute>(),
                    IsPublicOrAssembly = isPublicOrAssembly,
                    Type = isBacked ? PropertyOrFieldType.BackedProperty : PropertyOrFieldType.UnbackedProperty,
                    IsFromAndEnumerable = isEnumerable,
                    IndexInEnumerable = 0,
                };
                if (isEnumerable)
                {
                    int idx = 0;
                    var val = p.GetValue(obj) as IEnumerable<T>;
                    if (val != null) 
                        foreach (var i in val)
                            onData(i!, info with { IndexInEnumerable = idx++ });
                }
                else
                {
                    var val = p.GetValue(obj) as T;
                    if (val is null && constructNulls)
                    {
                        val = constructor(p.PropertyType, info);
                        p.SetValue(obj, val); // Will throw if no set accessor ({get;} only, or unbacked)
                    }
                    if (val != null || acceptNulls)
                        onData(val!, info);
                }
            }
        }
    }

    /// <summary>
    /// CPLX search for private and public properties/fields of the class
    /// Parent classes are iterated manualy to control the property/field ordering
    /// </summary>
    const BindingFlags SearchFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    record CplxRelevantFieldInfo(bool IsEnumerable);
    private static IEnumerable<(FieldInfo, CplxRelevantFieldInfo)> GetRelevantFields<T>(
        Type type, IEnumerable<Type>? ignoredDerivedTypes = null)
        where T : class
    {
        var flags = SearchFlags;
        var fields = type.GetFields(flags)
            .Where(f =>
               f.GetCustomAttribute<CplxIgnoreAttribute>() == null 
            && f.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching fields backing auto-generated properties 
        foreach (var f in fields)
        {
            if (ignoredDerivedTypes?.Any(t => f.FieldType.IsAssignableTo(t)) ?? false)
                /**/ ;// Do nothing, ignored type
            else if (f.FieldType.IsAssignableTo(typeof(T)))
                yield return (f, new(false));
            else if (f.FieldType.IsAssignableTo(typeof(IEnumerable<T>)))
                yield return (f, new(true));
        }
    }


    record CplxRelevantPropertyInfo(bool IsEnumerable, bool IsBacked);
    private static IEnumerable<(PropertyInfo, CplxRelevantPropertyInfo)> GetRelevantProperties<T>(
        Type type, IEnumerable<Type>? ignoredDerivedTypes = null)
        where T : class
    {
        var flags = SearchFlags;
        var properties = type.GetProperties(flags)
            .Where(p =>
               p.GetMethod != null // Only consider get-able properties
            && p.GetCustomAttribute<CplxIgnoreAttribute>() == null
            && p.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching auto-generated properties
        foreach (var p in properties)
        {
            if(ignoredDerivedTypes?.Any(t => p.PropertyType.IsAssignableTo(t)) ?? false)
                /**/ ;// Do nothing, ignored type
            else if (p.PropertyType.IsAssignableTo(typeof(T)))
                yield return (p, new(false, HasBackingField<T>(p)));
            else if (p.PropertyType.IsAssignableTo(typeof(IEnumerable<T>)))
                yield return (p, new(true, HasBackingField<IEnumerable<T>>(p)));
        }
    }


    /// <summary>
    /// Test if the property has a backing field
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="property">Property type information</param>
    /// <returns>true if a backing field has been found</returns>
    private static bool HasBackingField<T>(PropertyInfo property)
    {
        var declaringType = property.DeclaringType!;
        var fields = declaringType.GetFields(SearchFlags).Where(f => f.FieldType.IsAssignableTo(typeof(T)));
        var autoGeneratedFields = fields.Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() != null);
        string expectedBackingfieldName = $"<{property.Name}>k__BackingField"; // TODO : is this stable ?
        return autoGeneratedFields.Any(f => f.Name == expectedBackingfieldName);
    }
}

