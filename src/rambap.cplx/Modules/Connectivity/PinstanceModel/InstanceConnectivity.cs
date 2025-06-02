using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Export.Generators;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

public class InstanceConnectivity : IInstanceConceptProperty
{
    // TODO : set definition somewhere in the Part
    public bool IsACable { get; init; } = true;

    public required List<Port> Connectors { get; init; }
    public required List<Port> Wireables { get; init; }

    public required List<Mate> Connections { get; init; }
    public required List<IWiringConnection> WiringConnections { get; init; }

    public required List<PSignal> Signals { get; init; }

    public enum DisplaySide
    {
        Left,
        Rigth,
        Both,
    }

    internal InstanceConnectivity()
    {

    }


    public record WireMesh
    {
        public WireMesh(List<IWiringConnection> connections)
        {
            Connections = connections;
        }

        public List<IWiringConnection> Connections { get; }

        /// <summary>
        /// Return the edges of the wire mesh
        /// </summary>
        public List<Port> GetEdges()
        {
            // Get all port and count them
            var allPorts = Connections.SelectMany<IWiringConnection,Port>(c => [c.LeftPort, c.RightPort]);
            Dictionary<Port, int> portCounts = [];
            foreach(var port in allPorts)
            {
                if(portCounts.ContainsKey(port))
                    portCounts[port]++;
                else
                    portCounts.Add(port, 1);
            }
            // All port that are non unique are not edge
            var edgePorts = portCounts.Where(p => p.Value == 1).Select(p => p.Key);
            return [.. edgePorts ];
        }

        public List<Port> GetEndpoints()
        {
            var edges = GetEdges();
            var endpoints = edges.Select(p => p.GetUpperEndpointIdentityPort()).Distinct();
            return [.. endpoints];
        }

        public List<WireSpool> GetAllUsedWireSpools()
        {
            return [.. Connections.OfType<StructuralWire>().Select(w => w.WireSpool).Distinct()];
        }
    }

    public record WireMesh_DualEnded : WireMesh
    {
        public Port LeftPort { get; }
        public Port RightPort { get; }

        public WireMesh_DualEnded(WireMesh wireMesh) : base(wireMesh.Connections)
        {
            var edges = wireMesh.GetEdges();
            if (wireMesh.GetEdges().Count != 2)
                throw new InvalidOperationException("Wire mesh has more than two ends");
            LeftPort = edges.First();
            RightPort = edges.Last();
        }
    }

    /// <summary>
    /// Group all <see cref="WiringConnections"/> that are part of the same wire mesh <br/>
    /// </summary>
    internal List<WireMesh> GetWireMeshes()
    {
        // Add the connection to the ConnectionDic and all its joined connections
        static void PropagateGroupID(IWiringConnection Connection, int GroupID, Dictionary<IWiringConnection, int> ConnectionDic)
        {
            if (ConnectionDic.ContainsKey(Connection)) return;
            else
            {
                ConnectionDic.Add(Connection, GroupID);
                if(Connection.LeftPort is CWireEnd)
                {
                    // TBD : only propagate on same component ?
                    var leftPortPropagation = Connection.LeftPort.Connections.OfType<IWiringConnection>();
                    foreach (var c in leftPortPropagation) PropagateGroupID(c, GroupID, ConnectionDic);
                }
                if (Connection.RightPort is CWireEnd)
                {
                    var rigthPortPropagation = Connection.RightPort.Connections.OfType<IWiringConnection>();
                    foreach (var c in rigthPortPropagation) PropagateGroupID(c, GroupID, ConnectionDic);
                }
            }
        }
        
        // Dictionary of all wiring connection as they get assigned to groups. Key is wiring, Valye is his groupID
        Dictionary<IWiringConnection, int> connectionDic = new();
        int currentGroupid = 1;
        foreach (var c in WiringConnections)
        {
            if (connectionDic.ContainsKey(c)) continue; // Already part of a groupId, skip
            connectionDic[c] = currentGroupid;
            PropagateGroupID(c, currentGroupid, connectionDic);
            currentGroupid += 1;
        }

        var groups = connectionDic
            .GroupBy(g => g.Value)
            .Select(g => new WireMesh([.. g.Select(c => c.Key)]) )
            .ToList();
        return groups;
    }
}
