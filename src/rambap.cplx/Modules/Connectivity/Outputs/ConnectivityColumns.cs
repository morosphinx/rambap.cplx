using rambap.cplx.Export;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal static class ConnectivityColumns
{

    //public static DelegateColumn<ConnectivityTableContent> //LeftPart(ConnectorSide side, IColumn<ComponentContent> //componentColumn, string? title)
    //{
    //    return new DelegateColumn<ConnectivityTableContent>(
    //        title ?? componentColumn.Title,
    //        componentColumn.TypeHint,
    //        i => componentColumn.CellFor(i.GetConnector//(side).TopMostUser().Owner
    //            )
    //        );
    //}

    public static DelegateColumn<ConnectivityTableContent> ConnectorName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "Connector",
            ColumnTypeHint.String,
            i => i.GetImmediateConnector(side).Name);

    public static DelegateColumn<ConnectivityTableContent> ConnectorFullName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "ConnectorFullName",
            ColumnTypeHint.String,
            i => i.GetImmediateConnector(side).FullDefinitionName());

    public static DelegateColumn<ConnectivityTableContent> TopMostConnectorName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "TopMostConnector",
            ColumnTypeHint.String,
            i => i.GetTopMostConnector(side).Name);

    public static DelegateColumn<ConnectivityTableContent> Dashes()
        => new DelegateColumn<ConnectivityTableContent>(
            "-- Connect to --",
            ColumnTypeHint.String,
            i => "----------------");

    public static DelegateColumn<ConnectivityTableContent> ConnectionKind()
        => new DelegateColumn<ConnectivityTableContent>(
            "Kind",
            ColumnTypeHint.String,
            i => i.GetConnectionKind.ToString());
}
