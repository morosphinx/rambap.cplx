using rambap.cplx.Export;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTables
{
    public static Table<ConnectivityTableContent> ConnectionTable()
        => new Table<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator(),
            Columns = [
                    ConnectorFullName(ConnectorSide.Left),
                    ConnectorName(ConnectorSide.Left),
                    Dashes(),
                    ConnectorName(ConnectorSide.Rigth),
                    ConnectorFullName(ConnectorSide.Rigth),
                ]
        };
}
