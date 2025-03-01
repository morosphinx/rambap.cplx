using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTables
{
    public static TableProducer<ICplxContent> ConnectionTable(ConnectorIdentity identity)
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ConnectivityTableContent>()
            {
                PropertyIterator = c => ConnectivityTableIterator.MakeConnectivityTableContent(
                    c, ConnectivityTableIterator.ConnectionKind.Assembly),
                WriteBranches = false,
                RecursionCondition = (c, l) => true,
            },
            ContentTransform = cs => cs.Where(c => c is not IPureComponentContent),
            Columns = [
                    ConnectedComponent(ConnectorSide.Left,identity,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPortName(ConnectorSide.Left,identity),
                    Dashes("--"),
                    CablePart("Cable",c => c.CN),
                    Dashes("--"),
                    ConnectedComponent(ConnectorSide.Rigth,identity,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPortName(ConnectorSide.Rigth,identity),
                    Dashes("--"),
                    CablePart("Description",c => c.Instance.Documentation()?.Descriptions.FirstOrDefault()?.Text ?? ""),
                    CablePort(ConnectorSide.Left, "L", c => c.GetDeepestExposition().Owner.PN),
                    CablePort(ConnectorSide.Rigth, "R", c => c.GetDeepestExposition().Owner.PN),
                ]
        };

    public static TableProducer<ICplxContent> WiringTable(ConnectorIdentity identity)
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ConnectivityTableContent>()
            {
                PropertyIterator = c => ConnectivityTableIterator.MakeConnectivityTableContent(
                    c, ConnectivityTableIterator.ConnectionKind.Wiring),
                WriteBranches = false,
                RecursionCondition = (c,l) => true,
            },
            ContentTransform = cs => cs.Where(c => c is not IPureComponentContent),
            Columns = [
                    ConnectedComponent(ConnectorSide.Left,identity,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(ConnectorSide.Left,"Connector", c => c.Label),
                    ConnectedStructuralEquivalence(ConnectorSide.Left,"Pin",p => p.FullDefinitionName()),
                    Dashes("--"),
                    EmptyColumn("Signal"),
                    Dashes("--"),
                    ConnectedComponent(ConnectorSide.Rigth,identity,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(ConnectorSide.Rigth,"Connector", c => c.Label),
                    ConnectedStructuralEquivalence(ConnectorSide.Rigth,"Pin",p => p.FullDefinitionName()),
                ]
        };

    public static TableProducer<ICplxContent> InterfaceControlDocumentTable()
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ICDTableIterator.ICDTableContentProperty>()
            {

                PropertyIterator = ICDTableIterator.SelectPublicTopmostConnectors,
                PropertySubIterator = ICDTableIterator.SelectConnectorSubs,
                RecursionCondition = (c, l) => c.IsPublic,
                WriteBranches = true,
                GroupPNsAtSameLocation = false,
                StackPropertiesSingleChildBranches = false,
            },
            ContentTransform = contents => contents.Where(c =>
                c switch
                {
                    IPropertyContent<ICDTableIterator.ICDTableContentProperty> lp => true,
                    _ => c.Component.IsPublic, // Private part are still present as leaf, we remove them
                }),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree<ICDTableIterator.ICDTableContentProperty>(
                        i =>
                        {
                            var prop = i.Property;
                            if(prop.Port.HasStructuralEquivalence)
                               return prop.Port.GetShallowestStructuralEquivalence().GetTopMostExposition().Label ?? "";
                            return prop.Port.Label ?? "";
                        }),
                    ICDColumns.TopMostPortPart(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ],
        };
}
