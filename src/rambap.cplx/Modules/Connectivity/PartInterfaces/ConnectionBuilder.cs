using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Modules.Connectivity.Templates;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartInterfaces;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public abstract class ConnectivityBuilder
{
    /// <summary>
    /// The owning part implementing <see cref="IPartConnectable"/> we are currently processing
    /// </summary>
    internal Part ContextPart => ContextComponent.Template;

    /// <summary>
    /// Instance of the part we are currently processing. <br/>
    /// We need this to access calculated subcomponent information. <br/>
    /// Properties on this Pinstance Are note complete
    /// </summary>
    internal Pinstance ContextInstance => ContextComponent.Instance;

    internal Component ContextComponent { get; }

    // Internal constructor, prevent usage from outside assembly
    internal ConnectivityBuilder(Component contextComponent)
    {
        ContextComponent = contextComponent;
    }
}


/// <summary>
/// Contains functions to define a part ports <br/>
/// You cannot create this class directly. Instead, implement <see cref="IPartConnectable"/> on a <see cref="Part"/>
/// </summary>
public class PortBuilder : ConnectivityBuilder
{
    internal PortBuilder(Component contextComponent) : base(contextComponent) { }

    /// <summary>
    /// Define a connector as an exposition of another. Typical use case : <br/>
    /// when the connector of a subcomponent is visible on the face of this part. <br/>
    /// The connectors source and target will refer to the same physical connection point.<br/>
    /// </summary>
    /// <param name="source">Connector of a subcomponent. Need to be public</param>
    /// <param name="target">Connector of this part. Need to be public</param>
    public void ExposeAs(ISingleMateable source, ConnectablePort target)
    {
        var sourcePort = source.SingleMateablePort;
        ContextPart.AssertIsOwnedBySubComponent(sourcePort);
        ContextPart.AssertIsOwner(target);
        target.LocalImplementation.DefineAsAnExpositionOf(sourcePort.LocalImplementation);
    }
    /// <summary>
    /// Same as <see cref="ExposeAs(ConnectablePort, ConnectablePort)"/>, but instead multiplesubcomponent connectors
    /// are combined into a single connection points
    /// </summary>
    /// <param name="sources">Connectors of subcomponents. Need to be public</param>
    /// <param name="target">Connector of this part. Need to be public</param>
    public void ExposeAs(IEnumerable<ConnectablePort> sources, ConnectablePort target)
    {
        foreach (var c in sources)
            ContextPart.AssertIsOwnedBySubComponent(c);
        ContextPart.AssertIsOwner(target);
        target.LocalImplementation.DefineAsAnExpositionOf(sources.Select(s => s.LocalImplementation));
    }


    public void ExposeAs(WireablePort source, WireablePort target)
    {
        ContextPart.AssertIsOwnedBySubComponent(source);
        ContextPart.AssertIsOwner(target);
        target.LocalImplementation.DefineAsAnExpositionOf(source.LocalImplementation);
    }
    // Helper methods
    // The second connectable port CANNOT be an ISingleWireablePart,
    // Because it must be a property owned by the part, checked by AssertIsOwner()
    public void ExposeAs(ISingleWireable sourcePart, WireablePort target)
        => ExposeAs(sourcePart.SingleWireablePort, target);
}


/// <summary>
/// Contains functions to define a part wonnection and wiring <br/>
/// You cannot create this class directly. Instead, implement <see cref="IPartConnectable"/> on a <see cref="Part"/>
/// </summary>
public class ConnectionBuilder : ConnectivityBuilder
{
    internal ConnectionBuilder(Component contextComponent) : base(contextComponent) { }

    internal List<StructuralConnection> StructuralConnections { get; } = [];
    internal List<Mate> Connections { get; } = [];
    internal List<WiringConnection> Wirings { get; } = [];

    private void AssertOwnThisMate(Mate mate)
    {
        if (!Connections.Contains(mate))
            throw new InvalidOperationException($"Mate {mate} is not owned by this component");
    }
    private void AssertOwnThisWire(WirePart wire)
    {
        if (!ContextComponent.SubComponents.Contains(wire.ImplementingComponent))
            throw new InvalidOperationException($"Wire {wire} is not owned by this component");
    }

    public StructuralConnection StructuralConnection(ConnectablePort source, WireablePort target)
    {
        if (source.LocalImplementation.HasbeenDefined)
            throw new InvalidOperationException("Port cannot have a structural definition if its exposed");
        if (target.LocalImplementation.HasbeenDefined)
            throw new InvalidOperationException("Port cannot have a structural definition if its exposed");
        ContextPart.AssertIsOwnerOrParent(source);
        ContextPart.AssertIsOwnerOrParent(target);
        var connection = new StructuralConnection(source, target)
        {
            LeftPortComponent = source.Owner!.ImplementingComponent!,
            RigthPortComponent = target.Owner!.ImplementingComponent!,
            DeclaringComponent = ContextInstance.Parent
        };
        StructuralConnections.Add(connection);
        return connection;
    }

