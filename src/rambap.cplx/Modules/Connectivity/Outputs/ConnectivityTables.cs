using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTables
{
    public static TableProducer<ConnectivityTableContent> ConnectionTable(ConnectorIdentity identity)
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator()
            {
                IncludeSubComponentConnections = true,
                IteratedConnectionKind = ConnectivityTableIterator.ConnectionKind.Assembly
            },
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
                    CablePort(ConnectorSide.Left, "L", c => c.GetDeepestExposition().Owner!.ImplementingInstance.PN),
                    CablePort(ConnectorSide.Rigth, "R", c => c.GetDeepestExposition().Owner!.ImplementingInstance.PN),
                ]
        };

    public static TableProducer<ConnectivityTableContent> WiringTable()
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator()
            {
                IncludeSubComponentConnections = false,
                IteratedConnectionKind = ConnectivityTableIterator.ConnectionKind.Wiring
            },
            Columns = [
                    ConnectedComponent(ConnectorSide.Left,ConnectorIdentity.Topmost,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(ConnectorSide.Left,"Connector", c => c.Name),
                    ConnectedPortName(ConnectorSide.Left,ConnectorIdentity.Topmost),
                    Dashes("--"),
                    EmptyColumn("Signal"),
                    Dashes("--"),
                    ConnectedComponent(ConnectorSide.Rigth,ConnectorIdentity.Topmost,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(ConnectorSide.Rigth,"Connector", c => c.Name),
                    ConnectedPortName(ConnectorSide.Rigth,ConnectorIdentity.Topmost,true),
                ]
        };

    public static TableProducer<IComponentContent> InterfaceControlDocumentTable()
        => new TableProducer<IComponentContent>()
        {
            Iterator = new ICDTableIterator(),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree(
                        i =>
                        {
                            if(i.Property is ICDTableContentProperty prop){
                                 if(prop.Port.HasStructuralEquivalence)
                                    return prop.Port.GetShallowestStructuralEquivalence().GetTopMostExposition().Name ?? "";
                                 return prop.Port.Name ?? "";
                            }
                            return "";
                        }),
                    ICDColumns.TopMostPortPart(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ],
        };
}
