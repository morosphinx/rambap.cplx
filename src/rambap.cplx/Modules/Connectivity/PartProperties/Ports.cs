using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.PartInterfaces;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A port, generaly electrical, that can carry a signal<br/>
/// Do not declare on a Part, instead declare either a <see cref="ConnectablePort"/> or an <see cref="WireablePort"/>
/// </summary>
public abstract class SignalPort : IPartProperty
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
public class ConnectablePort : SignalPort, ISingleMateable
{
    public ConnectablePort SingleMateablePort => this;
}

/// <summary>
/// Define a wireable element, generaly electrical, on a Part. <br/>
/// To be then used by Parts implementing <see cref="PartInterfaces.IPartConnectable"/> <br/>
/// Define <see cref="ConnectablePort"/> as public when they can be seen and used from outside the Part.
/// </summary>
public class WireablePort : SignalPort, ISingleWireable
{
    public WireablePort SingleWireablePort => this;
}


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