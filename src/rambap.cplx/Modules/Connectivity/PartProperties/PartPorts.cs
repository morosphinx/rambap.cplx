using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A port, generaly electrical, that can carry a signal<br/>
/// Do not declare on a Part, instead declare either a <see cref="ConnectablePort"/> or an <see cref="WireablePort"/>
/// </summary>
public abstract class PartPort : IPartProperty
{
    /// <summary>
    /// A stack of Pinstance's <see cref="Port"/> e
    /// Pinstance initialisation order must guarantee that Implementation are stacked in component order <br/>
    /// Eg : ports lower in the stack are owned by subcomponent of upper items
    /// </summary>
    internal Stack<Port> Implementations { get; } = new();

    /// <summary>
    /// Return the top of the <see cref="Implementations"/> stack, eg : during Pinstance initialisation, <br/>
    /// this is equivalent to the last (= topmost) implementation of the Port
    /// </summary>
    internal Port LocalImplementation => Implementations.Peek();
}

/// <summary>
/// Define a connectable element, generaly electrical, on a Part. <br/>
/// To be then used by Parts implementing <see cref="PartInterfaces.IPartConnectable"/> <br/>
/// Define <see cref="ConnectablePort"/> as public when they can be seen and used from outside the Part.
/// </summary>
public sealed class ConnectablePort : PartPort, ISingleMateable
{
    public ConnectablePort SingleMateablePort => this;
}

/// <summary>
/// Define a wireable element, generaly electrical, on a Part. <br/>
/// To be then used by Parts implementing <see cref="PartInterfaces.IPartConnectable"/> <br/>
/// Define <see cref="ConnectablePort"/> as public when they can be seen and used from outside the Part.
/// </summary>
public sealed class WireablePort : PartPort, ISingleWireable
{
    public WireablePort SingleWireablePort => this;
}

/// <summary>
/// The end of a wire. Created as extremities of a <see cref="WirePart"/>, and can be joined together or to <see cref="WireablePort"/>
/// </summary>
public sealed class WireEnd : PartPort
{

}