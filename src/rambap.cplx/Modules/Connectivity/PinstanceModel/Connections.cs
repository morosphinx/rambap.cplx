using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

public abstract class SignalPortConnection
{
    public required Component DeclaringComponent { get; init; }
    public required Component LeftPortComponent { get; init; }
    public required Component RigthPortComponent { get; init; }

    public abstract Port LeftPort { get; }
    public abstract Port RightPort { get; }

    public Port GetOtherSide(Port thisSide)
    {
        if (thisSide == LeftPort) return RightPort;
        else if (thisSide == RightPort) return LeftPort;
        else throw new InvalidOperationException();
    }
}

public class StructuralConnection : SignalPortConnection
{
    public Port ConnectedPort { get; protected init; }
    public Port WiringPort { get; protected init; }

    internal StructuralConnection(ConnectablePort connector, WireablePort wireable)
    {
        ConnectedPort = connector.LocalImplementation;
        WiringPort = wireable.LocalImplementation;
        ConnectedPort.AddConnection(this);
        WiringPort.AddConnection(this);
    }
    public override Port LeftPort => ConnectedPort;
    public override Port RightPort => WiringPort;
}

public class Mate : SignalPortConnection
{
    internal Mate(PartPort leftConnectedPort, PartPort rigthConnectedPort)
    {
        LeftConnectedPort = leftConnectedPort.LocalImplementation;
        RigthConnectedPort = rigthConnectedPort.LocalImplementation;
        LeftConnectedPort.AddConnection(this);
        RigthConnectedPort.AddConnection(this);
    }

    public Port LeftConnectedPort { get; protected init; }
    public Port RigthConnectedPort { get; protected init; }

    public virtual IEnumerable<Mate> Connections
        => [this];

    public override Port LeftPort => LeftConnectedPort;
    public override Port RightPort => RigthConnectedPort;
}