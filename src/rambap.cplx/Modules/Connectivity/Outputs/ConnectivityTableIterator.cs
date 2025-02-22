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
        bool includeSubComponentConnections,
        ConnectionKind iteratedConnectionKind)
    {
        var instance = c.Instance;
        var connectivity = instance.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnections(instance, includeSubComponentConnections, iteratedConnectionKind);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByTopmostPort(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.LeftPort.GetTopMostUser() != groupLeftConnector;
                if (shouldReverse)
                    yield return new ConnectivityTableContent()
                    {
                        Connection = connection,
                        // Invert left/Rigth of group
                        LeftTopMostConnector = groupRightConnector,
                        RigthTopMostConnector = groupLeftConnector
                    };
                else
                    yield return new ConnectivityTableContent()
                    {
                        Connection = connection,
                        LeftTopMostConnector = groupLeftConnector,
                        RigthTopMostConnector = groupRightConnector
                    };
            }
        }
    }

    public static IEnumerable<SignalPortConnection> GetAllConnections(Pinstance instance, bool recursive, ConnectionKind connectionKind)
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
        if (recursive)
        {
            foreach(var subcomp in instance.Components)
            {
                if(instance.Connectivity != null) // TBD : include even non connectivity defining components ?
                    foreach(var c in GetAllConnections(subcomp.Instance,recursive,connectionKind))
                        yield return c;
            }
        }
    }
}
