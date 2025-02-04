using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Templates;

public abstract class Pin : Part, IPartConnectable, ISingleMateable, ISingleWireable
{
    // Fields are filled by cplx
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ConnectablePort Contact; // Pin or socket, mating side
    public WireablePort Receptacle; // Crmp or solder side, wiring side
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    // ISingleMateablePart contract implementation
    [CplxIgnore]
    public ConnectablePort SingleMateablePort => Contact;

    // ISingleWireablePart contract implementation
    [CplxIgnore]
    public WireablePort SingleWireablePort => Receptacle;

    // Implicit conversions for convenience
    public static implicit operator ConnectablePort(Pin p) => p.SingleMateablePort;
    public static implicit operator WireablePort(Pin p) => p.SingleWireablePort;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.StructuralConnection(Contact, Receptacle);
    }
}
