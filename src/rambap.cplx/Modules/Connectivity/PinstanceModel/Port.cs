using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

/// <summary>
/// A <see cref="Pinstance"/> Port, implementation of a Part's <see cref="SignalPort"/>
/// </summary>
/// <remarks>
/// Port are can implement either <see cref="WireablePort"/> or <see cref="ConnectablePort"/>,
/// those differ only by their use in<see cref="PartInterfaces.ConnectivityBuilder"/>
/// </remarks>
public partial class Port
{    
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


    internal Port(string label, Pinstance owner, bool isPublic)
    {
        Label = label;
        Owner = owner;
        IsPublic = isPublic;
    }

    // TBD :
    public Signal? AssignedSignal { get; init; }

    /// <summary>
    /// The Part's <see cref="SignalPort"/> this class <see cref="Port"/> implement <br/>
    /// Multiple <see cref="Port"/>s may implement the same <see cref="SignalPort"/>
    /// </summary>
    internal SignalPort? ImplementedPort { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    internal void Implement(SignalPort signalPort)
    {
        if (ImplementedPort != null)
            throw new InvalidOperationException($"Pinstance model port {this} already implement a Part signalPort");
        ImplementedPort = signalPort;
        signalPort.Implementations.Push(this);
    }

    // Construction Definition

    private PortDefinition definition = new PortDefinition_AdHoc();
    internal PortDefinition Definition
    {
        get => definition;
        private set
        {
            if (HasbeenDefined)
                throw new InvalidOperationException("Port already has a definition");
            definition = value;
        }
    }
    internal bool HasbeenDefined => definition is not PortDefinition_AdHoc;

    internal void DefineAsAnExpositionOf(Port source)
    {
        if (source == this) throw new InvalidOperationException("Can't expose self on self");
        if (HasbeenDefined) throw new InvalidOperationException($"Port has already been defined");
        if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        Definition = new PortDefinition_Exposed() { ExposedPort = source };
        source.Usage = new PortUsage_ExposedAs() { ExposedAs = this };
    }

    internal void DefineAsAnExpositionOf(IEnumerable<Port> sources)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Port has already been defined");
        foreach (var source in sources)
        {
            if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        }
        Definition = new PortDefinition_Combined() { CombinedPorts = [.. sources] };
        foreach (var source in sources)
        {
            source.Usage = new PortUsage_CombinedInto() { CombinedInto = this };
        }
    }

    // Usage definition

    internal PortUsage? Usage { get; private set; }
    internal bool HasBeenUseDefined => Usage != null;



    // Connections

    private SignalPortConnection? ExclusiveConnection { get; set; }
    private List<SignalPortConnection> NonExclusiveConnections { get; } = new();

    public void AddConnection(SignalPortConnection connection)
    {
        if (connection.IsExclusive)
        {
            if (CanAddExclusiveConnection)
                ExclusiveConnection = connection;
            else
                throw new InvalidOperationException("This connectable port is already connected");
        }
        else
        {
            NonExclusiveConnections.Add(connection);
        }
    }

    internal IEnumerable<SignalPortConnection> Connections
        => ExclusiveConnection != null
            ? [ExclusiveConnection, .. NonExclusiveConnections]
            : NonExclusiveConnections;

    internal bool CanAddExclusiveConnection =>
        !IsThisExclusivelyConnected &&
        !IsAnyChildsExclusivelyConnected() &&
        !IsAnyParentExclusivelyConnected();

    internal bool IsThisExclusivelyConnected => ExclusiveConnection != null;
    internal bool IsAnyChildsExclusivelyConnected()
    {
        var subdef = Definition switch
        {
            PortDefinition_AdHoc d => [],
            PortDefinition_Exposed d => [d.ExposedPort],
            PortDefinition_Combined d => d.CombinedPorts,
            null => [],
            _ => throw new NotImplementedException(),
        };
        return subdef.Any(d => d.IsThisExclusivelyConnected || d.IsAnyChildsExclusivelyConnected());
    }
    internal bool IsAnyParentExclusivelyConnected()
    {
        var parent = Usage switch
        {
            PortUsage_ExposedAs u => u.ExposedAs,
            PortUsage_CombinedInto u => u.CombinedInto,
            null => null,
            _ => throw new NotImplementedException(),
        };
        if (parent != null)
            return parent.IsThisExclusivelyConnected || parent.IsAnyParentExclusivelyConnected();
        else return false;
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