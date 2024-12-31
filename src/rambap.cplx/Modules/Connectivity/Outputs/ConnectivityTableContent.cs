﻿using rambap.cplx.PartProperties;
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
    public required SignalPort LeftTopMostConnector { get; init; }
    public required SignalPort RigthTopMostConnector { get; init; }

    public required ISignalingAction Connection { get; init; }

    public SignalPort GetTopMostConnector(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => LeftTopMostConnector,
            ConnectorSide.Rigth => RigthTopMostConnector,
            _ => throw new NotImplementedException(),
        };

    public SignalPort GetImmediateConnector(ConnectorSide side)
        => side switch
        {
            ConnectorSide.Left => Connection.LeftPort,
            ConnectorSide.Rigth => Connection.RightPort,
            _ => throw new NotImplementedException(),
        };

    public enum ConnectionKind
    {
        Structural,
        Mate,
        Wire,
        Bundle,
        Twist,
        Shield,
    }

    public ConnectionKind GetConnectionKind
        => Connection switch
        {
            StructuralConnection m => ConnectionKind.Structural,
            Mate m => ConnectionKind.Mate,
            Wire m => ConnectionKind.Wire,
            Bundle m => ConnectionKind.Bundle,
            Twist m => ConnectionKind.Twist,
            Shield m => ConnectionKind.Shield,
            _ => throw new NotImplementedException(),
        };
}