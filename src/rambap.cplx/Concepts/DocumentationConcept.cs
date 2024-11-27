using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using rambap.cplx.PartProperties;
using System.Reflection;
using static rambap.cplx.Concepts.InstanceDocumentation;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Concepts;

public class InstanceDocumentation : IInstanceConceptProperty
{
    public class NamedText(string title, string text)
    {
        public string Title => title;
        public string Text => text;
    }

    public List<NamedText> Descriptions { get; init; } = new();
    public List<NamedText> Links { get; init; } = new();
}

internal class DocumentationConcept : IConcept<InstanceDocumentation>
{
    public override InstanceDocumentation? Make(Pinstance i, Part template)
    {
        List<NamedText> descriptions = new();
        // Add description defined in attributes
        var descattrs = template.GetType().GetCustomAttributes<PartDescriptionAttribute>(); // TODO / TBD : inherit ?
        if( descattrs != null)
        {
            foreach(var d in descattrs)
            {
                descriptions.Add(new NamedText(d.Title, d.Text));
            }
        }
        // Add Description defined in properties
        ScanObjectContentFor<Description>(template,
            (d, i) => descriptions.Add(new InstanceDocumentation.NamedText(i.Name,d.Text)));

        List<NamedText> links = new();
        // Add links defined in properties
        ScanObjectContentFor<Link>(template,
            (d, i) => links.Add(new InstanceDocumentation.NamedText(i.Name, d.Hyperlink)));

        bool hasDocumentation = descriptions.Count > 0 || links.Count > 0;

        if (hasDocumentation)
            return new InstanceDocumentation() { Descriptions = descriptions, Links = links};
        else
            return null;
    }
}