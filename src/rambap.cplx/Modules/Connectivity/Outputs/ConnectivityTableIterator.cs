using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Modules.Connectivity.Model;
using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Modules.Connectivity.Outputs;

using TopmostConnectionGroup = (Connector LeftTopMost, Connector RigthTopMost, IEnumerable<Connection> Connections);

internal class ConnectivityTableIterator : IIterator<ConnectivityTableContent>
{
    public IEnumerable<ConnectivityTableContent> MakeContent(Pinstance content)
    {
        var connectivity = content.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnection(content);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        var connectionsGrouped = GroupConnectionsByTopmostEndpoints(connections);

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

    class LinkNondirectionalEqualityComparer : EqualityComparer<(Connector A, Connector B)>
    {
        public override bool Equals((Connector A, Connector B) x, (Connector A, Connector B) y)
            => (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

        public override int GetHashCode([DisallowNull] (Connector A, Connector B) obj)
            => obj.A.GetHashCode() ^ obj.B.GetHashCode();
    }

    public static IEnumerable<TopmostConnectionGroup> GroupConnectionsByTopmostEndpoints(
        IEnumerable<Connection> connections)
    {
        (Connector, Connector) GetTopMostConnectors(Connection con)
            => (con.ConnectorA.TopMostUser(), con.ConnectorB.TopMostUser());
        var linkComparer = new LinkNondirectionalEqualityComparer();

        var allPairDirectionDependant = connections.Select(GetTopMostConnectors).Distinct();
        var allPairDirectionIndependant = allPairDirectionDependant.Distinct(linkComparer);

        var groups = connections.GroupBy(GetTopMostConnectors, linkComparer);

        return groups.Select(g => (g.Key.Item1, g.Key.Item2, (IEnumerable<Connection>)g));
    }
}
