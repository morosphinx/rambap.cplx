using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.PartInterfaces;

public static class ConnectionModel
{
    public abstract record Connection
    {
        public Connector ConnectorA { get; protected init; }
        public Connector ConnectorB { get; protected init; }

        public virtual IEnumerable<Connection> Connections
            => [this];
    }

    public record Mate : Connection
    {
        internal Mate(Connector connectorA, Connector connectorB)
        {
            ConnectorA = connectorA;
            ConnectorB = connectorB;
        }
    }

    public abstract record Wireable : Connection {}

    public record Wire : Wireable
    {
        internal Wire(Connector connectorA, Connector connectorB)
        {
            ConnectorA = connectorA;
            ConnectorB = connectorB;
        }
    }

    private static (Connector,Connector) GetCommonPathOrThrow(IEnumerable<Connection> connections)
    {
        // Check that all items share the same path
        var leftTopMosts = connections.Select(t => t.ConnectorA.TopMostUser());
        var leftConnector = leftTopMosts.Distinct().Single();
        var rigthTopMosts = connections.Select(t => t.ConnectorB.TopMostUser());
        var rigthConnector = rigthTopMosts.Distinct().Single();
        return(leftConnector, rigthConnector);
    }

    public record Twist : Wireable
    {
        public IEnumerable<Wireable> TwistedItems { get; init; }
        public override IEnumerable<Connection> Connections
            => [.. TwistedItems.SelectMany(c => c.Connections)];

        internal Twist(IEnumerable<Wireable> twistedItems)
        {
            (ConnectorA, ConnectorB) = GetCommonPathOrThrow(twistedItems);
            TwistedItems = twistedItems.ToList();
        }
    }

    public record Shield : Wireable
    {
        public enum ShieldingSide { Left, Right, Both, Neither };
        public ShieldingSide Shielding { get; init; } = ShieldingSide.Both;
        public IEnumerable<Wireable> ShieldedItems { get; init; }
        public override IEnumerable<Connection> Connections
            => [.. ShieldedItems.SelectMany(c => c.Connections)];

        internal Shield(IEnumerable<Wireable> shieldedItems)
        {
            (ConnectorA, ConnectorB) = GetCommonPathOrThrow(shieldedItems);
            ShieldedItems = shieldedItems.ToList();
        }
    }
}
