using rambap.cplx.Modules.Connectivity.Templates;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Model;

public interface ISignalPortConnection
{
    SignalPort LeftPort { get; }
    SignalPort RightPort { get; }
    bool IsExclusive { get; }

    public SignalPort GetOtherSide(SignalPort thisSide)
    {
        if (thisSide == LeftPort) return RightPort;
        else if (thisSide == RightPort) return LeftPort;
        else throw new InvalidOperationException();
    }
}

public record StructuralConnection : ISignalPortConnection
{
    public SignalPort ConnectedPort { get; protected init; }
    public SignalPort WiringPort { get; protected init; }

    internal StructuralConnection(ConnectablePort connector, WireablePort wireable)
    {
        ConnectedPort = connector;
        WiringPort = wireable;
        connector.AddConnection(this);
        wireable.AddConnection(this);
    }
    public SignalPort LeftPort => ConnectedPort;
    public SignalPort RightPort => WiringPort;
    public bool IsExclusive => false;
}


public record Mate : ISignalPortConnection
{
    internal Mate(SignalPort leftConnectedPort, SignalPort rigthConnectedPort)
    {
        LeftConnectedPort = leftConnectedPort;
        RigthConnectedPort = rigthConnectedPort;
        LeftConnectedPort.AddConnection(this);
        RigthConnectedPort.AddConnection(this);
    }

    public SignalPort LeftConnectedPort { get; protected init; }
    public SignalPort RigthConnectedPort { get; protected init; }

    public virtual IEnumerable<Mate> Connections
        => [this];

    public SignalPort LeftPort => LeftConnectedPort;
    public SignalPort RightPort => RigthConnectedPort;
    public bool IsExclusive => true;
}