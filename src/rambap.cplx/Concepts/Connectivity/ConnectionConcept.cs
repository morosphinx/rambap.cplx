using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using static rambap.cplx.PartInterfaces.IPartConnectable;
using static rambap.cplx.PartInterfaces.IPartConnectable.ConnectionBuilder;

namespace rambap.cplx.Concepts.Connectivity;

public class InstanceConnectivity : IInstanceConceptProperty
{
    public List<ConnectionInstruction> Connections { get; } = new();

    internal InstanceConnectivity(ConnectionBuilder buildInfo)
    {
        Connections.AddRange(buildInfo.Connections);
    }
}

internal class ConnectionConcept : IConcept<InstanceConnectivity>
{
    public override InstanceConnectivity? Make(Pinstance i, Part template)
    {
        var connectionBuilder = new ConnectionBuilder(template);

        // Construct non declared Constructors
        ScanObjectContentFor<Connector>(template,
            (p, s) => { });

        if (template is IPartConnectable a)
        {
            a.Assembly_Connections(connectionBuilder);
            return new InstanceConnectivity(connectionBuilder);
        }
        else return null;
    }
}