using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

using TopmostConnectionGroup = (Port LeftTopMost, Port RigthTopMost, IEnumerable<SignalPortConnection> Connections);

internal static class ConnectionHelpers
{
    /// <summary>
    /// Compare tuples of port, allowing swaps <br/>
    /// Eg : (A, B) is equal to (B, A)
    /// </summary>
    class LinkNondirectionalEqualityComparer : EqualityComparer<(Port A, Port B)>
    {
        public override bool Equals((Port A, Port B) x, (Port A, Port B) y)
            => (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

        public override int GetHashCode([DisallowNull] (Port A, Port B) obj)
            => obj.A.GetHashCode() ^ obj.B.GetHashCode();
    }

    /// <summary>
    /// Group all connections by the path they take <br/>
    /// </summary>
    /// <param name="connections">List of connection</param>
    /// <returns>Group where all connections have the same topMost (Port from, Port to) path</returns>
    public static IEnumerable<TopmostConnectionGroup> GroupConnectionsByPath(
        IEnumerable<SignalPortConnection> connections)
    {
        (Port, Port) GetTopMostConnectors(SignalPortConnection con)
            => (con.LeftPort.GetUpperUsage(), con.RightPort.GetUpperUsage());
        var linkComparer = new LinkNondirectionalEqualityComparer();

        var allPairDirectionDependant = connections.Select(GetTopMostConnectors).Distinct();
        var allPairDirectionIndependant = allPairDirectionDependant.Distinct(linkComparer);

        var groups = connections.GroupBy(GetTopMostConnectors, linkComparer);

        return groups.Select(g => (g.Key.Item1, g.Key.Item2, (IEnumerable<SignalPortConnection>)g));
    }
}
