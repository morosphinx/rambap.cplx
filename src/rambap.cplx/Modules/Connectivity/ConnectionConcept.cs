using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;
using static rambap.cplx.Core.Support;
using static rambap.cplx.PartInterfaces.IPartConnectable;
using static rambap.cplx.PartInterfaces.IPartConnectable.ConnectionBuilder;

namespace rambap.cplx.Modules.Connectivity;

using TopmostConnectionGroup = (Connector LeftTopMost, Connector RigthTopMost, IEnumerable<Connection> Connections);

public class InstanceConnectivity : IInstanceConceptProperty
{
    public required List<Connector> Connectors { get; init; }

    public required List<Cabling> Connections { get; init; }

    public enum DisplaySide
    {
        Left,
        Rigth,
        Both,
    }

    public IEnumerable<Connector> UniqueConnecteds(DisplaySide displaySide = DisplaySide.Both)
    {
        var allConnections = Connections.SelectMany(c => c.Connections);
        var allConnectors = allConnections.SelectMany<Connection,Connector>(con =>
            displaySide switch {
                DisplaySide.Left => [con.ConnectorA],
                DisplaySide.Rigth => [con.ConnectorB],
                DisplaySide.Both => [con.ConnectorA, con.ConnectorB],
                _ => throw new NotImplementedException()
            });
        var allUniques = allConnectors.Distinct();
        var allUniqueParents = allUniques.Select(con => con.TopMostUser());
        var allUniqueTopmost = allUniqueParents.Distinct();
        return allUniqueTopmost;
    }

    class LinkNondirectionalEqualityComparer : EqualityComparer<(Connector A, Connector B)>
    {
        public override bool Equals((Connector A, Connector B) x, (Connector A, Connector B) y)
            => (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

        public override int GetHashCode([DisallowNull] (Connector A, Connector B) obj)
            => obj.A.GetHashCode() ^ obj.B.GetHashCode();
    }

    public IEnumerable<TopmostConnectionGroup> GetConnectionGroups()
    {
        (Connector, Connector) GetTopMostConnectors(Connection con)
            => (con.ConnectorA.TopMostUser(), con.ConnectorB.TopMostUser());
        var linkComparer = new LinkNondirectionalEqualityComparer();

        var allConnections = Connections.SelectMany(c => c.Connections);
        var allPairDirectionDependant = allConnections.Select(GetTopMostConnectors).Distinct();
        var allPairDirectionIndependant = allPairDirectionDependant.Distinct(linkComparer);

        var groups = allConnections.GroupBy(GetTopMostConnectors, linkComparer);

        return groups.Select(g => (g.Key.Item1, g.Key.Item2, (IEnumerable<Connection>) g));
    }

    internal InstanceConnectivity()
    {

    }
}

internal class ConnectionConcept : IConcept<InstanceConnectivity>
{
    public override InstanceConnectivity? Make(Pinstance i, Part template)
    {
        var connectionBuilder = new ConnectionBuilder(template);
        var selfConnectors = new List<Connector>();
        ScanObjectContentFor<Connector>(template,
            (p, s) => { selfConnectors.Add(p);
            });
        // At this point no connector in the seldConnectorList has a definition
        if (template is IPartConnectable a)
        {
            // User defined connection and exposition are created from here
            a.Assembly_Connections(connectionBuilder);
            foreach(var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }
            
            return new InstanceConnectivity()
            {
                Connectors = selfConnectors,
                Connections = connectionBuilder.Connections
            };
        }
        else return null;
    }
}