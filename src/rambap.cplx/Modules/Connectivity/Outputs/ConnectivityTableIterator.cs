using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Modules.Connectivity.Model;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTableIterator : IIterator<ConnectivityTableContent>
{
    public IEnumerable<ConnectivityTableContent> MakeContent(Pinstance content)
    {
        var connectivity = content.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnection(content);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByTopmostEndpoints(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.ConnectorA.TopMostUser() != groupLeftConnector;
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

    public static IEnumerable<Connection> GetAllConnection(Pinstance instance)
    {
        // Return all connection, NOT flattening grouped ones (Twisting / Sielding)
        foreach (var c in instance.Connectivity()!.Connections)
            yield return c;
        foreach(var subcomp in instance.Components)
        {
            if(instance.Connectivity != null) // TBD : include even non conectivity defining components ?
                foreach(var c in GetAllConnection(subcomp.Instance))
                    yield return c;
        }
    }
}
