using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Outputs;

class ConnectivityTableContent
{
    public required Connector LeftConnector { get; init; }
    public required Connector RigthConnector { get; init; }
}
