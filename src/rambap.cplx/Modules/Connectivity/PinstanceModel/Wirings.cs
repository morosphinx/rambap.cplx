using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;


public abstract class WiringConnection : SignalPortConnection
{
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

public class PinJunction : WiringConnection
{
    public required CWireablePort WireablePort { get; init; }
    public required CWireEnd WireEndPort { get; init; }

    public override Port LeftPort => WireablePort;
    public override Port RightPort => WireEndPort;
}

public class WireJunction : WiringConnection
{
    public required CWireEnd LeftWireEnd { get; init; }
    public required CWireEnd RigthWireEnd { get; init; }

    public override Port LeftPort => LeftWireEnd;
    public override Port RightPort => RigthWireEnd;
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