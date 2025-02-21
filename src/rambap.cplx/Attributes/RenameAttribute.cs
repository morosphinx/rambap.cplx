using static System.Net.Mime.MediaTypeNames;

namespace rambap.cplx.Attributes;

/// <summary>
/// Apply this attribute to a property/field to rename it in CPLX documentation <br/>
/// This is intended to allow documenting CN and properties whose names not valid C# identifiers <br/>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class RenameAttribute : Attribute
{
    public string Name { get; }
    public RenameAttribute(string name)
    {
        Name = name;
    }
}