    /// <summary>
    /// Mate connectorA and connectorB, or represent an implicit signal connection betweenconnectors.<br/>
    /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
    /// </summary>
    /// <param name="connectorA">Left side connector. Must be owed by this part or one of its components</param>
    /// <param name="connectorB">Rigth side connector. Must be owed by this part or one of its components</param>
    public void Mate(ISingleMateable mateableA, ISingleMateable mateableB)
    {
        var connectorA = mateableA.SingleMateablePort;
        var connectorB = mateableB.SingleMateablePort;
        ContextPart.AssertIsOwnerOrParent(connectorA);
        ContextPart.AssertIsOwnerOrParent(connectorB);
        // TODO : Test here that both connector are compatible
        var connection = new Mate(connectorA, connectorB)
        {
            LeftPortComponent = connectorA.Owner!.ImplementingComponent!,
            RigthPortComponent = connectorB.Owner!.ImplementingComponent!,
            DeclaringComponent = ContextInstance.Parent
        };
        Connections.Add(connection);
    }


    /// <summary>
    /// Try to use a cable to connect two connector
    /// </summary>
    /// <param name="cablePart"></param>
    /// <param name="connectorA"></param>
    /// <param name="connectorB"></param>
    /// <returns></returns>
    public void CableWith(Part cablePart, ISingleMateable connectorA, ISingleMateable connectorB)
    {
        // 1 - Assert with contextInstance that cablePart is a component or subcomponent of this.
        ContextPart.AssertIsASubComponent(cablePart);
        // 2 - Find in the instance of this part
        Pinstance i = cablePart.ImplementingComponent!.Instance;
        // 3 - Assert that the instance connector is a Cable
        if (i.Connectivity() == null)
            throw new InvalidOperationException($"{cablePart} is not a Connectable part");
        var ci = i.Connectivity()!;
        // 3 a - It must have exactly 2 public port
        //   b - Both must NOT be connected yet (fully connectable, no connected subPort)
        if (ci.Connectors.Count() != 2)
            throw new InvalidOperationException($"{cablePart} must have exacly two connectors");

        // TODO Check here that both ports are ready for a new mating connection
        // This is all to prevent the mates from begin created if one of them may fail

        var cons = ci.Connectors.OrderBy(c => c.Label).ToList();
        var cableLeftPort = (ISingleMateable) cons[0].ImplementedPort!;
        var cableRigthPort = (ISingleMateable) cons[1].ImplementedPort!;
        // TODO : 
        // 4 - Test both direction for connection validity. If one is valid, good.
        // If none are valid, crash
        // If both are valid, pick the firt one if connector are equivalent (how to prove ?) otherwise throw

        // 5 - If all good, create both matings (call the Mate() function) and return the mates
        // Etablish // with format of wire (WirePort A, WirePort B, Wire Definition) : we are doing something similar
        Mate(connectorA, cableLeftPort);
        Mate(cableRigthPort, connectorB);
    }



    private PlaceholderWireSpool? PlaceholderWireSpool;
    private PlaceholderWireSpool GetCreatePlaceholderWireSpool()
    {
        if(PlaceholderWireSpool == null)
        {
            PlaceholderWireSpool = new PlaceholderWireSpool();
            var contextComponent = this.ContextInstance.Parent;
            contextComponent.AddConceptPart(PlaceholderWireSpool);
        }
        return PlaceholderWireSpool!;
    }


    /// <summary>
    /// Connect connectorA and connectorB using a non descript, signal carrying wire <br/>
    /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
    /// </summary>
    /// <param name="wireableA">Left side connector. Must be owed by this part or one of it components</param>
    /// <param name="wireableB">Rigth side connector. Must be owed by this part or one ofits components</param>
    /// <returns>Object representing the created wire</returns>
    public WirePart Wire(ISingleWireable wireableA, ISingleWireable wireableB)
        => Wire(wireableA, wireableB, GetCreatePlaceholderWireSpool(), 100);

    public WirePart Wire(ISingleWireable wireableA, ISingleWireable wireableB, WireSpool wireSpool, double length)
    {
        ContextPart.AssertIsOwnerOrParent(wireableA.SingleWireablePort);
        ContextPart.AssertIsOwnerOrParent(wireableB.SingleWireablePort);
        ContextPart.AssertIsASubComponent(wireSpool);

        WirePart createdWire = new WirePart()
        {
            Length = length,
            Origin = wireSpool
        };
        var contextComponent = this.ContextInstance.Parent;
        contextComponent.AddConceptPart(createdWire);

        Wire(wireableA, createdWire.LeftPort);
        Wire(createdWire.RightPort, wireableB);

        return createdWire;
    }

