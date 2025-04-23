using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.Modules.Documentation.Outputs;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;

namespace rambap.cplx.Export.Prodocs;

public class MdWiringPlan : TxtPInstanceFile
{
    public List<string> WiringDescriptionTitles = ["Wirings"];

    private TxtTableFile ComponentsTable
        => new TxtTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = SystemViewTables.ComponentInventory(new DocumentationPerimeter_SinglePart())
        };

    private static bool BreakOnPathChange(ConnectionTableProperty p1, ConnectionTableProperty p2)
        => p1.LeftUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
            != p2.LeftUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
        || p1.RigthUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
            != p2.RigthUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage();

    private TxtTableFile WiringTable
        => new TxtTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = ConnectivityTables.WiringTable(new DocumentationPerimeter_SinglePart())
            with
            {
                AddTableBreakCondition = (l1, l2) =>
                    BreakOnPathChange((l1 as LeafProperty<ConnectionTableProperty>)!.Property,
                                      (l2 as LeafProperty<ConnectionTableProperty>)!.Property)
            },
        };

    public override string GetText() =>
$"""
# WIRING PLAN : {Content.PN}

## Identification
{CommonSections.CommonHeader(Content)}

## Components :

{ComponentsTable.GetAllLines().JoinStrings()}

## Wirings :

{WiringTable.GetAllLines().JoinStrings()}

## Notes :

{CommonSections.MarkdownDocLines(Content, WiringDescriptionTitles).JoinStrings()}

""";
}
