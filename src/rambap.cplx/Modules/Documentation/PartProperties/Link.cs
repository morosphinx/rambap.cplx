#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Documentation. Hyperlink to an external ressource
/// </summary>
/// <param name="hyperlink"></param>
public class Link(string hyperlink)
{
    public static implicit operator Link(string hyperlink) => new Link(hyperlink);
    public string Hyperlink => hyperlink;
}