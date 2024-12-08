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
            CheckIsFromThisOrParent(source);
            CheckIsOwnedByThis(target);
            if (target.HasBeenExposed)
                throw new InvalidOperationException($"Connection {target} has already been exposed from part {Context}");
            target.HasBeenExposed = true;
        }
        public void ExposeAs(IEnumerable<Connector> sources, Connector target)
        {
            foreach(var c in sources)
                CheckIsFromThisOrParent(c);
            CheckIsOwnedByThis(target);
            if (target.HasBeenExposed)
                throw new InvalidOperationException($"Connection {target} has already been exposed from part {Context}");
            target.HasBeenExposed = true;
        }

        public void Connect(Connector connectorA, Connector connectorB)
        {
            CheckIsFromThisOrParent(connectorA);
            CheckIsFromThisOrParent(connectorB);
            Connections.Add(new ConnectionInstruction(connectorA, connectorB));
        }


        private void CheckIsOwnedByThis(IPartProperty property)
        {
            if (Context != property.Owner)
                throw new InvalidOperationException("Property must be owned by this object");
        }
        private void CheckIsFromThisOrParent(IPartProperty partProperty)
            => CheckIsThisOrParent(partProperty.Owner!);

        private void CheckIsThisOrParent(Part part)
        {
            if (part == Context)
                return; // We are in self component tree, all is good
            else if (part.Parent == null)
                throw new InvalidOperationException("Part is connected is not in this component's tree");
            else
                CheckIsThisOrParent(part.Parent);
        }


        Part Context { get; init; }
        // Internal constructor, prevent usage from outside assembly
        internal ConnectionBuilder(Part context)
        {
            Context = context;
        }
    }
}

