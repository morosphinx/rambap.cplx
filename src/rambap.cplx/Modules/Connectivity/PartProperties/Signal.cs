using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Templates;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class Signal : IPartProperty
{
    // No unbacked property support ("=>") with implicit conversion ?
    // Would cause confusion with two different cases, => Signal ; and => SignalPort ; both valid
    // public static implicit operator Signal(SignalPort s) => throw new NotImplementedException();

    // Must always have a builder with a Do.() method for parametric part construction, so let's do that
}
