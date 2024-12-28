using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Modules.Connectivity.Model;

using TopmostConnectionGroup = (Connector LeftTopMost, Connector RigthTopMost, IEnumerable<Connection> Connections);

internal static class ConnectionHelpers
{
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
