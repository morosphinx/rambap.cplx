namespace rambap.cplx.PartProperties;

/// <summary>
/// Documentation. Hyperlink to an external ressource
/// </summary>
/// <param name="hyperlink"></param>
public class Link(string hyperlink)
{
    public static implicit operator Link(string hyperlink) => new Link(hyperlink);
    public string Hyperlink => hyperlink;
}