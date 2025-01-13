using rambap.cplx.Export.Tables;
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

    public enum ConnectorIdentity
    {
        Immediate,
        Topmost
    }

    public static DelegateColumn<ConnectivityTableContent> ConnectorName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "Connector",
            ColumnTypeHint.String,
            i => i.GetImmediateConnector(side).Name);

    public static DelegateColumn<ConnectivityTableContent> ConnectorFullName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "Connector",
            ColumnTypeHint.String,
            i => i.GetImmediateConnector(side).FullDefinitionName());

    public static DelegateColumn<ConnectivityTableContent> TopMostConnectorName(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "TopMostConnector",
            ColumnTypeHint.String,
            i => i.GetTopMostConnector(side).Name);

    public static DelegateColumn<ConnectivityTableContent> ConnectorPartPN(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "Part",
            ColumnTypeHint.String,
            i => i.GetTopMostConnector(side).Owner!.ImplementingInstance.PN);

    public static DelegateColumn<ConnectivityTableContent> ConnectorPartCN(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "CN",
            ColumnTypeHint.String,
            i => i.GetTopMostConnector(side).Owner!.ImplementingInstance.CN);

    public static DelegateColumn<ConnectivityTableContent> ConnectorPartCID(ConnectorSide side)
        => new DelegateColumn<ConnectivityTableContent>(
            "CID",
            ColumnTypeHint.String,
            i => i.GetTopMostConnector(side).Owner!.ImplementingInstance.CID());

    public static DelegateColumn<ConnectivityTableContent> Dashes(string title = "-- Connect to --")
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            ColumnTypeHint.String,
            i => new string('-',title.Length));

    public static DelegateColumn<ConnectivityTableContent> ConnectionKind()
        => new DelegateColumn<ConnectivityTableContent>(
            "Kind",
            ColumnTypeHint.String,
            i => i.GetConnectionKind.ToString());
}
