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

    // Is unbacked property support ("=>") with implicit conversion a good idea ?
    // Would cause confusion with two different cases, => Signal ; and => SignalPort ; both valid
    // public static implicit operator Signal(SignalPort s) => throw new NotImplementedException();

    // TODO : This produces local (new) signal IPartProperties => Theses are created anew with each call to get()
    // as result,
    //  - they do _not_ pass IPartPropety initialisation before use
    //  - when set as public, extenal call doesn't work on the same object

    // Are not implicit operation, because of ConnectionBuilder.Wire() overloads
    public static explicit operator Signal(WireablePort assignation)
        => new ImplicitAssignedSignal() { AssignedPorts = [assignation] };
    public static explicit operator Signal(List<WireablePort> assignations)
        => new ImplicitAssignedSignal() { AssignedPorts = [..assignations] };
}

internal class ImplicitAssignedSignal : Signal
{
    public required List<ISingleWireable> AssignedPorts { get; init; }
}
