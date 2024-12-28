using rambap.cplx.Core;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using rambap.cplx.Modules.Connectivity.Model;
using static rambap.cplx.PartInterfaces.IPartConnectable;

namespace rambap.cplx.Modules.Connectivity;

public class InstanceConnectivity : IInstanceConceptProperty
{
    // TODO : set definition somewhere in the Part
    public bool IsACable { get; init; } = true;

    public required List<Connector> PublicConnectors { get; init; }

    public required List<Connector> Connectors { get; init; }

    public required List<Connection> Connections { get; init; }

    public enum DisplaySide
    {
        Left,
        Rigth,
        Both,
    }

    internal InstanceConnectivity()
    {

    }
}

internal class ConnectionConcept : IConcept<InstanceConnectivity>
{
    public override InstanceConnectivity? Make(Pinstance instance, Part template)
    {
        var connectionBuilder = new ConnectionBuilder(template);
        var selfConnectors = new List<Connector>();
        var selfPublicConnectors = new List<Connector>();
        ScanObjectContentFor<Connector>(template,
            (p, s) => {
                selfConnectors.Add(p);
                if(s.IsPublicOrAssembly) selfPublicConnectors.Add(p);
            });
        // At this point no connector in the selfConnectorList has a definition
        if (template is IPartConnectable a)
        {
            // User defined connection and exposition are created from here
            a.Assembly_Connections(connectionBuilder);
            foreach(var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }

            var selfDefinedConnection = connectionBuilder!.Connections;

            // All Components that have a Connectivity definition are used as black boxes
            foreach (var c in instance.Components)
            {
                var connectivity = c.Instance.Connectivity();
                if(connectivity != null)
                {
                    if (connectivity.IsACable)
                    {
                        var cableConnectors = connectivity.PublicConnectors;
                        AbstractConnectionDueToCable(selfDefinedConnection, cableConnectors);
                    }
                }
            }
            
            return new InstanceConnectivity()
            {
                PublicConnectors = selfPublicConnectors,
                Connectors = selfConnectors,
                Connections = selfDefinedConnection
            };
        }
        else return null;
    }

    private void AbstractConnectionDueToCable(List<Connection> cablings, IEnumerable<Connector> cableConnectors)
    {

    }
}