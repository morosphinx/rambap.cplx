using rambap.cplx.Core;
using rambap.cplx.Modules.Documentation;

namespace rambap.cplx.Export.Prodocs;

internal static class CommonSections
{
    public static string CommonHeader(Component component) =>
 $"""
 |#|Value|
 |-|-----|
 |PN|{component.PN}|
 |Rev|{component.Instance.Revision}|
 |Ver|{component.Instance.Version}|
 |Date|{cplx.Globals.GenerationDate}|
 """;

    public static string JoinStrings(this IEnumerable<string> lines, string separator = "\r\n")
        => string.Join(separator, lines);


    public static IEnumerable<string> MarkdownDocLines(Component component,
        IEnumerable<string> titles,
        bool titleAreAccepted = true,
        string sectionHeader = "###")
    {
        Func<InstanceDocumentation.NamedText, bool> titleAccept = (t) => titles.Contains(t.Title);
        Func<InstanceDocumentation.NamedText, bool> titleReject = (t) => ! titles.Contains(t.Title);
        return MarkdownDocLines(component,
            titleAreAccepted ? titleAccept : titleReject,
            sectionHeader);
    }

    public static IEnumerable<string> MarkdownDocLines(Component Component,
        Func<InstanceDocumentation.NamedText,bool>? selector = null,
        string sectionHeader = "###")
    {
        if (selector == null) selector = (t) => true;
        if(Component.Instance.Documentation() is var docu and not null)
        {
            var selectDescs = docu.Descriptions.Where(d => selector(d));
            foreach(var desc in selectDescs)
            {
                yield return $"{sectionHeader} {desc.Title} :";
                yield return desc.Text;
                yield return "";
            }
        }
    }
}
