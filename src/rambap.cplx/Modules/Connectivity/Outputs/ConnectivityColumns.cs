using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal static class ConnectivityColumns
{
    public enum ConnectorIdentity
    {
        Immediate, // Will display the immediate connector
        Topmost, // Will traverse the connector hierarchy and return informatio nrelative to the uppermost Expose() call
    }

    public static DelegateColumn<ConnectivityTableContent> ConnectorName(ConnectorSide side, ConnectorIdentity identity, bool fullName = false)
        => new DelegateColumn<ConnectivityTableContent>(
            "Connector",
            ColumnTypeHint.String,
            i => identity switch
            {
                ConnectorIdentity.Immediate when fullName   => i.GetImmediateConnector(side).Name,
                ConnectorIdentity.Immediate when !fullName  => i.GetImmediateConnector(side).FullDefinitionName(),
                ConnectorIdentity.Topmost when fullName     => i.GetTopMostConnector(side).Name,
                ConnectorIdentity.Topmost when !fullName    => i.GetTopMostConnector(side).FullDefinitionName(),
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ConnectivityTableContent> ConnectorPart(ConnectorSide side, ConnectorIdentity identity, string title, Func<Pinstance,string> getter)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            ColumnTypeHint.String,
            i => getter.Invoke(identity switch
            {
                ConnectorIdentity.Immediate => i.GetImmediateConnector(side).Owner!.ImplementingInstance,
                ConnectorIdentity.Topmost   => i.GetTopMostConnector(side).Owner!.ImplementingInstance,
                _ => throw new NotImplementedException(),
            }));

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
