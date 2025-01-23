using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.Model;
using rambap.cplx.Modules.Connectivity.Templates;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartInterfaces;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Contains functions to define a part connectivity <br/>
/// You cannot create this class directly. Instead, implement <see cref="IPartConnectable"/> on a <see cref="Part"/>
/// </summary>
public class ConnectionBuilder
{
    internal List<StructuralConnection> StructuralConnections { get; } = [];
    internal List<AssemblingConnection> Connections { get; } = [];
    internal List<WiringAction> Wirings { get; } = [];


    private void AssertOwnThisCabling(Mate cabling)
    {
        if (!Connections.Contains(cabling))
            throw new InvalidOperationException($"Cabling {cabling} is not owned by this");
    }
    private void AssertOwnThisWiring(WiringAction wiring)
    {
        if (!Wirings.Contains(wiring))
            throw new InvalidOperationException($"Wiring {wiring} is not owned by this");
    }

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
        target.DefineAsAnExpositionOf(sourcePort);
    }


    public void ExposeAs(WireablePort source, WireablePort target)
    {
        ContextPart.AssertIsOwnedBySubComponent(source);
        ContextPart.AssertIsOwner(target);
        target.DefineAsAnExpositionOf(source);
    }
    // Helper methods
    // The second connectable port CANNOT be an ISingleWireablePart,
    // Because it is owned by the part, checked by AssertIsOwner()
    public void ExposeAs(ISingleWireable sourcePart, WireablePort target)
    => ExposeAs(sourcePart.SingleWireablePort, target);

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
        target.DefineAsAnExpositionOf(sources);
    }

    public StructuralConnection StructuralConnection(ConnectablePort source, WireablePort target)
    {
        source.DefineAsHadHoc(); // Force the source to be a simple connecable, throw otherwise
        ContextPart.AssertIsOwnerOrParent(source);
        ContextPart.AssertIsOwnerOrParent(target);
        var connection = new StructuralConnection(source, target)
        {
            LeftPortInstance = source.Owner!.ImplementingInstance,
            RigthPortInstance = target.Owner!.ImplementingInstance,
            DeclaringInstance = ContextInstance
        };
        StructuralConnections.Add(connection);
        return connection;
    }

    /// <summary>
    /// Mate connectorA and connectorB, or represent an implicit signal connection betweenconnectors.<br/>
    /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
    /// </summary>
    /// <param name="connectorA">Left side connector. Must be owed by this part or one of it components</param>
    /// <param name="connectorB">Rigth side connector. Must be owed by this part or one ofits components</param>
    /// <returns>Object representing the created wire</returns>
    public Mate Mate(ISingleMateable mateableA, ISingleMateable mateableB)
    {
        var connectorA = mateableA.SingleMateablePort;
        var connectorB = mateableB.SingleMateablePort;
        ContextPart.AssertIsOwnerOrParent(connectorA);
        ContextPart.AssertIsOwnerOrParent(connectorB);
        // TODO : Test here that both connector are compatible
        var connection = new Mate(connectorA, connectorB)
        {
            LeftPortInstance = connectorA.Owner!.ImplementingInstance,
            RigthPortInstance = connectorB.Owner!.ImplementingInstance,
            DeclaringInstance = ContextInstance
        };
        Connections.Add(connection);
        return connection;
    }


    /// <summary>
    /// Try to use a cable to connect two connector
    /// </summary>
    /// <param name="cablePart"></param>
    /// <param name="connectorA"></param>
    /// <param name="connectorB"></param>
    /// <returns></returns>
    public Cable CableWith(Part cablePart, ISingleMateable connectorA, ISingleMateable connectorB)
    {


        // 1 - Assert with contextInstance that cablePart is a component or subcomponent of this.
        ContextPart.AssertIsASubComponent(cablePart);
        // 2 - Find in the instance of this part
        Pinstance i = cablePart.ImplementingInstance; // TODO . find instqncem bqsed on this context4 childs
        // 3 - Assert that the instance connector is a Cable
        if (i.Connectivity() == null)
            throw new InvalidOperationException($"{cablePart} is not a Connectable part");
        var ci = i.Connectivity()!;
        // 3 a - It must have exactly 2 public port
        //   b - Both must NOT be connected yet (fully connectable, no connected subPort)
        if (ci.Connectors.Count() != 2)
            throw new InvalidOperationException($"{cablePart} must have exacly two connectors");
        if (ci.Connectors.Any(c => !c.CanAddExclusiveConnection))
            throw new InvalidOperationException($"{cablePart} is already connected elsewhere");
        // Check that connector A and  connectorB are also not connected ?
        // This is all to prevent the mates from begin created if any of them may fail

        var cons = ci.Connectors.OrderBy(c => c.Name).ToList();
        // TODO : 
        // 4 - Test both direction for connection validity. If one is valid, good.
        // If none are valid, crash
        // If both are valid, pick the firt one if connector are equivalent (how to prove ?) otherwise throw

        // 5 - If all good, create both matings (call the Mate() function) and return the mates
        // Etablish // with format of wire (WirePort A, WirePort B, Wire Definition) : we are doing something similar
        var leftMate = Mate(connectorA, cons[0]);
        var rigthMate = Mate(cons[1],connectorB);
        Connections.Remove(leftMate);
        Connections.Remove(rigthMate);

        var cableComponent = cablePart.ImplementingInstance.Parent!;
        var cable = new Cable(cableComponent, leftMate, rigthMate)
        {
            LeftPortInstance = leftMate.LeftPortInstance,
            RigthPortInstance = rigthMate.RigthPortInstance,
            DeclaringInstance = ContextInstance
        };
        Connections.Add(cable);
        return cable;
    }




    /// <summary>
    /// Connect connectorA and connectorB using a non descript, signal carrying wire <br/>
    /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
    /// </summary>
    /// <param name="wireableA">Left side connector. Must be owed by this part or one of it components</param>
    /// <param name="wireableB">Rigth side connector. Must be owed by this part or one ofits components</param>
    /// <returns>Object representing the created wire</returns>
    public Wire Wire(ISingleWireable wireableA, ISingleWireable wireableB)
    {
        var wireablePortA = wireableA.SingleWireablePort;
        var wireablePortB = wireableB.SingleWireablePort;
        ContextPart.AssertIsOwnerOrParent(wireablePortA);
        ContextPart.AssertIsOwnerOrParent(wireablePortB);
        var connection = new Wire(wireablePortA, wireablePortB)
        {
            LeftPortInstance = wireablePortA.Owner!.ImplementingInstance,
            RigthPortInstance = wireablePortB.Owner!.ImplementingInstance,
            DeclaringInstance = ContextInstance
        };
        Wirings.Add(connection);
        return connection;
    }

    public Twist Twist(IEnumerable<WiringAction> twistedCablings)
    {
        foreach (var c in twistedCablings)
            AssertOwnThisWiring(c);
        var path = WiringAction.GetCommonPathOrThrow(twistedCablings);
        foreach (var c in twistedCablings)
            Wirings.Remove(c);
        var twist = new Twist(twistedCablings)
        {
            LeftPortInstance = path.Item1.Owner!.ImplementingInstance,
            RigthPortInstance = path.Item2.Owner!.ImplementingInstance,
            DeclaringInstance = ContextInstance
        };
        Wirings.Add(twist);
        return twist;
    }

    public Shield Shield(IEnumerable<WiringAction> twistedCablings)
    {
        foreach (var c in twistedCablings)
            AssertOwnThisWiring(c);
        var path = WiringAction.GetCommonPathOrThrow(twistedCablings);
        foreach (var c in twistedCablings)
            Wirings.Remove(c);
        var twist = new Shield(twistedCablings)
        {
            LeftPortInstance = path.Item1.Owner!.ImplementingInstance,
            RigthPortInstance = path.Item2.Owner!.ImplementingInstance,
            DeclaringInstance = ContextInstance
        };
        Wirings.Add(twist);
        return twist;
    }

    public void AssignTo(Signal signal, SignalPort port)
    {
        port.Signal = signal;
    }

    /// <summary>
    /// The owning part implementing <see cref="IPartConnectable"/> we are currently processing
    /// </summary>
    Part ContextPart { get; }

    /// <summary>
    /// Instance of the part we are currently processing. <br/>
    /// We need this to access calculated subcomponent information. <br/>
    /// Properties on this Pinstance Are note complete
    /// </summary>
    Pinstance ContextInstance{ get; }


    // Internal constructor, prevent usage from outside assembly
    internal ConnectionBuilder(Pinstance instance, Part part)
    {
        ContextPart = part;
        ContextInstance = instance;
    }
}

