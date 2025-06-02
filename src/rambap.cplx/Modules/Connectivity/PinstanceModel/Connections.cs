using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Templates;
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

public abstract class SignalPortConnection<P1, P2> : SignalPortConnection
    where P1 : Port 
    where P2 : Port
{
    public override Port LeftPort => LeftPort_Typed;
    public override Port RightPort => LeftPort_Typed;
    protected P1 LeftPort_Typed { get; }
    protected P2 RightPort_Typed { get; }
    internal SignalPortConnection(P1 portL, P2 portR)
    {
        LeftPort_Typed = portL;
        RightPort_Typed = portR;
        portL.AddConnection(this);
        portR.AddConnection(this);
    }
}

public class StructuralConnection : SignalPortConnection<CConnectablePort, CWireablePort>
{
    public StructuralConnection(CConnectablePort portL, CWireablePort portR) : base(portL, portR){}
    public CConnectablePort ConnectedPort => LeftPort_Typed;
    public CWireablePort WiringPort => RightPort_Typed;
}

public class Mate : SignalPortConnection<CConnectablePort, CConnectablePort>
{
    public Mate(CConnectablePort portL, CConnectablePort portR) : base(portL, portR){}
    public CConnectablePort LeftConnectedPort => LeftPort_Typed;
    public CConnectablePort RigthConnectedPort => RightPort_Typed;
}

public interface IWiringConnection
{
    Component DeclaringComponent { get; }
    Port LeftPort {  get; }
    Port RightPort { get; }

    private static (PartPort, PartPort) GetCommonPathOrThrow(IEnumerable<PinJunction> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.LeftPort.GetUpperUsage());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.RightPort.GetUpperUsage());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return (leftConnector.ImplementedPort!, rigthConnector.ImplementedPort!);
    }
}

public class PinJunction : SignalPortConnection<CWireablePort, CWireEnd>, IWiringConnection
{
    public PinJunction(CWireablePort portL, CWireEnd portR) : base(portL, portR) {}
    public CWireablePort WireablePort => LeftPort_Typed;
    public CWireEnd WireEndPort => RightPort_Typed;
}

public class WireJunction : SignalPortConnection<CWireEnd, CWireEnd>, IWiringConnection
{
    public WireJunction(CWireEnd portL, CWireEnd portR) : base(portL, portR) {}
    public CWireEnd LeftWireEnd => LeftPort_Typed;
    public CWireEnd RigthWireEnd => RightPort_Typed;
}

public class StructuralWire : SignalPortConnection<CWireEnd, CWireEnd>, IWiringConnection
{
    internal StructuralWire(CWireEnd portL, CWireEnd portR) : base(portL, portR) { }
    public CWireEnd LeftWireEnd => LeftPort_Typed;
    public CWireEnd RigthWireEnd => RightPort_Typed;

    public required WireSpool WireSpool { get; init; } // TBD : This expose a Part during instantiation
}

//public abstract class WireableGrouping : WiringAction
//{
//    public override Port LeftWiredPort { get; init; }
//    public override Port RigthWiredPort { get; init; }
//    public IEnumerable<WiringAction> GroupedItems { get; init; }
//    public override IEnumerable<WiringAction> Wirings
//        => [.. GroupedItems.SelectMany(c => c.Wirings)];

//    internal WireableGrouping(IEnumerable<WiringAction> groupedItems)
//    {
//        var commonPath = GetCommonPathOrThrow(groupedItems);
//        LeftWiredPort = commonPath.Item1.LocalImplementation;
//        RigthWiredPort = commonPath.Item2.LocalImplementation;
//        GroupedItems = groupedItems.ToList();
//        LeftWiredPort.AddConnection(this);
//        RigthWiredPort.AddConnection(this);
//    }
//}

//public class Bundle : WireableGrouping
//{
//    internal Bundle(IEnumerable<WiringAction> twistedItems)
//        : base(twistedItems) { }
//}

//public class Twist : WireableGrouping
//{
//    internal Twist(IEnumerable<WiringAction> twistedItems)
//        : base(twistedItems) { }
//}

//public class Shield : WireableGrouping
//{
//    public enum ShieldingSide { Left, Right, Both, Neither };
//    public ShieldingSide Shielding { get; init; } = ShieldingSide.Both;

//    internal Shield(IEnumerable<WiringAction> shieldedItems)
//        : base(shieldedItems) { }
//}