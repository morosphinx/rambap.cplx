using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using rambap.cplx.PartProperties;
using System.Reflection;
using static rambap.cplx.Modules.Documentation.InstanceDocumentation;
using static rambap.cplx.Core.Support;
using rambap.cplx.PartInterfaces;
using rambap.cplx.Export;

namespace rambap.cplx.Modules.Documentation;

public class InstanceDocumentation : IInstanceConceptProperty
{
    public class NamedText(string title, string text)
    {
        public string Title => title;
        public string Text => text;
    }

    public List<NamedText> Descriptions { get; init; } = new();
    public List<NamedText> Links { get; init; } = new();


    public string GetAllLineDescription()
    {
        var descriptions = Descriptions.Select(d => d.Text);
        return string.Join("\r\n", descriptions);
    }
    public string GetSingleLineDescription()
    {
        var firstDesc = Descriptions.FirstOrDefault()?.Text;
        if (firstDesc == null) return "";
        var idx = firstDesc.IndexOfAny(['\r', '\n']);
        if (idx > 0) return firstDesc.Substring(0, idx);
        else return firstDesc;
    }

    // 
    public Func<Pinstance, IEnumerable<(string, IInstruction)>>? MakeAdditionalDocuments { get; init; }
}

internal class DocumentationConcept : IConcept<InstanceDocumentation>
{
    public override InstanceDocumentation? Make(Pinstance instance, Part template)
    {
        List<NamedText> descriptions = new();
        // Add description defined in attributes
        var descattrs = template.GetType().GetCustomAttributes<PartDescriptionAttribute>(); // TODO / TBD : inherit ?
        if (descattrs != null)
        {
            foreach (var d in descattrs)
            {
                descriptions.Add(new NamedText(d.Title, d.Text));
            }
        }
        // Add Description defined in properties
        ScanObjectContentFor<Description>(template,
            (d, i) => descriptions.Add(new NamedText(i.Name, d.Text)), true);

        List<NamedText> links = new();
        // Add links defined in properties
        ScanObjectContentFor<Link>(template,
            (d, i) => links.Add(new NamedText(i.Name, d.Hyperlink)), true);

        Func<Pinstance, IEnumerable<(string, IInstruction)>>? makeAdditionDocuments = null;
        if(template is IPartAdditionalDocuments a)
        {
            var documentationBuilder = new DocumentationBuilder(instance, template);
            a.Additional_Documentation(documentationBuilder);
            // TODO : this keep the DocumentationBuilder in memory. Is it ok ? 
            makeAdditionDocuments =
                documentationBuilder.MakeAllAdditionInstructions;
        }

        bool hasAdditionalDocuments = makeAdditionDocuments != null;
        bool hasDocumentation = hasAdditionalDocuments || descriptions.Count > 0 || links.Count > 0;

        if (hasDocumentation)
            return new InstanceDocumentation()
            {
                Descriptions = descriptions,
                Links = links,
                MakeAdditionalDocuments = makeAdditionDocuments,
            };
        else
            return null;
    }
}