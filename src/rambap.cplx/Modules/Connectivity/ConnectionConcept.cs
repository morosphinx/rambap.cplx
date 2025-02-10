using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

namespace rambap.cplx.Modules.Connectivity;

public class InstanceConnectivity : IInstanceConceptProperty
{
    // TODO : set definition somewhere in the Part
    public bool IsACable { get; init; } = true;

    public required List<Port> Connectors { get; init; }
    public required List<Port> Wireables { get; init; }
    public required List<AssemblingConnection> Connections { get; init; }
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
        Port? MakePort(SignalPort p, PropertyOrFieldInfo s){
            var newPort = new Port()
            {
                Label = s.Name,
                Owner = instance,
                IsPublic = s.IsPublicOrAssembly,
            };
            // Prevent double implementation of local ports
            if (p.Implementations.TryPeek(out var partPort))
                if (p.LocalImplementation.Owner == instance)
                    return null; // Port is already implemented, ignore it
            if (s.Type == PropertyOrFieldType.UnbackedProperty)
            {
                // Express an exposition
                var subPart = template.AssertIsOwnedBySubComponent(p);
                var subInstance = subPart.ImplementingInstance;
                var subPort = p.Implementations.Peek();
                newPort.DefineAsAnExpositionOf(subPort);
            }
            // Register as an implementation
            newPort.Implement(p);
            return newPort;
        }

        var portsConnectable = new List<Port>();
        ScanObjectContentFor<ConnectablePort>(template,
            (p, s) => {
                var newPort = MakePort(p, s);
                if(newPort != null)
                    portsConnectable.Add(newPort);
            },true);
        ScanObjectContentFor<IEnumerable<ConnectablePort>>(template,
            (iep, s) => {
                foreach(var p in iep)
                {
                    var newPort = MakePort(p, s);
                    if (newPort != null)
                        portsConnectable.Add(newPort);
                }
            }, true);


        var portWireable = new List<Port>();
        ScanObjectContentFor<WireablePort>(template,
            (p, s) => {
                var newPort = MakePort(p, s);
                if (newPort != null)
                    portWireable.Add(newPort);
            }, true);
        ScanObjectContentFor<IEnumerable<WireablePort>>(template,
            (iep, s) => {
                foreach (var p in iep)
                {
                    var newPort = MakePort(p, s);
                    if (newPort != null)
                        portWireable.Add(newPort);
                }
            }, true);

        bool hasAnyPort = portsConnectable.Any() || portWireable.Any();

        // At this point no connector in the selfConnectorList has a definition
        if (template is IPartConnectable a)
        {
            // All signalport should have an implementing port at this point
            
            // User defined exposition are created from here
            var portBuilder = new PortBuilder(instance, template);
            a.Assembly_Ports(portBuilder);

            // User defined connections are created from here
            var connectionBuilder = new ConnectionBuilder(instance, template);
            a.Assembly_Connections(connectionBuilder);

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
                Connectors = portsConnectable,
                Wireables = portWireable,
                Connections = selfDefinedConnection.ToList(),
                Wirings = selfDefinedWirings.ToList(),
            };
            CheckInterfaceContracts(template, connectivity);
            return connectivity;
        }
        else if (hasAnyPort)
        {
            var connectivity = new InstanceConnectivity()
            {
                Connectors = portsConnectable,
                Wireables = portWireable,
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