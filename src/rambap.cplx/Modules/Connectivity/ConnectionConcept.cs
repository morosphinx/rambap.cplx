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

    public required List<ConnectablePort> Connectors { get; init; }
    public required List<WireablePort> Wireables { get; init; }
    public required List<IAssemblingConnection> Connections { get; init; }
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
        var selfConnectors = new List<ConnectablePort>();
        ScanObjectContentFor<ConnectablePort>(template,
            (p, s) => {
                selfConnectors.Add(p);
            });
        var selfWirings = new List<WireablePort>();
        ScanObjectContentFor<WireablePort>(template,
            (p, s) => {
                selfWirings.Add(p);
            });

        // At this point no connector in the selfConnectorList has a definition
        if (template is IPartConnectable a)
        {
            var connectionBuilder = new ConnectionBuilder(instance, template);
            // User defined connection and exposition are created from here
            a.Assembly_Connections(connectionBuilder);
            foreach (var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }

            var selfDefinedConnection = connectionBuilder!.Connections;
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

            var connectivity = new InstanceConnectivity()
            {
                Connectors = selfConnectors,
                Wireables = selfWirings,
                Connections = selfDefinedConnection.ToList(),
                Wirings = selfDefinedWirings.ToList(),
            };
            CheckInterfaceContracts(template, connectivity);
            return connectivity;
        }
        else if (selfConnectors.Any())
        {
            // Force definition on every connector, even if the part is not an IPartConnectable
            foreach (var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }
            foreach (var w in selfWirings)
            {
                if (!w.HasbeenDefined)
                    w.DefineAsHadHoc();
            }
            var connectivity = new InstanceConnectivity()
            {
                Connectors = selfConnectors,
                Wireables = selfWirings,
                Connections = [],
                Wirings = [],
            };
            CheckInterfaceContracts(template, connectivity);
            return connectivity;
        }
        else return null;
    }

    private void CheckInterfaceContracts(Part part, InstanceConnectivity connectivity)
    {
        if (part is ISingleMateable)
            if (connectivity.Connections.Count > 1)
                throw new InvalidDataException($"{part} implement {nameof(ISingleMateable)} but has more than one {nameof(ConnectablePort)}");
        if (part is ISingleWireable)
            if (connectivity.Wireables.Count > 1)
                throw new InvalidDataException($"{part} implement {nameof(ISingleWireable)} but has more than one {nameof(WireablePort)}");
    }
}