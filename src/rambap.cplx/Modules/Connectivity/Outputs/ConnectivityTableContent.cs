using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Core;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTableContent
{
    public enum PortSide
    {
        Left,
        Rigth,
    }
    public enum PortIdentity
    {
        /// <summary> Port of the property, no transforms </summary>
        Self,
        /// <summary> Traverse port definition hierarchy upward through all expositions </summary>
        UpperExposition,
        /// <summary> Traverse port definition hierarchy downward through all expositions </summary>
        LowerExposition,
        /// <summary> Traverse port definition hierarchy upward through all expositions and combinations </summary>
        UpperUsage,
    }

    // TODO : why are those in their own property here, while they can also be deduced from the connection data ?
    // Group connection together on display, detect changs in left/right definition ?
    // This may also be inversed from the connection left / rigth definition
    public required Port LeftUpperUsagePort { get; init; }
    public required Port RigthUpperUsagePort { get; init; }
    public required SignalPortConnection Connection { get; init; }

    public Port GetConnectedPort(PortSide side, PortIdentity identity)
    {
        var sidePort = side switch
        {
            PortSide.Left => LeftUpperUsagePort,
            PortSide.Rigth => RigthUpperUsagePort,
            _ => throw new NotImplementedException(),
        };
        var identityPort = identity switch
        {
            PortIdentity.Self => sidePort,
            PortIdentity.UpperExposition => sidePort.GetUpperExposition(),
            PortIdentity.LowerExposition => sidePort.GetLowerExposition(),
            PortIdentity.UpperUsage => sidePort.GetUpperUsage(),
            _ => throw new NotImplementedException(),
        };
        return identityPort;
    }

    public Component? GetConnectedComponent(PortSide side, PortIdentity identity)
        => GetConnectedPort(side, identity).Owner!.Parent;

    public Component? GetCableConnectionComponent(PortSide side)
    {
        if (Connection is Cable c)
        {
            return side switch
            {
                PortSide.Left => c.LeftMate.RightPort.Owner!.Parent,
                PortSide.Rigth => c.RigthMate.LeftPort.Owner!.Parent,
                _ => throw new NotImplementedException(),
            };

        } else return null;
    }
    public PinstanceModel.Port? GetCableConnectionPort(PortSide side)
    {
        if (Connection is Cable c)
        {
            return side switch
            {
                PortSide.Left => c.LeftMate.RightPort,
                PortSide.Rigth => c.RigthMate.LeftPort,
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
