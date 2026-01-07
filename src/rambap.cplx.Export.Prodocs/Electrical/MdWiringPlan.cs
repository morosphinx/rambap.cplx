using rambap.cplx.Core;
using rambap.cplx.Export.CoreTables;
using rambap.cplx.Export.Text;
using rambap.cplx.Instantiation;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplx.Export.Prodocs.Electrical;

public class MdWiringPlan : TxtPInstanceFile
{
    public List<string> WiringDescriptionTitles = ["Wirings"];

    private TxtTableFile ComponentsTable
        => new TxtTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = new ComponentInventory(new DocumentationPerimeter_SinglePartAndItsContents())
        };

    private static bool BreakOnPathChange(ConnectivityTableProperty p1, ConnectivityTableProperty p2)
        => p1.LeftEndpointPort != p2.LeftEndpointPort
        || p1.RigthEndpointPort != p2.RigthEndpointPort;

    private TxtTableFile WiringTable
        => new TxtTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = new WiringTable(new DocumentationPerimeter_SinglePartAndItsContents())
            with
            {
                AddTableBreakCondition = (l1, l2) =>
                    BreakOnPathChange((l1 as IPropertyContent<ConnectivityTableProperty>)!.Property,
                                      (l2 as IPropertyContent<ConnectivityTableProperty>)!.Property)
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
