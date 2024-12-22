using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.PartInterfaces;

public interface IPartConnectable
{
    public void Assembly_Connections(ConnectionBuilder Do);

    public class ConnectionBuilder
    {
        public record ConnectionInstruction(Connector connectorA, Connector connectorB)
        {

        }

        public List<ConnectionInstruction> Connections { get; } = new();

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

        public void Connect(Connector connectorA, Connector connectorB)
        {
            Context.AssertIsParentOf(connectorA);
            Context.AssertIsParentOf(connectorB);
            Connections.Add(new ConnectionInstruction(connectorA, connectorB));

            // TODO save exposition information
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

