using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.Modules.Connectivity;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Instantiation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// An electrical significant transfered over wiring <br/>
/// Each signal is associated to a single wire or wire group.
/// </summary>
public class Signal : IPartProperty
{
    internal PSignal? Implementation { get; private set; }
    internal void MakeImplementation(string label, Pinstance owner, bool isPublic)
    {
        Implementation = new(label, owner, isPublic);
    }

    internal List<PartPort> Assignations { get; } = [];
}

/// <summary>
/// Signal definition used as an hidden backing of signal created with <see cref="PartExtentions.SignalOf"/>
/// </summary>
internal class ImplicitAssignedSignal : Signal
{
    public required List<ISingleWireable> AssignedPorts { get; init; }
}
