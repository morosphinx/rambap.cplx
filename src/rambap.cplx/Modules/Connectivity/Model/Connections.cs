using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Templates;
using rambap.cplx.PartProperties;
using System.Runtime.CompilerServices;

namespace rambap.cplx.Modules.Connectivity.Model;

public abstract class SignalPortConnection
{
    public required Pinstance DeclaringInstance { get; init; }
    public required Pinstance LeftPortInstance { get; init; }
    public required Pinstance RigthPortInstance { get; init; }

    public abstract SignalPort LeftPort { get; }
    public abstract SignalPort RightPort { get; }
    public abstract bool IsExclusive { get; }

    public SignalPort GetOtherSide(SignalPort thisSide)
    {
        if (thisSide == LeftPort) return RightPort;
        else if (thisSide == RightPort) return LeftPort;
        else throw new InvalidOperationException();
    }
}

public class StructuralConnection : SignalPortConnection
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
    public override SignalPort LeftPort => ConnectedPort;
    public override SignalPort RightPort => WiringPort;
    public override bool IsExclusive => false;
}

public abstract class AssemblingConnection : SignalPortConnection { }

public class Mate : AssemblingConnection
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

    public override SignalPort LeftPort => LeftConnectedPort;
    public override SignalPort RightPort => RigthConnectedPort;
    public override bool IsExclusive => true;
}

public class Cable : AssemblingConnection
{
    public override SignalPort LeftPort => LeftMate.LeftPort;
    public override SignalPort RightPort => RigthMate.RightPort;
    public override bool IsExclusive => true;

    public Mate LeftMate { get; }
    public Mate RigthMate { get; }

    public Component CableComponent { get; }

    internal Cable(Component cable, Mate leftMate, Mate rigthMate)
    {
        CableComponent = cable;
        LeftMate = leftMate;
        RigthMate = rigthMate;
    }

}