using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity;

public static class PartExtentions
{
    public static Signal SignalOf(this Part part, params WireablePort[] ports)
        => ConnectionConcept.GetSignalFromLocalDir(part, [.. ports]);
    public static Signal SignalOf(this Part part, IEnumerable<WireablePort> ports)
        => ConnectionConcept.GetSignalFromLocalDir(part, [.. ports]);
}