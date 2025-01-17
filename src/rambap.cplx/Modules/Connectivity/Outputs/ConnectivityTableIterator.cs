using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Connectivity.Model;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTableIterator : IIterator<ConnectivityTableContent>
{
    public required bool IncludeSubComponentConnections { get; init; }
    public required ConnectionKind IteratedConnectionKind { get; init; }

    public IEnumerable<ConnectivityTableContent> MakeContent(Pinstance instance)
    {
        var connectivity = instance.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnections(instance, IncludeSubComponentConnections, IteratedConnectionKind);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByTopmostPort(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.LeftPort.TopMostUser() != groupLeftConnector;
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

    public enum ConnectionKind
    {
        Assembly,
        Wiring
    }
    public static IEnumerable<ISignalPortConnection> GetAllConnections(Pinstance instance, bool recursive, ConnectionKind connectionKind)
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
