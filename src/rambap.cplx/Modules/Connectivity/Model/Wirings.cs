using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Model;


public abstract record WiringAction : ISignalingAction
{
    public WireablePort LeftWiredPort { get; protected init; }
    public WireablePort RigthWiredPort { get; protected init; }

    public virtual IEnumerable<WiringAction> Wirings
        => [this];

    protected static (SignalPort, SignalPort) GetCommonPathOrThrow(IEnumerable<WiringAction> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.LeftWiredPort.TopMostUser());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.RigthWiredPort.TopMostUser());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return (leftConnector, rigthConnector);
    }
    public SignalPort LeftPort => LeftWiredPort;
    public SignalPort RightPort => RigthWiredPort;
}

public record Wire : WiringAction
{
    internal Wire(WireablePort wireableA, WireablePort wireableB)
    {
        LeftWiredPort = wireableA;
        RigthWiredPort = wireableB;
    }
}

public abstract record WireableGrouping : WiringAction
{
    public IEnumerable<WiringAction> GroupedItems { get; init; }
    public override IEnumerable<WiringAction> Wirings
        => [.. GroupedItems.SelectMany(c => c.Wirings)];

    internal WireableGrouping(IEnumerable<WiringAction> groupedItems)
    {
        var commonPath = GetCommonPathOrThrow(groupedItems);
        LeftWiredPort = (WireablePort) commonPath.Item1;
        RigthWiredPort = (WireablePort)commonPath.Item2;
        GroupedItems = groupedItems.ToList();
    }
}

public record Bundle : WireableGrouping
{
    internal Bundle(IEnumerable<WiringAction> twistedItems)
        : base(twistedItems) { }
}

public record Twist : WireableGrouping
{
    internal Twist(IEnumerable<WiringAction> twistedItems)
        : base(twistedItems) { }
}

public record Shield : WireableGrouping
{
    public enum ShieldingSide { Left, Right, Both, Neither };
    public ShieldingSide Shielding { get; init; } = ShieldingSide.Both;

    internal Shield(IEnumerable<WiringAction> shieldedItems)
        : base(shieldedItems) { }
}