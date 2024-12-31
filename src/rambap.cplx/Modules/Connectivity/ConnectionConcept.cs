using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using rambap.cplx.Modules.Connectivity.Model;

namespace rambap.cplx.Modules.Connectivity;

public class InstanceConnectivity : IInstanceConceptProperty
{
    // TODO : set definition somewhere in the Part
    public bool IsACable { get; init; } = true;

    public required List<ConnectablePort> PublicConnectors { get; init; }

    public required List<ConnectablePort> Connectors { get; init; }

    public required List<Mate> Connections { get; init; }
    public required List<WiringAction> Wirings { get; init; }

    public enum DisplaySide
    {
        Left,
        Rigth,
        Both,
    }

    internal InstanceConnectivity()
    {

    }
}

internal class ConnectionConcept : IConcept<InstanceConnectivity>
{
    public override InstanceConnectivity? Make(Pinstance instance, Part template)
    {
        var connectionBuilder = new ConnectionBuilder(template);
        var selfConnectors = new List<ConnectablePort>();
        var selfPublicConnectors = new List<ConnectablePort>();
        ScanObjectContentFor<ConnectablePort>(template,
            (p, s) => {
                selfConnectors.Add(p);
                if(s.IsPublicOrAssembly) selfPublicConnectors.Add(p);
            });
        // At this point no connector in the selfConnectorList has a definition
        if (template is IPartConnectable a)
        {
            // User defined connection and exposition are created from here
            a.Assembly_Connections(connectionBuilder);
            foreach(var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }

            var selfDefinedConnection = connectionBuilder!.Mates;
            var selfDefinedWirings = connectionBuilder!.Wirings;
            // 
            // var groups = ConnectionHelpers.GroupConnectionsByTopmostPort(selfDefinedWirings);
            // var wireGroupedConnections = groups.SelectMany(g =>
            //     g.Connections.All(c => c is WiringAction)
            //         ? [new Bundle(g.Connections.OfType<WiringAction>())]
            //         : g.Connections);

            // All Components that have a Connectivity definition are used as black boxes
            //void AbstractConnectionDueToCable(List<Connection> /cablings, /       IEnumerable<Connector> cableConnectors)
            //{
            //
            //}
            // foreach (var c in instance.Components)
            // {
            //     var connectivity = c.Instance.Connectivity();
            //     if(connectivity != null)
            //     {
            //         if (connectivity.IsACable)
            //         {
            //             var cableConnectors = // connectivity.PublicConnectors;
            //             AbstractConnectionDueToCable// (selfDefinedConnection, cableConnectors);
            //         }
            //     }
            // }

            return new InstanceConnectivity()
            {
                PublicConnectors = selfPublicConnectors,
                Connectors = selfConnectors,
                Connections = selfDefinedConnection.ToList(),
                Wirings = selfDefinedWirings.ToList(),
            };
        }
        else return null;
    }
}