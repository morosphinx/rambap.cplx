using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Outputs;

class ConnectivityTableContent
{
    public enum ConnectorSide
    {
        Left,
        Rigth,
    }

    public required Connector LeftConnector { get; init; }
    public required Connector RigthConnector { get; init; }

    public Connector GetConnector(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => LeftConnector,
            ConnectorSide.Rigth => RigthConnector,
            _ => throw new NotImplementedException(),
        };
}
