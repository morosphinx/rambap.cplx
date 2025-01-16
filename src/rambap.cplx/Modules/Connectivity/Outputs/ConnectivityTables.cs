using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTables
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
                    ConnectorPart(ConnectorSide.Left,identity,"CID", i => i.CID(" / ")),
                    ConnectorName(ConnectorSide.Left,identity),
                    Dashes("--"),
                    CablePart("Cable",c => c.CN),
                    Dashes("--"),
                    ConnectorPart(ConnectorSide.Rigth,identity,"CID", i => i.CID(" / ")),
                    ConnectorName(ConnectorSide.Rigth,identity),
                ]
        };

    public static TableProducer<ConnectivityTableContent> WiringTable(ConnectorIdentity identity = ConnectorIdentity.Immediate)
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator()
            {
                IncludeSubComponentConnections = false,
                IteratedConnectionKind = ConnectivityTableIterator.ConnectionKind.Wiring
            },
            Columns = [
                    ConnectorPart(ConnectorSide.Left,identity,"CN", i => i.CN),
                    ConnectorName(ConnectorSide.Left,identity),
                    Dashes("--"),
                    EmptyColumn("Signal"),
                    Dashes("--"),
                    ConnectorPart(ConnectorSide.Rigth,identity,"CN", i => i.CN),
                    ConnectorName(ConnectorSide.Rigth,identity),
                ]
        };

    public static TableProducer<IComponentContent> InterfaceControlDocumentTable()
        => new TableProducer<IComponentContent>()
        {
            Iterator = new ICDTableIterator(),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree(
                        i => i.Property is ICDTableContentProperty prop ? prop.Port.TopMostRelevant().Name ?? "e" : "f"),
                    ICDColumns.TopMostPortPart(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ]
        };
}
