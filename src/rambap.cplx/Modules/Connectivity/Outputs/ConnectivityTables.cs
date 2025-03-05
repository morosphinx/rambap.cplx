using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTables
{
    public static TableProducer<ICplxContent> ConnectionTable()
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
                    ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPort(PortSide.Left,PortIdentity.UpperUsage,"Port", c => c.Label),
                    Dashes("--"),
                    CablePart("Cable",c => c.CN),
                    Dashes("--"),
                    ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPort(PortSide.Rigth,PortIdentity.UpperUsage,"Port", c => c.Label),
                    Dashes("--"),
                    CablePart("Cable",c => c.Instance.PN),
                    CablePart("Description",c => c.Instance.Documentation()?.Descriptions.FirstOrDefault()?.Text ?? ""),
                    CablePort(PortSide.Left, "L", c => c.GetLowerExposition().Owner.PN),
                    CablePort(PortSide.Rigth, "R", c => c.GetLowerExposition().Owner.PN),
                ]
        };

    public static TableProducer<ICplxContent> WiringTable()
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
                    ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(PortSide.Left,"Connector", c => c.Label),
                    ConnectedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
                    Dashes("--"),
                    EmptyColumn("Signal"),
                    Dashes("--"),
                    ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(PortSide.Rigth,"Connector", c => c.Label),
                    ConnectedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
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
                               return prop.Port.GetShallowestStructuralEquivalence().GetUpperExposition().Label ?? "";
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
