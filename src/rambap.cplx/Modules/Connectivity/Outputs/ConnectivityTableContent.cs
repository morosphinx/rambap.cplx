using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.Model;

namespace rambap.cplx.Modules.Connectivity.Outputs;

class ConnectivityTableContent
{
    public enum ConnectorSide
    {
        Left,
        Rigth,
    }

    // TODO : why are those in their own property here, while they can also be deduced from the connection data ?
    // Group connection together on display, detect changs in left/right definition ?
    // This may also be inversed from the ocnnecton left / rigth definition
    public required Connector LeftTopMostConnector { get; init; }
    public required Connector RigthTopMostConnector { get; init; }

    public required Connection Connection { get; init; }

    public Connector GetTopMostConnector(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => LeftTopMostConnector,
            ConnectorSide.Rigth => RigthTopMostConnector,
            _ => throw new NotImplementedException(),
        };

    public Connector GetImmediateConnector(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => Connection.ConnectorA,
            ConnectorSide.Rigth => Connection.ConnectorB,
            _ => throw new NotImplementedException(),
        };

    public enum ConnectionKind
    {
        Mate,
        Wire,
        Twist,
        Shield,
    }

    public ConnectionKind GetConnectionKind
        => Connection switch
        {
            Mate m => ConnectionKind.Mate,
            Wire m => ConnectionKind.Wire,
            Twist m => ConnectionKind.Twist,
            Shield m => ConnectionKind.Shield,
            _ => throw new NotImplementedException(),
        };
}
