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
        // Take a signalPort and implement it
        // Note that SignalPorts are do not have 1-1 relation to PropertyOrFieldInfo
        // For exemple in the case of expression backed properties
        Port? MakePort(SignalPort p, PropertyOrFieldInfo s){ // TODO : _Not_ have property or field info used here ? confusion with part property autoconstruction
            var newPort = new Port(
                /// Unbacked Property use the property name as the exposed connector name, otherwise default to <see cref="Part.CplxImplicitInitialization"/> method
                label : s.Type == PropertyOrFieldType.UnbackedProperty ? s.Name : p.Name ?? s.Name,
                owner : instance,
                isPublic : s.IsPublicOrAssembly
            );
            if (p.Implementations.TryPeek(out var partPort))
            {
                // There is an implementation, check that it's not on this part already
                if (p.LocalImplementation.Owner == instance)
                {
                    // Double definition of a port from the same part => Override name instead
                    p.LocalImplementation.Label = newPort.Label;
                    p.IsPublic |= newPort.IsPublic;
                    return null;
                }
            }
            if (s.Type == PropertyOrFieldType.UnbackedProperty)
            {
                // Express an exposition, exposed port must be owned by a subcomponent
                template.AssertIsOwnedBySubComponent(p);
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
            }, acceptUnbacked : true);

        var portWireable = new List<Port>();
        ScanObjectContentFor<WireablePort>(template,
            (p, s) => {
                var newPort = MakePort(p, s);
                if (newPort != null)
                    portWireable.Add(newPort);
            }, acceptUnbacked:  true);

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

            // Run Signal assignation, if declared
            if(template is IPartSignalDefining sd)
            {
                var signalBuilder = new SignalBuilder(instance, template);
                sd.Assembly_Signals(signalBuilder);
            }
            //
            var selfDefinedConnection = connectionBuilder!.Connections;
            var selfDefinedWirings = connectionBuilder!.Wirings;
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
            // Some port may have been defined / exposed, but no connection
            // Run Signal assignation, if declared
            if (template is IPartSignalDefining sd)
            {
                var signalBuilder = new SignalBuilder(instance, template);
                sd.Assembly_Signals(signalBuilder);
            }
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

    private void RunSignalAssignation(Part template)
    {

    }
}