    public void Wire(WireEnd wireEnd, ISingleWireable wireable)
        => Wire(wireable, wireEnd);
    public void Wire(ISingleWireable wireable, WireEnd wireEnd)
    {
        ContextPart.AssertIsOwnerOrParent(wireable.SingleWireablePort);
        ContextPart.AssertIsOwnerOrParent(wireEnd);

        CWireablePort pa = (CWireablePort)wireable.SingleWireablePort.LocalImplementation;
        CWireEnd pb = (CWireEnd)wireEnd.LocalImplementation;

        var junction = new PinJunction()
        {
            WireablePort = pa,
            LeftPortComponent = pa.Owner.Parent,
            WireEndPort = pb,
            RigthPortComponent = pb.Owner.Parent,
            DeclaringComponent = ContextInstance.Parent,
        };
        Wirings.Add(junction);
    }

    public void Wire(WireEnd wireEndA, WireEnd wireEndB)
    {

        CWireEnd pa = (CWireEnd)wireEndA.LocalImplementation;
        CWireEnd pb = (CWireEnd)wireEndB.LocalImplementation;

        var junction = new WireJunction()
        {
            LeftWireEnd = pa,
            LeftPortComponent = pa.Owner.Parent,
            RigthWireEnd = pb,
            RigthPortComponent = pb.Owner.Parent,
            DeclaringComponent = ContextInstance.Parent,
        };
        Wirings.Add(junction);
    }


    public void Wire(Signal signalA, Signal signalB)
        => Wire(signalA, signalB, GetCreatePlaceholderWireSpool(), 100);

    public List<WirePart> Wire(Signal signalA, Signal signalB, WireSpool wireSpool, double length)
    {
        ContextPart.AssertIsOwnerOrParent(signalA);
        ContextPart.AssertIsOwnerOrParent(signalB);

        List<WirePart> createdWires = [];

        // All port assigned to the signals must be wireable or wireEnd
        var signalPortsA = signalA.Assignations;
        var signalPortsB = signalB.Assignations;
        // TODO : behavior when wiring signal defined targetting / from other subparts ?
        // Only link publicly visible ports ?
        foreach (var portA in signalPortsA.OfType<WireablePort>())
        {
            foreach (var portB in signalPortsB.OfType<WireablePort>())
            {
                // This is the only case where we need to create new wires
                var newWire = Wire(portA, portB, wireSpool, length);
                createdWires.Add(newWire);
            }
            foreach (var wireEndB in signalPortsB.OfType<WireEnd>())
            {
                Wire(portA, wireEndB);
            }
        }
        foreach (var wireEndA in signalPortsA.OfType<WireEnd>())
        {
            foreach (var PortB in signalPortsB.OfType<WireablePort>())
            {
                Wire(wireEndA, PortB);
            }
            foreach (var wireEndB in signalPortsB.OfType<WireEnd>())
            {
                Wire(wireEndA, wireEndB);
            }
        }
        return createdWires;
    }

    //public void Twist(IEnumerable<WirePart> twistedCablings)
    //{
    //    foreach (var c in twistedCablings)
    //        AssertOwnThisWire(c);
    //    var path = WiringSet.GetCommonPathOrThrow(twistedCablings);
    //    
    //    var twist = new Twist(twistedCablings)
    //    {
    //        LeftPortComponent = path.Item1.Owner!.ImplementingComponent!,
    //        RigthPortComponent = path.Item2.Owner!.ImplementingComponent!,
    //        DeclaringComponent = ContextInstance.Parent
    //    };
    //    Wirings.Add(twist);
    //    return twist;
    //}

    //public Shield Shield(IEnumerable<WiringSet> twistedCablings)
    //{
    //    foreach (var c in twistedCablings)
    //        AssertOwnThisWiring(c);
    //    var path = WiringSet.GetCommonPathOrThrow(twistedCablings);
    //    foreach (var c in twistedCablings)
    //        Wirings.Remove(c);
    //    var twist = new Shield(twistedCablings)
    //    {
    //        LeftPortComponent = path.Item1.Owner!.ImplementingComponent!,
    //        RigthPortComponent = path.Item2.Owner!.ImplementingComponent!,
    //        DeclaringComponent = ContextInstance.Parent
    //    };
    //    Wirings.Add(twist);
    //    return twist;
    //}

    public void AssignTo(Signal signal, PartPort port)
    {
        throw new NotImplementedException();
        // port.Signal = signal;
    }
}

