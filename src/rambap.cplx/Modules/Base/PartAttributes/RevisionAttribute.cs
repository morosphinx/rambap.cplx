namespace rambap.cplx.PartAttributes;

/// <summary>
/// Apply this attribute to a <see cref="Part"> class to set its Revision.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RevisionAttribute : Attribute
{
    public string Revision { get; init; }

    public RevisionAttribute(string revision)
    {
        this.Revision = revision;
    }
}
