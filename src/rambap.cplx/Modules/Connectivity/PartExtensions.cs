using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity;

public static class PartExtentions
{
    public static Signal SignalOf(this Part part, WireablePort port)
        => ConnectionConcept.GetSignalFromLocalDir(part, [port]);
    public static Signal SignalOf(this Part part, WireablePort A, WireablePort B)
        => ConnectionConcept.GetSignalFromLocalDir(part, [A, B]);
    public static Signal SignalOf(this Part part, WireablePort A, WireablePort B, WireablePort C)
        => ConnectionConcept.GetSignalFromLocalDir(part, [A,B,C]);
    public static Signal SignalOf(this Part part, WireablePort A, WireablePort B, WireablePort C, WireablePort D)
        => ConnectionConcept.GetSignalFromLocalDir(part, [A, B, C, D]);
    public static Signal SignalOf(this Part part, WireablePort A, WireablePort B, WireablePort C, WireablePort D, WireablePort E)
        => ConnectionConcept.GetSignalFromLocalDir(part, [A, B, C, D, E]);

    public static Signal SignalOf(this Part part, IEnumerable<WireablePort> ports)
        => ConnectionConcept.GetSignalFromLocalDir(part, [.. ports]);
}