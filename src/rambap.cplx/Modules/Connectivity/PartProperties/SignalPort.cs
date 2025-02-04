using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public abstract class SignalPort : IPartProperty
{
    internal Stack<Port> Implementations { get; } = new();
    internal Port LocalImplementation => Implementations.Peek();

    /* Dead code moved to Pintance model Port.cs, to clean
    public static bool AreCompatible(SignalPort A, SignalPort B)
     => A.GetType() == B.GetType() &&
        AreCompatible(A.Definition!, B.Definition!);
    internal static bool AreCompatible(PortDefinition A, PortDefinition B)
    {
        return (A, B) switch
        {
            // Copied connectors : Test one level deeper
            (ExposedDefinition a, ExposedDefinition b) => AreCompatible(a.ExposedPort.Definition!, b.ExposedPort.Definition!),
            (ExposedDefinition a, AdHocDefinition b) => AreCompatible(a.ExposedPort.Definition!, b),
            (ExposedDefinition a, CombinedDefinition b) => AreCompatible(a.ExposedPort.Definition!, b),
            (AdHocDefinition a, ExposedDefinition b) => AreCompatible(a, b.ExposedPort.Definition!),
            (CombinedDefinition a, ExposedDefinition b) => AreCompatible(a, b.ExposedPort.Definition!),
            // Add Hoc : Always compatible with itself
            (AdHocDefinition a, AdHocDefinition b) => true,
            // Add Hoc and combined are never compatible : Even whith a single pin, a combined connector imply an additional containing level
            (AdHocDefinition a, CombinedDefinition b) => false,
            (CombinedDefinition a, AdHocDefinition b) => false,
            // Two Combined : Valid when same size, compatible subconnectors in order
            (CombinedDefinition a, CombinedDefinition b) =>
                a.CombinedPorts.Count == b.CombinedPorts.Count &&
                Enumerable.Range(0, a.CombinedPorts.Count)
                          .All(i => AreCompatible(a.CombinedPorts[i].Definition!, b.CombinedPorts[i].Definition!)),
            // 3 x 3 = 9 total cases, all covered
            _ => throw new NotImplementedException(),
        };
    }
    */
}
