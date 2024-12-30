using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Model;

public interface ISignalingAction
{
    SignalPort LeftPort { get; }
    SignalPort RightPort { get; }
}

public record StructuralConnection : ISignalingAction
{
    public SignalPort ConnectedPort { get; protected init; }
    public SignalPort WirringPort { get; protected init; }

    internal StructuralConnection(ConnectablePort connector, WireablePort wireable)
    {
        ConnectedPort = connector;
        WirringPort = wireable;
    }
    public SignalPort LeftPort => ConnectedPort;
    public SignalPort RightPort => WirringPort;
}


public abstract record ConnectingAction : ISignalingAction
{
    public SignalPort LeftConnectedPort { get; protected init; }
    public SignalPort RigthConnectedPort { get; protected init; }

    public virtual IEnumerable<ConnectingAction> Connections
        => [this];

    public SignalPort LeftPort => LeftConnectedPort;
    public SignalPort RightPort => RigthConnectedPort;
}


public record Mate : ConnectingAction
{
    internal Mate(ConnectablePort connectorA, ConnectablePort connectorB)
    {
        LeftConnectedPort = connectorA;
        RigthConnectedPort = connectorB;
    }
}
