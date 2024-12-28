using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Model;

public abstract record Connection
{
    public Connector ConnectorA { get; protected init; }
    public Connector ConnectorB { get; protected init; }

    public virtual IEnumerable<Connection> Connections
        => [this];

    protected static (Connector, Connector) GetCommonPathOrThrow(IEnumerable<Connection> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.ConnectorA.TopMostUser());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.ConnectorB.TopMostUser());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return (leftConnector, rigthConnector);
    }
}
public record Structural : Connection
{
    internal Structural(Connector connectorA, Connector connectorB)
    {
        ConnectorA = connectorA;
        ConnectorB = connectorB;
    }
}

public record Mate : Connection
{
    internal Mate(Connector connectorA, Connector connectorB)
    {
        ConnectorA = connectorA;
        ConnectorB = connectorB;
    }
}

public abstract record Wireable : Connection { }

public record Wire : Wireable
{
    internal Wire(Connector connectorA, Connector connectorB)
    {
        ConnectorA = connectorA;
        ConnectorB = connectorB;
    }
}

public abstract record WireableGrouping : Wireable
{
    public IEnumerable<Wireable> GroupedItems { get; init; }
    public override IEnumerable<Connection> Connections
        => [.. GroupedItems.SelectMany(c => c.Connections)];

    internal WireableGrouping(IEnumerable<Wireable> groupedItems)
    {
        (ConnectorA, ConnectorB) = GetCommonPathOrThrow(groupedItems);
        GroupedItems = groupedItems.ToList();
    }
}

public record Bundle : WireableGrouping
{
    internal Bundle(IEnumerable<Wireable> twistedItems)
        : base(twistedItems) { }
}

public record Twist : WireableGrouping
{
    internal Twist(IEnumerable<Wireable> twistedItems)
        : base(twistedItems) { }
}

public record Shield : WireableGrouping
{
    public enum ShieldingSide { Left, Right, Both, Neither };
    public ShieldingSide Shielding { get; init; } = ShieldingSide.Both;

    internal Shield(IEnumerable<Wireable> shieldedItems)
        : base(shieldedItems) { }
}