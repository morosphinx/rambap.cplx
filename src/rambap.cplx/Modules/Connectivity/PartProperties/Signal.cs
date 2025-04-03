using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Modules.Connectivity.Templates;
using rambap.cplx.PartInterfaces;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class Signal : IPartProperty
{
    internal PSignal? Implementation { get; private set; }
    internal void MakeImplementation(string label, Pinstance owner, bool isPublic)
    {
        Implementation = new(label, owner, isPublic);
    }

    internal List<SignalPort> Assignations { get; } = [];
}

internal class ImplicitAssignedSignal : Signal
{
    public required List<ISingleWireable> AssignedPorts { get; init; }
}
