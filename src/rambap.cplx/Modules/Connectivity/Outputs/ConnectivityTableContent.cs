using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Core;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTableContent
{
    public enum ConnectorSide
    {
        Left,
        Rigth,
    }


    // TODO : why are those in their own property here, while they can also be deduced from the connection data ?
    // Group connection together on display, detect changs in left/right definition ?
    // This may also be inversed from the ocnnecton left / rigth definition
    public required Port LeftTopMostConnector { get; init; }
    public required Port RigthTopMostConnector { get; init; }
    public required SignalPortConnection Connection { get; init; }

    public Port GetTopMostPort(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => LeftTopMostConnector,
            ConnectorSide.Rigth => RigthTopMostConnector,
            _ => throw new NotImplementedException(),
        };

    public Port GetImmediatePort(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => Connection.LeftPort,
            ConnectorSide.Rigth => Connection.RightPort,
            _ => throw new NotImplementedException(),
        };

    public Component? GetConnectedComponent(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => LeftTopMostConnector.Owner!.Parent,
            ConnectorSide.Rigth => RigthTopMostConnector.Owner!.Parent,
            _ => throw new NotImplementedException(),
        };

    public Component? GetCableConnectionComponent(ConnectorSide side)
    {
        if (Connection is Cable c)
        {
            return side switch
            {
                ConnectorSide.Left => c.LeftMate.RightPort.Owner!.Parent,
                ConnectorSide.Rigth => c.RigthMate.LeftPort.Owner!.Parent,
                _ => throw new NotImplementedException(),
            };

        } else return null;
    }
    public PinstanceModel.Port? GetCableConnectionPort(ConnectorSide side)
    {
        if (Connection is Cable c)
        {
            return side switch
            {
                ConnectorSide.Left => c.LeftMate.RightPort,
                ConnectorSide.Rigth => c.RigthMate.LeftPort,
                _ => throw new NotImplementedException(),
            };

        }
        else return null;
    }

    public enum ConnectionKind
    {
        Structural,
        Mate,
        Cable,
        Wire,
        Bundle,
        Twist,
        Shield,
    }

    public ConnectionKind GetConnectionKind
        => Connection switch
        {
            StructuralConnection => ConnectionKind.Structural,
            Mate => ConnectionKind.Mate,
            Cable => ConnectionKind.Cable,
            Wire => ConnectionKind.Wire,
            Bundle => ConnectionKind.Bundle,
            Twist => ConnectionKind.Twist,
            Shield => ConnectionKind.Shield,
            _ => throw new NotImplementedException(),
        };
}
