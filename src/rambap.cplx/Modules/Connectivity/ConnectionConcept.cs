using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using static rambap.cplx.PartInterfaces.IPartConnectable;
using static rambap.cplx.PartInterfaces.IPartConnectable.ConnectionBuilder;

namespace rambap.cplx.Modules.Connectivity;

public class InstanceConnectivity : IInstanceConceptProperty
{
    public required List<Connector> Connectors { get; init; }

    public required List<ConnectionInstruction> Connections { get; init; } 

    internal InstanceConnectivity()
    {

    }
}

internal class ConnectionConcept : IConcept<InstanceConnectivity>
{
    public override InstanceConnectivity? Make(Pinstance i, Part template)
    {
        var connectionBuilder = new ConnectionBuilder(template);
        var selfConnectors = new List<Connector>();
        ScanObjectContentFor<Connector>(template,
            (p, s) => { selfConnectors.Add(p);
            });
        // At this point no connector in the seldConnectorList has a definition
        if (template is IPartConnectable a)
        {
            // User defined connection and exposition are created from here
            a.Assembly_Connections(connectionBuilder);
            foreach(var c in selfConnectors)
            {
                if (!c.HasbeenDefined)
                    c.DefineAsHadHoc();
            }
            
            return new InstanceConnectivity()
            {
                Connectors = selfConnectors,
                Connections = connectionBuilder.Connections
            };
        }
        else return null;
    }
}