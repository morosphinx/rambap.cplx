using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.PartInterfaces;

/// <summary>
/// Interface implemented by <see cref="Part"/>s that contains electrical wiring and cabling. <br/>
/// Define first a number of <see cref="Connector"/> on the Part and its component, <br/>
/// and define connection between those in the <see cref="Assembly_Connections"/> method
/// </summary>
public interface IPartConnectable
{
    /// <summary>
    /// Define the connection and exposition of <see cref="Connector"/>s of the Part
    /// </summary>
    /// <param name="Do">A <see cref="ConnectionBuilder"/> with the method difining wiirng</param>
    public void Assembly_Connections(ConnectionBuilder Do);

    /// <summary>
    /// Contains functions to define a part connectivity <br/>
    /// You cannot create this class directly. Instead, implement <see cref="IPartConnectable"/> on a <see cref="Part"/>
    /// </summary>
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

        /// <summary>
        /// Define a connector as an exposition of another. Typical use case : <br/>
        /// when the connector of a subcomponent is visible on the face of this part. <br/>
        /// The connectors source and target will refer to the same physical connection point. <br/>
        /// </summary>
        /// <param name="source">Connector of a subcomponent. Need to be public</param>
        /// <param name="target">Connector of this part. Need to be public</param>
        public void ExposeAs(Connector source, Connector target)
        {
            Context.AssertIsParentOf(source);
            Context.AssertIsOwnerOf(target);
            target.DefineAsAnExpositionOf(source);
        }

        /// <summary>
        /// Same as <see cref="ExposeAs(Connector, Connector)"/>, but instead multiple subcomponent connectors
        /// are combined into a single connection points
        /// </summary>
        /// <param name="sources">Connectors of subcomponents. Need to be public</param>
        /// <param name="target">Connector of this part. Need to be public</param>
        public void ExposeAs(IEnumerable<Connector> sources, Connector target)
        {
            foreach(var c in sources)
                Context.AssertIsParentOf(c);
            Context.AssertIsOwnerOf(target);
            target.DefineAsAnExpositionOf(sources);
        }

        /// <summary>
        /// Connect connectorA and connectorB in a non-descript way, generaly wiring. <br/>
        /// connectorA and connectorB are physicaly distinct connection points. <br/>
        /// TODO : When generating documentation, left / rigth side of connection diagram will try to match this declaration
        /// </summary>
        /// <param name="connectorA">Left side connector. Must be owed by this part or one of its components</param>
        /// <param name="connectorB">Rigth side connector. Must be owed by this part or one of its components</param>
        /// <returns>Object representing the created connection</returns>
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

        /// <summary>
        /// The owning part implementing <see cref="IPartConnectable"/> we are currently processing
        /// </summary>
        Part Context { get; init; }
        // Internal constructor, prevent usage from outside assembly
        internal ConnectionBuilder(Part context)
        {
            Context = context;
        }
    }
}

