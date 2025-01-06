using rambap.cplx.PartAttributes;
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
    /// <summary> Define execution behavior </summary>
    internal enum AutoContent
    {
        /// <summary> Skip encountered null values </summary>
        IgnoreNulls,
        /// <summary> Process encountered null value </summary>
        AcceptNulls,
        /// <summary> Create and assign a new() value before processing null values </summary>
        ConstructIfNulls
    }

    internal record PropertyOrFieldInfo
    {
        public required string Name { get; init; }
        public required IEnumerable<ComponentDescriptionAttribute> Comments { get; init; }

        // see https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.isassembly
        public required bool IsPublicOrAssembly { get; init; }
    }

    /// <summary>
    /// Iterate all properties and fields of type T of an object. Call onData() on each encountered value of exact or derived type
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
        => ScanObjectContentFor(obj, onData, contentMode, (y, s) => (T)Activator.CreateInstance(y)!);

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
        => ScanObjectContentFor(obj, onData, AutoContent.ConstructIfNulls, constructor);

    // Accept public/internal felds and properties with a get()

    private static void ScanObjectContentFor<T>(
        object obj,
        Action<T, PropertyOrFieldInfo> onData,
        AutoContent contentMode,
        Func<Type, PropertyOrFieldInfo, T> constructor)
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
            p.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching backing auto-generated fields
        foreach (var p in validProperties)
        {
            bool isPublicOrAssembly = p.GetMethod!.IsPublic || p.GetMethod!.IsAssembly;
            PropertyOrFieldInfo info = new() { Name = p.Name, Comments = p.GetCustomAttributes<ComponentDescriptionAttribute>(), IsPublicOrAssembly = isPublicOrAssembly };
            var val = p.GetValue(obj) as T;
            if (val is null && ConstructNulls)
            {
                val = constructor(p.PropertyType, info);
                p.SetValue(obj, val);
            }
            if (val != null || AcceptNulls)
                onData(val!, info);
        }
        // Search type and parent for all fields assignable to T that does not have the CplxIgnore Attribute
        var fields = GetFieldsRecursive<T>(objectType);
        var validFields = fields.Where(f =>
            f.GetCustomAttribute<CplxIgnoreAttribute>() == null &&
            f.GetCustomAttribute<CompilerGeneratedAttribute>() == null); // Avoid matching backing auto-generated fields
        foreach (var f in validFields)
        {
            bool isPublicOrAssembly = f.IsPublic || f.IsAssembly;
            PropertyOrFieldInfo info = new() { Name = f.Name, Comments = f.GetCustomAttributes<ComponentDescriptionAttribute>(), IsPublicOrAssembly = isPublicOrAssembly };
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

    private static IEnumerable<PropertyInfo> GetPropertiesRecursive<T>(Type type)
        where T : class
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
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
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
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
}

