using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System.Linq;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

/// <summary>
/// A <see cref="Pinstance"/> Port, implementation of a Part's <see cref="PartPort"/>
/// </summary>
/// <remarks>
/// Port are can implement either <see cref="WireablePort"/> or <see cref="ConnectablePort"/>,
/// those differ only by their use in<see cref="PartInterfaces.ConnectivityBuilder"/>
/// </remarks>
public abstract partial class Port
{    
    internal Port(string label, Pinstance owner, bool isPublic)
    {
        Label = label;
        Owner = owner;
        IsPublic = isPublic;
    }

    /// <summary>
    /// Identifier of this port, visible from outside this interface
    /// </summary>
    public string Label { get; internal set ; }

    /// <summary>
    /// Pinstance owning this port
    /// </summary>
    public Pinstance Owner { get; }

    /// <summary>
    /// True is this port is physicaly visible from outside of the Pinstance
    /// </summary>
    public bool IsPublic { get; internal set; }


    // Signal assignation

    private PSignal? assignedSignal;
    public PSignal? AssignedSignal
    {
        get => assignedSignal;
        internal set
        {
            if (assignedSignal != null)
                throw new InvalidOperationException("A signal is already assigned to this port");
            assignedSignal = value;
        }
    }

    /// <summary>
    /// The Part's <see cref="PartPort"/> this class <see cref="Port"/> implement <br/>
    /// Multiple <see cref="Port"/>s may implement the same <see cref="PartPort"/>
    /// </summary>
    internal PartPort? ImplementedPort { get; private set; }

    internal void Implement(PartPort signalPort)
    {
        if (ImplementedPort != null)
            throw new InvalidOperationException($"Pinstance model port {this} already implement a Part signalPort");
        ImplementedPort = signalPort;
        signalPort.Implementations.Push(this);
    }

    // Construction Definition
    internal PortDefinition Definition { get ; private set; } = new PortDefinition_AdHoc();
    internal bool HasbeenDefined => Definition is not PortDefinition_AdHoc;

    // Usage definition
    internal PortUsage? Usage { get; private set; }
    internal bool HasBeenUseDefined => Usage != null;

    // Definition actions
    private void CheckDefinitionValidity(IEnumerable<Port> sources)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Port has already been defined");
        if (sources.Contains(this)) throw new InvalidOperationException("Can't expose self on self");
        foreach (var source in sources)
        {
            if (source.HasBeenUseDefined) throw new InvalidOperationException($"Port {source} has already been used in another definition ({source.Usage!.User})");
            if (source.GetType() != this.GetType()) throw new InvalidOperationException($"Cannot define port from another type");
        }
    }
    internal void DefineAsAnExpositionOf(Port source)
    {
        // Checks
        CheckDefinitionValidity([source]);
        // Define and mark used ports
        Definition = new PortDefinition_Exposed() { ExposedPort = source };
        source.Usage = new PortUsage_ExposedAs() { ExposedAs = this };
    }
    protected abstract bool CanBeCombined { get; }
    internal void DefineAsAnExpositionOf(IEnumerable<Port> sources)
    {
        // Checks
        if (!CanBeCombined) throw new InvalidOperationException($"Cannot combine multiples ports into a {this.GetType()}");
        CheckDefinitionValidity(sources);
        // Define and mark used ports
        Definition = new PortDefinition_Combined() { CombinedPorts = [.. sources] };
        foreach (var source in sources)
        {
            source.Usage = new PortUsage_CombinedInto() { CombinedInto = this };
        }
    }

    // Connections
    private List<SignalPortConnection> connections { get; } = new();
    internal IEnumerable<SignalPortConnection> Connections => connections;

    protected abstract void AssertCanAddConnection(SignalPortConnection connection);
    public void AddConnection(SignalPortConnection connection)
    {
        AssertCanAddConnection(connection);
        connections.Add(connection);
    }
}

