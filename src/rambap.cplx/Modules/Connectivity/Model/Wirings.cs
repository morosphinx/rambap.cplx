using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Model;


public abstract class WiringAction : SignalPortConnection
{
    public abstract WireablePort LeftWiredPort { get; init; }
    public abstract WireablePort RigthWiredPort { get; init; }
    public virtual IEnumerable<WiringAction> Wirings
        => [this];

    internal static (SignalPort, SignalPort) GetCommonPathOrThrow(IEnumerable<WiringAction> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.LeftPort.GetTopMostUser());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.RightPort.GetTopMostUser());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return (leftConnector, rigthConnector);
    }
    public override SignalPort LeftPort => LeftWiredPort;
    public override SignalPort RightPort => RigthWiredPort;
    public override bool IsExclusive => false;

}

public class Wire : WiringAction
{
    public override WireablePort LeftWiredPort { get; init; }
    public override WireablePort RigthWiredPort { get; init; }
    internal Wire(WireablePort wireableA, WireablePort wireableB)
    {
        LeftWiredPort = wireableA;
        RigthWiredPort = wireableB;
        wireableA.AddConnection(this);
        wireableB.AddConnection(this);
    }
}

public abstract class WireableGrouping : WiringAction
{
    public override WireablePort LeftWiredPort { get; init; }
    public override WireablePort RigthWiredPort { get; init; }
    public IEnumerable<WiringAction> GroupedItems { get; init; }
    public override IEnumerable<WiringAction> Wirings
        => [.. GroupedItems.SelectMany(c => c.Wirings)];

    internal WireableGrouping(IEnumerable<WiringAction> groupedItems)
    {
        var commonPath = GetCommonPathOrThrow(groupedItems);
        LeftWiredPort = (WireablePort) commonPath.Item1;
        RigthWiredPort = (WireablePort) commonPath.Item2;
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