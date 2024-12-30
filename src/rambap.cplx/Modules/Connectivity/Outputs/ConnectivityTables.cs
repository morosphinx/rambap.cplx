using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTables
{
    public static TableProducer<ConnectivityTableContent> ConnectionTable()
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator(),
            Columns = [
                    ConnectorFullName(ConnectorSide.Left),
                    ConnectorName(ConnectorSide.Left),
                    Dashes(),
                    ConnectionKind(),
                    ConnectorName(ConnectorSide.Rigth),
                    ConnectorFullName(ConnectorSide.Rigth),
                ]
        };
}
