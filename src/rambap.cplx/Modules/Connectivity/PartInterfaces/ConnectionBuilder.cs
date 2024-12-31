using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.Model;

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
    internal List<Mate> Mates { get; } = [];
    internal List<WiringAction> Wirings { get; } = [];


    private void AssertOwnThisCabling(Mate cabling)
    {
        if (!Mates.Contains(cabling))
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
    public void ExposeAs(ConnectablePort source, ConnectablePort target)
    {
        Context.AssertIsOwnedBySubComponent(source);
        Context.AssertIsOwner(target);
        target.DefineAsAnExpositionOf(source);
    }
    public void ExposeAs(WireablePort source, WireablePort target)
    {
        Context.AssertIsOwnedBySubComponent(source);
        Context.AssertIsOwner(target);
        target.DefineAsAnExpositionOf(source);
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
            Context.AssertIsOwnedBySubComponent(c);
        Context.AssertIsOwner(target);
        target.DefineAsAnExpositionOf(sources);
    }

    public StructuralConnection StructuralConnection(ConnectablePort source, WireablePort target)
    {
        source.DefineAsHadHoc(); // Force the source to be a simple connecable, throw otherwise
        Context.AssertIsOwnerOrParent(source);
        Context.AssertIsOwnerOrParent(target);
        var connection = new StructuralConnection(source, target);
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
    public Mate Mate(ConnectablePort connectorA, ConnectablePort connectorB)
    {
        Context.AssertIsOwnerOrParent(connectorA);
        Context.AssertIsOwnerOrParent(connectorB);
        // TODO : Test here that both connector are compatible
        var connection = new Mate(connectorA, connectorB);
        Mates.Add(connection);
        return connection;
    }

    /// <summary>
    /// Connect connectorA and connectorB using a non descript, signal carrying wire <br/>
    /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
    /// </summary>
    /// <param name="wireableA">Left side connector. Must be owed by this part or one of it components</param>
    /// <param name="wireableB">Rigth side connector. Must be owed by this part or one ofits components</param>
    /// <returns>Object representing the created wire</returns>
    public Wire Wire(WireablePort wireableA, WireablePort wireableB)
    {
        Context.AssertIsOwnerOrParent(wireableA);
        Context.AssertIsOwnerOrParent(wireableB);
        var connection = new Wire(wireableA, wireableB);
        Wirings.Add(connection);
        return connection;
    }

    public Twist Twist(IEnumerable<WiringAction> twistedCablings)
    {
        foreach (var c in twistedCablings)
            AssertOwnThisWiring(c);
        foreach (var c in twistedCablings)
            Wirings.Remove(c);
        var twist = new Twist(twistedCablings);
        Wirings.Add(twist);
        return twist;
    }

    public Shield Shield(IEnumerable<WiringAction> twistedCablings)
    {
        foreach (var c in twistedCablings)
            AssertOwnThisWiring(c);
        foreach (var c in twistedCablings)
            Wirings.Remove(c);
        var twist = new Shield(twistedCablings);
        Wirings.Add(twist);
        return twist;
    }

    public void AssignTo(Signal signal, SignalPort port)
    {
        port.Signal = signal;
    }

    /// <summary>
    /// The owning part implementing <see cref="IPartConnectable"/> we are currentlyprocessing
    /// </summary>
    Part Context { get; init; }
    // Internal constructor, prevent usage from outside assembly
    internal ConnectionBuilder(Part context)
    {
        Context = context;
    }
}

