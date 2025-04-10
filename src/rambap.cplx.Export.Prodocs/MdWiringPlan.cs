using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.Modules.Documentation.Outputs;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;

namespace rambap.cplx.Export.Prodocs;

public class MdWiringPlan : SinglePInstanceCustomFile
{
    private TextTableFile ComponentsTable
        => new TextTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = SystemViewTables.ComponentInventory(false)
        };

    private static bool BreakOnPathChange(ConnectionTableProperty p1, ConnectionTableProperty p2)
        => p1.LeftUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
            != p2.LeftUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
        || p1.RigthUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage()
            != p2.RigthUpperUsagePort.GetShallowestStructuralEquivalence().GetUpperUsage();

    private TextTableFile WiringTable
        => new TextTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = ConnectivityTables.WiringTable(false)
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

""";
}
