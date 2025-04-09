using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;

namespace rambap.cplx.Export.Prodocs;

public class MdWiringPlan : IInstruction
{
    private Pinstance DocumentedPart { get; init; }

    private TextTableFile ComponentsTable
        => new TextTableFile(DocumentedPart)
        {
            Formater = new MarkdownTableFormater(),
            Table = Modules.Documentation.Outputs.SystemViewTables.ComponentInventory(false),
        };

    private TextTableFile WiringTable
        => new TextTableFile(DocumentedPart)
        {
            Formater = new MarkdownTableFormater(),
            Table = Modules.Connectivity.Outputs.ConnectivityTables.WiringTable(false),
        };

    public MdWiringPlan(Pinstance Target)
    {
        DocumentedPart = Target;
    }

    public void Do(string path)
    {
        using (var file = File.CreateText(path))
        {
            file.Write(FileContents);
        }
    }

    private string FileContents =>
$"""
# WIRING PLAN : {DocumentedPart.PN}

## Identification
{CommonSections.CommonHeader(DocumentedPart)}

## Components :

{ComponentsTable.GetAllLines().GetText()}

## Wirings :

{WiringTable.GetAllLines().GetText()}

""";
}