public class CConnectablePort : Port
{
    public CConnectablePort(string label, Pinstance owner, bool isPublic) : base(label, owner, isPublic){}
    protected override bool CanBeCombined => true;
    protected override void AssertCanAddConnection(SignalPortConnection connection)
    {
        // Connectable port can have :
        // A single StructuralConnection => to pin wireable face
        // A single matting, if and only if they are not combined (partial matting are not supported)
        // This check apply to the entire port definition hierarchy
        switch (connection)
        {
            case Mate m:
                {
                    if (IsExpositionColumnUsageCombined())
                        throw new InvalidOperationException($"Cannot add {nameof(Mate)} to this {nameof(ConnectablePort)}, "
                            + "the port is combined with other ports. It cannot be mated independently");
                    if (ExpositionColumnConnection().OfType<Mate>().Any())
                        throw new InvalidOperationException($"Cannot add {nameof(Mate)} to this {nameof(ConnectablePort)}, "
                            + "there is already a defined structural connection");
                    break;
                }
            case StructuralConnection s:
                {
                    if (ExpositionColumnConnection().OfType<StructuralConnection>().Any())
                        throw new InvalidOperationException($"Cannot add {nameof(StructuralConnection)} to this {nameof(ConnectablePort)}, "
                            + "there is already a defined structural connection");
                    break;
                }
            default: throw new InvalidOperationException($"{this.GetType()} cannot use connection type {connection.GetType()}");
        }
    }
}

public class CWireablePort : Port
{
    public CWireablePort(string label, Pinstance owner, bool isPublic) : base(label, owner, isPublic) { }
    protected override bool CanBeCombined => false;
    protected override void AssertCanAddConnection(SignalPortConnection connection)
    {
        // Wireable port can have :
        // Any number of PinJunction
        // A single StructuralConnection => to pin mating face
        // This check apply to the entire port definition hierarchy
        switch (connection)
        {
            case PinJunction m: // Always accepted
                break;
            case StructuralConnection s:
                {
                    if (ExpositionColumnConnection().OfType<StructuralConnection>().Any())
                        throw new InvalidOperationException($"Cannot add {nameof(StructuralConnection)} to this {nameof(WireablePort)}, "
                            + "there is already a defined structural connection");
                    break;
                }
            default: throw new InvalidOperationException($"{this.GetType()} cannot use connection type {connection.GetType()}");
        }
    }
}
public class CWireEnd : Port
{
    public CWireEnd(string label, Pinstance owner, bool isPublic) : base(label, owner, isPublic) { }
    protected override bool CanBeCombined => false;
    protected override void AssertCanAddConnection(SignalPortConnection connection)
    {
        // Wire end may have EITHER :
        // A single Pin junction => pin connection
        // Multiple Wire junction => splice, etc
        // This is required to prevent wire from propagating a pin to pin constraint, ex :
        //      PinJunction(PinA,WireEndA) + WireJunction(WireEndA,WireEndB) + PinJunction(PinB,WireEndB)
        // This check apply to the entire port definition hierarchy
        switch (connection)
        {
            case PinJunction m:
                {
                    if (ExpositionColumnConnection().Any())
                        throw new InvalidOperationException($"Cannot add {nameof(PinJunction)} to this {nameof(WireEnd)}, "
                            +"it is already connected somewhere else");
                    break;
                }
            case WireJunction wire:
                {
                    if(ExpositionColumnConnection().OfType<PinJunction>().Any())
                        throw new InvalidOperationException($"Cannot add {nameof(WireJunction)} to this {nameof(WireEnd)}, "
                            + "it is already part of a pin junction");
                    break;
                }
            default: throw new InvalidOperationException($"{nameof(WireEnd)} cannot use connection type {connection.GetType()}");
        }
    }
}


internal abstract class PortDefinition
{
    public abstract IEnumerable<Port> SubPorts { get; }
}
internal class PortDefinition_AdHoc : PortDefinition // Single pin / signal
{
    public override IEnumerable<Port> SubPorts => [];
}
internal class PortDefinition_Exposed : PortDefinition // Exposed connector
{
    public override IEnumerable<Port> SubPorts => ExposedPort.Definition?.SubPorts ?? [];
    public required Port ExposedPort { get; init; }
}
internal class PortDefinition_Combined : PortDefinition // Exposed / grouped connector
{
    public override IEnumerable<Port> SubPorts => CombinedPorts;
    public required List<Port> CombinedPorts { get; init; } // Connector in order, their names is used as the label
}

internal abstract class PortUsage
{
    public abstract Port User { get; }
}
internal class PortUsage_ExposedAs : PortUsage
{
    public required Port ExposedAs { get; init; }
    public override Port User => ExposedAs;
}
internal class PortUsage_CombinedInto : PortUsage
{
    public required Port CombinedInto { get; init; }
    public override Port User => CombinedInto;
}