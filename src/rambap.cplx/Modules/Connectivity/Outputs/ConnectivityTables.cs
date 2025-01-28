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
                    ConnectorComponent(ConnectorSide.Left,identity,"CID", c => c?.CID(" / ") ?? "."),
                    PortName(ConnectorSide.Left,identity),
                    Dashes("--"),
                    CablePart("Cable",c => c.CN),
                    Dashes("--"),
                    ConnectorComponent(ConnectorSide.Rigth,identity,"CID", c => c?.CID(" / ") ?? "."),
                    PortName(ConnectorSide.Rigth,identity),
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
                    ConnectorComponent(ConnectorSide.Left,ConnectorIdentity.Topmost,"CN", c => c.CN),
                    ConnectorStructuralEquivalenceTopmostPort(ConnectorSide.Left,"Connector", c => c.Name),
                    PortName(ConnectorSide.Left,ConnectorIdentity.Topmost),
                    Dashes("--"),
                    EmptyColumn("Signal"),
                    Dashes("--"),
                    ConnectorComponent(ConnectorSide.Rigth,ConnectorIdentity.Topmost,"CN", c => c.CN),
                    ConnectorStructuralEquivalenceTopmostPort(ConnectorSide.Rigth,"Connector", c => c.Name),
                    PortName(ConnectorSide.Rigth,ConnectorIdentity.Topmost,true),
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
                                    return prop.Port.GetShallowestStructuralEquivalence().TopMostRelevant().Name ?? "";
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
