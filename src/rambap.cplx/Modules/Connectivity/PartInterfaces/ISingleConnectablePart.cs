using rambap.cplx.PartProperties;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartInterfaces;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Interface indicating that the part only have a single Mateable connector. <br/>
/// Calls to <see cref="ConnectionBuilder.Mate"/> may omit the connector. <br/>
/// When creating the Pinstance, the part will raise an error if there is more than one ConnectablePort.
/// </summary>
public interface ISingleMateable
{
    ConnectablePort SingleMateablePort { get; }
}

/// <summary>
/// Interface indicating that the part only have a single Wireable connector. <br/>
/// Calls to <see cref="ConnectionBuilder.Wire"/> may omit the connector. <br/>
/// When creating the Pinstance, the part will raise an error if there is more than one WireablePort.
/// </summary>
public interface ISingleWireable
{
    WireablePort SingleWireablePort { get; }
}
