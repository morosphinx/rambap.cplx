using rambap.cplx.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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
    /// Iterate all properties and fields of type T of an object. Call onData() on each encountered value of exact or derived type <br/>
    /// Auto construction of scanned properties/fields can be configured by setting the contentMode
    /// </summary>
    /// <typeparam name="T">Type to look for</typeparam>
    /// <param name="obj">Object looked into</param>
    /// <param name="onData">Action to call : void (value, property_or_field_info)</param>
    /// <param name="contentMode">Behavior if the value is null. <br/>
    /// When <see cref="AutoContent.ConstructIfNulls"/>, a parameterless constructor is called </param>
    internal static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        AutoContent contentMode = AutoContent.IgnoreNulls)
        where T : class
        => ScanObjectContentFor(obj, onData, contentMode, (y, s) => (T)Activator.CreateInstance(y)!, false);


    /// <summary>
    /// Iterate all properties and fields of type T of an object. Call onData() on each encountered value of exact or derived type<br/>
    /// Unbacked (eg : var a => b ;) properties may be scanned
    /// </summary>
    /// <typeparam name="T">Type to look for</typeparam>
    /// <param name="obj">Object looked into</param>
    /// <param name="onData">Action to call : void (value, property_or_field_info)</param>
    /// <param name="acceptUnbacked">Behavior if the value is null. <br/>
    /// When <see cref="AutoContent.ConstructIfNulls"/>, a parameterless constructor is called </param>
    internal static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        bool acceptUnbacked)
        where T : class
        => ScanObjectContentFor(obj, onData, AutoContent.IgnoreNulls, (y, s) => (T)Activator.CreateInstance(y)!, acceptUnbacked);

    /// <summary>
    /// Iterate all properties and fields of type T of an object. Call onData() on each encountered value of exact or derived type
    /// </summary>
    /// <typeparam name="T">Type to look for</typeparam>
    /// <param name="obj">Object looked into</param>
    /// <param name="onData">Action to call : void (value, property_or_field_info)</param>
    /// <param name="constructor">Constructor called in case the value is null : T (property_or_field_type, property_or_field_info) </param>
    internal static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        Func<Type, PropertyOrFieldInfo, T> constructor)
        where T : class
        => ScanObjectContentFor(obj, onData, AutoContent.ConstructIfNulls, constructor, false);

    // Accept public/internal felds and properties with a get()

    private static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        AutoContent contentMode,
        Func<Type, PropertyOrFieldInfo, T> constructor,
        bool includeUnbackedProperties)
        where T : class
    {
        var objectType = obj.GetType();
        bool ConstructNulls = contentMode == AutoContent.ConstructIfNulls;
        bool AcceptNulls = contentMode == AutoContent.AcceptNulls;
        // Search type and parent for all properties assignable to T that does not have the CplxIgnore Attribute
        var properties = GetPropertiesRecursive<T>(objectType);
        var validProperties = properties.Where(p =>
            p.GetMethod != null &&
            p.GetCustomAttribute<CplxIgnoreAttribute>() == null &&
            p.GetCustomAttribute<CompilerGeneratedAttribute>() == null)// Avoid matching auto-generated properties 
            .Select(p => (p, HasBackingField<T>(p)));
        if (!includeUnbackedProperties)
        {
            // Avoid matching expression bodied properties, such as "Part Name => Other ;"/
            // Expression bodied properties have a get accessor, no set, and cannot be initialised
            validProperties = validProperties.Where(v => v.Item2);
        }

        foreach (var t in validProperties)
        {
            var p = t.p;
            var isBacked = t.Item2;
            bool isPublicOrAssembly = p.GetMethod!.IsPublic || p.GetMethod!.IsAssembly;
            var rename = p.GetCustomAttribute<RenameAttribute>()?.Name;
            PropertyOrFieldInfo info = new() {
                Name = rename ?? p.Name,
                Comments = p.GetCustomAttributes<ComponentDescriptionAttribute>(), 
                IsPublicOrAssembly = isPublicOrAssembly,
                Type = isBacked ? PropertyOrFieldType.BackedProperty : PropertyOrFieldType.UnbackedProperty};
            var val = p.GetValue(obj) as T;
            if (val is null && ConstructNulls)
            {
                val = constructor(p.PropertyType, info);
                p.SetValue(obj, val); // Will throw if no set accessor ({get;} only, or unbacked)
            }
            if (val != null || AcceptNulls)
                onData(val!, info);
        }
        // Search type and parent for all fields assignable to T that does not have the CplxIgnore Attribute
        var fields = GetFieldsRecursive<T>(objectType);
        var validFields = fields.Where(f =>
            f.GetCustomAttribute<CplxIgnoreAttribute>() == null &&
            f.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching fields backing auto-generated properties
        foreach (var f in validFields)
        {
            bool isPublicOrAssembly = f.IsPublic || f.IsAssembly;
            var rename = f.GetCustomAttribute<RenameAttribute>()?.Name;
            PropertyOrFieldInfo info = new(){
                Name = rename ?? f.Name,
                Comments = f.GetCustomAttributes<ComponentDescriptionAttribute>(),
                IsPublicOrAssembly = isPublicOrAssembly,
                Type = PropertyOrFieldType.Field,
            };
            var val = f.GetValue(obj) as T;
            if (val is null && ConstructNulls)
            {
                val = constructor(f.FieldType, info);
                f.SetValue(obj, val);
            }
            if (val != null || AcceptNulls)
                onData(val!, info);
        }
    }

    /// <summary>
    /// CPLX search for private and public properties/fields of the class
    /// Parent classes are iterated manualy to control the property/field ordering
    /// </summary>
    const BindingFlags SearchFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static IEnumerable<PropertyInfo> GetPropertiesRecursive<T>(Type type)
        where T : class
    {
        var flags = SearchFlags;
        var properties = type.GetProperties(flags).Where(p => p.PropertyType.IsAssignableTo(typeof(T)));
        var parentType = type.BaseType;
        if (parentType != typeof(object)) // Do not recurse on type object
        {
            // Recurse, with parent fields before self ones
            IEnumerable<PropertyInfo> allPropertiesIncludingParent = [.. GetPropertiesRecursive<T>(parentType!), .. properties];
            foreach (var f in allPropertiesIncludingParent)
                yield return f;
        }
        else
        {
            foreach (var f in properties)
                yield return f;
        }
    }

    private static IEnumerable<FieldInfo> GetFieldsRecursive<T>(Type type)
        where T : class
    {
        var flags = SearchFlags;
        var fields = type.GetFields(flags).Where(f => f.FieldType.IsAssignableTo(typeof(T)));
        var parentType = type.BaseType;
        if (parentType != typeof(object)) // Do not recurse on type object
        {
            // Recurse, with parent fields before self ones
            IEnumerable<FieldInfo> allFieldsIncludingParent = [.. GetFieldsRecursive<T>(parentType!), .. fields];
            foreach (var f in allFieldsIncludingParent)
                yield return f;
        }
        else
        {
            foreach (var f in fields)
                yield return f;
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

