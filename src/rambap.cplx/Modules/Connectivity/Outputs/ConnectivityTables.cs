using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTables
{
    public static TableProducer<ConnectivityTableContent> ConnectionTable()
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator(),
            Columns = [
                    ConnectorPartCID(ConnectorSide.Left),
                    ConnectorPartCN(ConnectorSide.Left),
                    ConnectorPartPN(ConnectorSide.Left),
                    ConnectorFullName(ConnectorSide.Left),
                    ConnectorName(ConnectorSide.Left),
                    Dashes(),
                    ConnectionKind(),
                    ConnectorName(ConnectorSide.Rigth),
                    ConnectorFullName(ConnectorSide.Rigth),
                    ConnectorPartPN(ConnectorSide.Rigth),
                    ConnectorPartCN(ConnectorSide.Rigth),
                    ConnectorPartCID(ConnectorSide.Rigth),
                ]
        };

    public static TableProducer<IComponentContent> InterfaceControlDocumentTable()
        => new TableProducer<IComponentContent>()
        {
            Iterator = new ICDTableIterator(),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ]
        };
}
