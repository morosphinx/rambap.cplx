using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Modules.Connectivity.Model;

using TopmostConnectionGroup = (SignalPort LeftTopMost, SignalPort RigthTopMost, IEnumerable<SignalPortConnection> Connections);

internal static class ConnectionHelpers
{
    class LinkNondirectionalEqualityComparer : EqualityComparer<(SignalPort A, SignalPort B)>
    {
        public override bool Equals((SignalPort A, SignalPort B) x, (SignalPort A, SignalPort B) y)
            => (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

        public override int GetHashCode([DisallowNull] (SignalPort A, SignalPort B) obj)
            => obj.A.GetHashCode() ^ obj.B.GetHashCode();
    }

    public static IEnumerable<TopmostConnectionGroup> GroupConnectionsByTopmostPort(
        IEnumerable<SignalPortConnection> connections)
    {
        (SignalPort, SignalPort) GetTopMostConnectors(SignalPortConnection con)
            => (con.LeftPort.TopMostUser(), con.RightPort.TopMostUser());
        var linkComparer = new LinkNondirectionalEqualityComparer();

        var allPairDirectionDependant = connections.Select(GetTopMostConnectors).Distinct();
        var allPairDirectionIndependant = allPairDirectionDependant.Distinct(linkComparer);

        var groups = connections.GroupBy(GetTopMostConnectors, linkComparer);

        return groups.Select(g => (g.Key.Item1, g.Key.Item2, (IEnumerable<SignalPortConnection>)g));
    }
}
