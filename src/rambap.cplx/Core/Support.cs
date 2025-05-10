using rambap.cplx.Attributes;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static rambap.cplx.Core.Support;

// Unit testing access some internal properties : 
[assembly: InternalsVisibleTo("rambap.cplxtests.CoreTests")]

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
            // Search type and parent for all fields or properties assignable to T that does not have the CplxIgnore Attribute
            var validMembers = GetRelevantMembers<T>(evalType, ignoredDerivedTypes);
            if (!includeUnbackedProperties)
            {
                // Avoid matching expression bodied properties, such as "Part Name => Other ;"/
                // Expression bodied properties have a get accessor, no set, and cannot be initialised
                validMembers = validMembers
                    .Where(v => v.memberInfo is not PropertyInfo || v.scanInfo.IsBacked);
            }
            foreach (var m in validMembers)
            {
                // Create a struct to pack the info to the rest of cplx
                var member = m.memberInfo;
                bool isPublicOrAssembly = member switch
                {
                    FieldInfo f => f.IsPublic || f.IsAssembly,
                    PropertyInfo p => p.GetMethod!.IsPublic || p.GetMethod!.IsAssembly,
                    _ => throw new NotImplementedException()
                };
                var rename = member.GetCustomAttribute<RenameAttribute>()?.Name;
                var isEnumerable = m.scanInfo.IsEnumerable;
                var cplxMemberType = member switch
                {
                    FieldInfo f => PropertyOrFieldType.Field,
                    PropertyInfo p => m.scanInfo.IsBacked
                        ? PropertyOrFieldType.BackedProperty
                        : PropertyOrFieldType.UnbackedProperty,
                    _ => throw new NotImplementedException()
                };
                PropertyOrFieldInfo cplxInfo = new()
                {
                    Name = rename ?? member.Name,
                    Comments = member.GetCustomAttributes<ComponentDescriptionAttribute>(),
                    IsPublicOrAssembly = isPublicOrAssembly,
                    Type = cplxMemberType,
                    IsFromAndEnumerable = isEnumerable,
                    IndexInEnumerable = 0,
                };

                // Work on the object itself
                object? reflexionValue = member switch
                {
                    FieldInfo f => f.GetValue(obj),
                    PropertyInfo p => p.GetValue(obj),
                    _ => throw new NotImplementedException()
                };
                Type reflexionType = member switch
                {
                    FieldInfo f => f.FieldType,
                    PropertyInfo p => p.PropertyType,
                    _ => throw new NotImplementedException()
                };
                if (isEnumerable)
                {
                    int idx = 0;
                    var val = reflexionValue as IEnumerable<T>;
                    // Do not auto-contruct enumerable
                    // Empty enumerables do not carry any meaning for cplx
                    if (val != null)
                        foreach(var i in val)
                            onData(i!, cplxInfo with { IndexInEnumerable = idx ++ });

                } else
                {
                    var val = reflexionValue as T;
                    if (val is null && constructNulls)
                    {
                        // Auto construct the property or field
                        val = constructor(reflexionType, cplxInfo);
                        switch (member)
                        {
                            case FieldInfo f: f.SetValue(obj, val); break;
                            case PropertyInfo p:
                                // TBD : check that it realy throw in case of unbacked
                                // Previously we init-ed all field before all properties
                                // this isn't the case. What happen if the backing field
                                // is after the backed property in the declaration ?
                                p.SetValue(obj, val); // Will throw if no set accessor ({get;} only, or unbacked)
                                break;
                            default: throw new NotImplementedException();
                        }
                    }
                    if (val != null || acceptNulls)
                        onData(val!, cplxInfo); // TBD : Throw on null, even if accept nulls ?
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

    record CplxRelevantMemberInfo(bool IsEnumerable, bool IsBacked);
    private static IEnumerable<(MemberInfo memberInfo, CplxRelevantMemberInfo scanInfo)> GetRelevantMembers<T>(
        Type scannedType, IEnumerable<Type>? ignoredDerivedTypes = null)
        where T : class
    {
        var flags = SearchFlags ;
        var validMembers = scannedType.GetMembers(flags)
            .Where(m => m switch
            {
                FieldInfo f => true,
                PropertyInfo p => p.GetMethod != null, // Only consider get-able properties
                _ => false
            })
            .Where(m => m.GetCustomAttribute<CplxIgnoreAttribute>() == null
                     && m.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching auto-generated properties
        var membersWithType =
            validMembers.Select<MemberInfo, (MemberInfo info, Type type)>(m => m switch
            {
                FieldInfo f => (f,f.FieldType),
                PropertyInfo p => (p, p.PropertyType),
                _ => throw new NotImplementedException(),
            });
        foreach (var m in validMembers)
        {
            var memberType = m switch
            {
                FieldInfo f => f.FieldType,
                PropertyInfo p => p.PropertyType,
                _ => throw new NotImplementedException()
            };
            if (ignoredDerivedTypes?.Any(t => memberType.IsAssignableTo(t)) ?? false)
                continue ;// Do nothing, ignored type
            else if (memberType.IsAssignableTo(typeof(T)))
            {
                bool isBacked = false;
                if (m is PropertyInfo p)
                    isBacked = HasBackingField<T>(p);
                yield return (m, new(false, isBacked));

            }
            else if (memberType.IsAssignableTo(typeof(IEnumerable<T>)))
            {
                bool isBacked = false;
                if (m is PropertyInfo p)
                    isBacked = HasBackingField<IEnumerable<T>>(p);
                yield return (m, new(true, isBacked));
            }
            // Cases of property groups
            // else if (memberType.IsAssignableTo(typeof(PropertyGroup)))
            // {
            //     GetRelevantMembers<T>(memberType, ignoredDerivedTypes);
            // }
            // else if (memberType.IsAssignableTo(typeof(IEnumerable<PropertyGroup>)))
            // {
            // 
            // }
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

