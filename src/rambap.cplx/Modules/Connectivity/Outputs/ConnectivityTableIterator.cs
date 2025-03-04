using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ConnectivityTableIterator
{
    public enum ConnectionKind
    {
        Assembly,
        Wiring
    }

    public static IEnumerable<ConnectivityTableContent> MakeConnectivityTableContent(
        Component c,
        ConnectionKind iteratedConnectionKind)
    {
        var instance = c.Instance;
        var connectivity = instance.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnections(instance, iteratedConnectionKind);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        // TODO / TBD : grouping previously happenned at global level. npt equivalent to grouping in
        // post trandform ?

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByPath(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.LeftPort.GetUpperUsage() != groupLeftConnector;
                if (shouldReverse)
                    yield return new ConnectivityTableContent()
                    {
                        Connection = connection,
                        // Invert left/Rigth of group
                        LeftUpperUsagePort = groupRightConnector,
                        RigthUpperUsagePort = groupLeftConnector
                    };
                else
                    yield return new ConnectivityTableContent()
                    {
                        Connection = connection,
                        LeftUpperUsagePort = groupLeftConnector,
                        RigthUpperUsagePort = groupRightConnector
                    };
            }
        }
    }

    public static IEnumerable<SignalPortConnection> GetAllConnections(Pinstance instance, ConnectionKind connectionKind)
    {
        // Return all connection, NOT flattening grouped ones (Twisting / Sielding)
        switch (connectionKind)
        {
            case ConnectionKind.Assembly:
                foreach (var c in instance.Connectivity()?.Connections ?? [])
                    yield return c;
                break;
            case ConnectionKind.Wiring:
                foreach (var c in instance.Connectivity()?.Wirings ?? [])
                    yield return c;
                break;
        }
    }
}
