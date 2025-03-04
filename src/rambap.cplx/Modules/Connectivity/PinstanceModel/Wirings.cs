using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;


public abstract class WiringAction : SignalPortConnection
{
    public abstract Port LeftWiredPort { get; init; }
    public abstract Port RigthWiredPort { get; init; }
    public virtual IEnumerable<WiringAction> Wirings
        => [this];

    internal static (SignalPort, SignalPort) GetCommonPathOrThrow(IEnumerable<WiringAction> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.LeftPort.GetUpperUsage());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.RightPort.GetUpperUsage());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return (leftConnector.ImplementedPort!, rigthConnector.ImplementedPort!);
    }
    public override Port LeftPort => LeftWiredPort;
    public override Port RightPort => RigthWiredPort;
    public override bool IsExclusive => false;

}

public class Wire : WiringAction
{
    public override Port LeftWiredPort { get; init; }
    public override Port RigthWiredPort { get; init; }
    internal Wire(WireablePort wireableA, WireablePort wireableB)
    {
        LeftWiredPort = wireableA.LocalImplementation;
        RigthWiredPort = wireableB.LocalImplementation;
        LeftWiredPort.AddConnection(this);
        RigthWiredPort.AddConnection(this);
    }
}

public abstract class WireableGrouping : WiringAction
{
    public override Port LeftWiredPort { get; init; }
    public override Port RigthWiredPort { get; init; }
    public IEnumerable<WiringAction> GroupedItems { get; init; }
    public override IEnumerable<WiringAction> Wirings
        => [.. GroupedItems.SelectMany(c => c.Wirings)];

    internal WireableGrouping(IEnumerable<WiringAction> groupedItems)
    {
        var commonPath = GetCommonPathOrThrow(groupedItems);
        LeftWiredPort = commonPath.Item1.LocalImplementation;
        RigthWiredPort = commonPath.Item2.LocalImplementation;
        GroupedItems = groupedItems.ToList();
        LeftWiredPort.AddConnection(this);
        RigthWiredPort.AddConnection(this);
    }
}

public class Bundle : WireableGrouping
{
    internal Bundle(IEnumerable<WiringAction> twistedItems)
        : base(twistedItems) { }
}

public class Twist : WireableGrouping
{
    internal Twist(IEnumerable<WiringAction> twistedItems)
        : base(twistedItems) { }
}

public class Shield : WireableGrouping
{
    public enum ShieldingSide { Left, Right, Both, Neither };
    public ShieldingSide Shielding { get; init; } = ShieldingSide.Both;

    internal Shield(IEnumerable<WiringAction> shieldedItems)
        : base(shieldedItems) { }
}