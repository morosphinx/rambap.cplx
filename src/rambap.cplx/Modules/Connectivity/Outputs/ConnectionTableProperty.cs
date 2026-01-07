using rambap.cplx.Instantiation;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using static rambap.cplx.Modules.Connectivity.PinstanceModel.InstanceConnectivity;

namespace rambap.cplx.Modules.Connectivity.Outputs;

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
public enum ConnectionKind
{
    Structural,
    Mate,
    PinJunction,
    WireJunction,
    WireMesh,
}

/// <summary>
/// Reprsent a link in an exported table
/// </summary>
public abstract class ConnectivityTableProperty
{
    /// <summary>
    /// The left Linked port
    /// </summary>
    public abstract Port LeftLinkPort { get; }

    /// <summary>
    /// The rigth Linked port
    /// </summary>
    public abstract Port RigthLinkPort { get; }

    /// <summary>
    /// The relevant endpoint that contains the <see cref="LeftLinkPort"/>. <br/>
    /// This is either the port itself or the most relevant connector that contains it
    /// </summary>
    public Port LeftEndpointPort => LeftLinkPort.GetUpperEndpointIdentityPort();

    /// <summary>
    /// The relevant endpoint that contains the <see cref="RigthLinkPort"/>. <br/>
    /// This is either the port itself or the most relevant connector that contains it
    /// </summary>
    public Port RigthEndpointPort => RigthLinkPort.GetUpperEndpointIdentityPort();

    /// <summary>
    /// If true, method that return a port based on <see cref="PortSide"/> return the other port instead <br/>
    /// Used to control the link direction display
    /// </summary>
    public required bool ShowReverted { get; init; } // TODO
    private PortSide MayRevert(PortSide side)
        => side switch
        {
            _ when !ShowReverted => side,
            PortSide.Left => PortSide.Rigth,
            PortSide.Rigth => PortSide.Left,
            _ => throw new NotImplementedException(),
        };

    public abstract ConnectionKind ConnectionKind { get; }

    public Port GetEndpointPort(PortSide side)
    {
        var effectiveSide = MayRevert(side);
        var sidePort = effectiveSide switch
        {
            PortSide.Left => LeftEndpointPort,
            PortSide.Rigth => RigthEndpointPort,
            _ => throw new NotImplementedException(),
        };
        return sidePort;
    }

    public Port GetLinkPort(PortSide side, PortIdentity identity)
    {
        var effectiveSide = MayRevert(side);
        var sidePort = effectiveSide switch
        {
            PortSide.Left => LeftLinkPort,
            PortSide.Rigth => RigthLinkPort,
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

    public Component GetLinkedComponent(PortSide side, PortIdentity identity)
    {
        var effectiveSide = MayRevert(side);
        return GetLinkPort(effectiveSide, identity).Owner.Parent;
    }

    public PSignal? GetUpperSignal(PortSide side)
    {
        var effectiveSide = MayRevert(side);
        return effectiveSide switch
        {
            PortSide.Left => LeftLinkPort.GetUpperSignal(),
            PortSide.Rigth => RigthLinkPort.GetUpperSignal(),
            _ => throw new NotImplementedException(),
        };
    }
    public string GetLikelySignal(string separator = " / ")
    {
        IEnumerable<PSignal?> signals = [GetUpperSignal(PortSide.Left), GetUpperSignal(PortSide.Rigth)];
        var signalNames = signals.Where(s => s != null)
            .Select(s => s!.Label)
            .Distinct();
        return string.Join(separator, signalNames);
    }

}

public class ConnectionTableProperty : ConnectivityTableProperty
{
    public override Port LeftLinkPort => Connection.LeftPort;
    public override Port RigthLinkPort => Connection.RightPort;
    public required SignalPortConnection Connection { get; init; }
    public override ConnectionKind ConnectionKind
        => Connection switch
        {
            StructuralConnection => ConnectionKind.Structural,
            Mate => ConnectionKind.Mate,
            PinJunction => ConnectionKind.PinJunction,
            WireJunction => ConnectionKind.WireJunction,
            _ => throw new NotImplementedException(),
        };

    public static IEnumerable<ConnectionTableProperty> GetConnectionTableProperty(Component c)
    {
        var instance = c.Instance;
        var connections = instance.Connectivity()?.Connections ?? [];

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByPath(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.LeftPort.GetUpperUsage() != groupLeftConnector;
                yield return new ConnectionTableProperty()
                    {
                        Connection = connection,
                        // Invert left/Rigth of group
                        ShowReverted = shouldReverse
                    };
            }
        }
    }

    public Component? GetCableConnectionComponent(PortSide side)
    {
        // if (Connection is Cable c)
        // {
        //     return side switch
        //     {
        //         PortSide.Left => c.LeftMate.RightPort.Owner.Parent,
        //         PortSide.Rigth => c.RigthMate.LeftPort.Owner.Parent,
        //         _ => throw new NotImplementedException(),
        //     };
        // 
        // } else return null;
        return null; // TEMP DISABLE
    }
    public Port? GetCableConnectionPort(PortSide side)
    {
        // if (Connection is Cable c)
        // {
        //     return side switch
        //     {
        //         PortSide.Left => c.LeftMate.RightPort,
        //         PortSide.Rigth => c.RigthMate.LeftPort,
        //         _ => throw new NotImplementedException(),
        //     };
        // 
        // }
        // else return null;
        return null; // TEMP DISABLE
    }
}

public class WiringTableProperty : ConnectivityTableProperty
{

    public required WireMesh_DualEnded WireMesh { get; init; }
    public override Port LeftLinkPort => WireMesh.LeftPort;
    public override Port RigthLinkPort => WireMesh.RightPort;

    public override ConnectionKind ConnectionKind => ConnectionKind.WireMesh;

    public static IEnumerable<WiringTableProperty> GetWiringTableProperty(Component c)
    {
        var instance = c.Instance;
        var meshes = instance.Connectivity()?.GetWireMeshes() ?? [];

        foreach (var m in meshes)
        {
            yield return new WiringTableProperty()
            {
                WireMesh = new WireMesh_DualEnded(m),

                ShowReverted = false,
            };
        }
    }
}

