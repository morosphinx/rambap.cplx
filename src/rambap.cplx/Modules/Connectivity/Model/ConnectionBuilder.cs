using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.Model;

namespace rambap.cplx.PartInterfaces;

public partial interface IPartConnectable
{
    /// <summary>
    /// Contains functions to define a part connectivity <br/>
    /// You cannot create this class directly. Instead, implement <see cref="IPartConnectable"/> on a <see cref="Part"/>
    /// </summary>
    public class ConnectionBuilder
    {
        public List<Connection> Connections { get; } = new();


        private void AssertOwnThisCabling(Connection cabling)
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
            Context.AssertIsOwnedBySubComponent(source);
            Context.AssertIsOwner(target);
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
                Context.AssertIsOwnedBySubComponent(c);
            Context.AssertIsOwner(target);
            target.DefineAsAnExpositionOf(sources);
        }

        /// <summary>
        /// Mate connectorA and connectorB, or represent an implicit signal connection between connectors.<br/>
        /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
        /// </summary>
        /// <param name="connectorA">Left side connector. Must be owed by this part or one of its components</param>
        /// <param name="connectorB">Rigth side connector. Must be owed by this part or one of its components</param>
        /// <returns>Object representing the created wire</returns>
        public Mate Mate(Connector connectorA, Connector connectorB)
        {
            Context.AssertIsOwnerOrParent(connectorA);
            Context.AssertIsOwnerOrParent(connectorB);
            // TODO : Test here that both connector are compatible
            var connection = new Mate(connectorA, connectorB);
            Connections.Add(connection);
            return connection;
        }

        /// <summary>
        /// Connect connectorA and connectorB using a non descript, signal carrying wire <br/>
        /// ConnectorA and ConnectorB are physicaly distinct connection points. <br/>
        /// </summary>
        /// <param name="connectorA">Left side connector. Must be owed by this part or one of its components</param>
        /// <param name="connectorB">Rigth side connector. Must be owed by this part or one of its components</param>
        /// <returns>Object representing the created wire</returns>
        public Wire Wire(Connector connectorA, Connector connectorB)
        {
            Context.AssertIsOwnerOrParent(connectorA);
            Context.AssertIsOwnerOrParent(connectorB);
            // TODO : Test here that both conenctor are wireable
            var connection = new Wire(connectorA, connectorB);
            Connections.Add(connection);
            return connection;
        }

        public Twist Twist(IEnumerable<Wireable> twistedCablings)
        {
            foreach (var c in twistedCablings)
                AssertOwnThisCabling(c);
            foreach (var c in twistedCablings)
                Connections.Remove(c);
            var twist = new Twist(twistedCablings);
            Connections.Add(twist);
            return twist;
        }

        public Shield Shield(IEnumerable<Wireable> twistedCablings)
        {
            foreach (var c in twistedCablings)
                AssertOwnThisCabling(c);
            foreach (var c in twistedCablings)
                Connections.Remove(c);
            var twist = new Shield(twistedCablings);
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

