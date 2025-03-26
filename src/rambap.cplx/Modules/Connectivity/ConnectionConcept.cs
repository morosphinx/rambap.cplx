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

        var portsWireable = new List<Port>();
        ScanObjectContentFor<WireablePort>(template,
            (p, s) => {
                var newPort = MakePort(p, s);
                if (newPort != null)
                    portsWireable.Add(newPort);
            }, acceptUnbacked:  true);

        var signals = new List<Signal>();
        ScanObjectContentFor<Signal>(template,
            (p, s) =>
            {
                if(p.Owner == null)
                {
                    // localy created Signal with implicit signal => Workaround for now
                    Part.InitPartProperty(template, p, s);
                }
                if(p.Implementation == null)
                    p.MakeImplementation(p.Name ?? s.Name,instance,s.IsPublicOrAssembly);
                signals.Add(p);
            }, acceptUnbacked: true);

        // Apply port construction, defined in IPartConnectable
        if (template is IPartConnectable a1)
        {
            // User defined exposition are created from here
            var portBuilder = new PortBuilder(instance, template);
            a1.Assembly_Ports(portBuilder);
        }

        foreach(var s in signals
            .Where(i => i.Owner == template)
            .OfType<ImplicitAssignedSignal>())
        {
            foreach(var p in s.AssignedPorts)
                SignalBuilder.AssignBase(s, p.SingleWireablePort);
        }

        // Apply Signal assignation, if declared in IPartSignalDefining
        if (template is IPartSignalDefining a2)
        {
            var signalBuilder = new SignalBuilder(instance, template);
            a2.Assembly_Signals(signalBuilder);
        }
        // Apply wiring and connection construction, defined in IPartConnectable
        List<AssemblingConnection> selfDefinedConnection = [];
        List<WiringAction> selfDefinedWirings = [];
        if (template is IPartConnectable a3)
        {
            // User defined connections are created from here
            var connectionBuilder = new ConnectionBuilder(instance, template);
            a3.Assembly_Connections(connectionBuilder);

            selfDefinedConnection = connectionBuilder.Connections;
            selfDefinedWirings = connectionBuilder.Wirings;
        }
        bool hasAnyPortProperty = portsConnectable.Count != 0 || portsWireable.Count != 0;
        bool hasAnySignalProperty= signals.Count != 0;
        if (hasAnyPortProperty || hasAnySignalProperty || template is IPartConnectable || template is IPartSignalDefining)
        {
            var connectivity = new InstanceConnectivity()
            {
                Connectors = portsConnectable,
                Wireables = portsWireable,
                Connections = selfDefinedConnection,
                Wirings = selfDefinedWirings,
                Signals = signals.Select(s => s.Implementation!).ToList(),
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