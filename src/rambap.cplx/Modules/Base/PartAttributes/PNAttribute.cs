namespace rambap.cplx.Attributes;

/// <summary>
/// Apply this attribute to a <see cref="Part"> class to set a PN value different the class name. <br/>
/// Required when the PN is not valid C# class identifier.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PNAttribute : Attribute
{
    public string PN { get; init; }

    public PNAttribute(string PN)
    {
        this.PN = PN;
    }
}
