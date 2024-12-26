using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.PartInterfaces;

public interface IPartConnectable
{
    public void Assembly_Connections(ConnectionBuilder Do);

    public class ConnectionBuilder
    {
        public abstract record Cabling
        {
            public abstract IEnumerable<Connection> Connections { get; }
        }

        public record Connection: Cabling
        {
            public Connector ConnectorA { get; init; }
            public Connector ConnectorB { get; init; }

            public override IEnumerable<Connection> Connections
                => [this];

            internal Connection(Connector connectorA, Connector connectorB)
            {
                ConnectorA = connectorA;
                ConnectorB = connectorB;
            }
        }
        
        public record Twisting : Cabling
        {
            public IEnumerable<Cabling> TwistedCablings { get; init; }
            public override IEnumerable<Connection> Connections
                => [.. TwistedCablings.SelectMany(c => c.Connections)];

            internal Twisting(IEnumerable<Cabling> twistedCablings)
            { 
                TwistedCablings = twistedCablings.ToList();
            }
        }

        public List<Cabling> Connections { get; } = new();

        private void AssertOwnThisCabling(Cabling cabling)
        {
            if (!Connections.Contains(cabling))
                throw new InvalidOperationException($"Cabling {cabling} is not owned by this");
        }

        //public void Connect(Connector connector1, Connector connector2)
        public void ExposeAs(Connector source, Connector target)
        {
            Context.AssertIsParentOf(source);
            Context.AssertIsOwnerOf(target);
            target.DefineAsAnExpositionOf(source);
        }
        public void ExposeAs(IEnumerable<Connector> sources, Connector target)
        {
            foreach(var c in sources)
                Context.AssertIsParentOf(c);
            Context.AssertIsOwnerOf(target);
            target.DefineAsAnExpositionOf(sources);
        }

        public Cabling Connect(Connector connectorA, Connector connectorB)
        {
            Context.AssertIsParentOf(connectorA);
            Context.AssertIsParentOf(connectorB);
            var connection = new Connection(connectorA, connectorB);
            Connections.Add(connection);
            return connection;
        }

        public Cabling Twist(IEnumerable<Cabling> twistedCablings)
        {
            foreach (var c in twistedCablings)
                AssertOwnThisCabling(c);
            foreach (var c in twistedCablings)
                Connections.Remove(c);
            var twist = new Twisting(twistedCablings);
            Connections.Add(twist);
            return twist;
        }

        public void AssignTo(Signal signal, Connector connector)
        {
            connector.Signal = signal;
        }

        Part Context { get; init; }
        // Internal constructor, prevent usage from outside assembly
        internal ConnectionBuilder(Part context)
        {
            Context = context;
        }
    }
}

