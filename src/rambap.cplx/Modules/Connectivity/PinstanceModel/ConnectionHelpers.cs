using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

using TopmostConnectionGroup = (Port LeftTopMost, Port RigthTopMost, IEnumerable<SignalPortConnection> Connections);

internal static class ConnectionHelpers
{
    class LinkNondirectionalEqualityComparer : EqualityComparer<(Port A, Port B)>
    {
        public override bool Equals((Port A, Port B) x, (Port A, Port B) y)
            => (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

        public override int GetHashCode([DisallowNull] (Port A, Port B) obj)
            => obj.A.GetHashCode() ^ obj.B.GetHashCode();
    }

    public static IEnumerable<TopmostConnectionGroup> GroupConnectionsByTopmostPort(
        IEnumerable<SignalPortConnection> connections)
    {
        (Port, Port) GetTopMostConnectors(SignalPortConnection con)
            => (con.LeftPort.GetTopMostUser(), con.RightPort.GetTopMostUser());
        var linkComparer = new LinkNondirectionalEqualityComparer();

        var allPairDirectionDependant = connections.Select(GetTopMostConnectors).Distinct();
        var allPairDirectionIndependant = allPairDirectionDependant.Distinct(linkComparer);

        var groups = connections.GroupBy(GetTopMostConnectors, linkComparer);

        return groups.Select(g => (g.Key.Item1, g.Key.Item2, (IEnumerable<SignalPortConnection>)g));
    }
}
