using rambap.cplx.PartProperties;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Core;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectionTableProperty
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

    public Component GetConnectedComponent(PortSide side, PortIdentity identity)
        => GetConnectedPort(side, identity).Owner.Parent;

    public Component? GetCableConnectionComponent(PortSide side)
    {
        if (Connection is Cable c)
        {
            return side switch
            {
                PortSide.Left => c.LeftMate.RightPort.Owner.Parent,
                PortSide.Rigth => c.RigthMate.LeftPort.Owner.Parent,
                _ => throw new NotImplementedException(),
            };

        } else return null;
    }
    public Port? GetCableConnectionPort(PortSide side)
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
        PinJunction,
        WireJunction,
    }

    public ConnectionKind GetConnectionKind
        => Connection switch
        {
            StructuralConnection => ConnectionKind.Structural,
            Mate => ConnectionKind.Mate,
            Cable => ConnectionKind.Cable,
            PinJunction => ConnectionKind.PinJunction,
            WireJunction => ConnectionKind.WireJunction,
            _ => throw new NotImplementedException(),
        };

    public PSignal? GetUpperSignal(PortSide side)
    {
        return side switch
        {
            PortSide.Left => Connection.LeftPort.GetUpperSignal(),
            PortSide.Rigth => Connection.RightPort.GetUpperSignal(),
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

    public enum ConnectionCategory
    {
        Assembly,
        Wiring
    }

    public static IEnumerable<ConnectionTableProperty> GetConnectivityTableProperties(
        Component c,
        ConnectionCategory iteratedConnectionCategory)
    {
        var instance = c.Instance;
        var connectivity = instance.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnections(instance, iteratedConnectionCategory);
        // var connectionsFlattened = connections.SelectMany(c => c.Connections);

        // TODO / TBD : grouping previously happenned at global level. npt equivalent to grouping in
        // post trandform ?

        var connectionsGrouped = ConnectionHelpers.GroupConnectionsByPath(connections);

        foreach (var group in connectionsGrouped)
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.LeftPort.GetUpperUsage() != groupLeftConnector;
                if (shouldReverse)
                    yield return new ConnectionTableProperty()
                    {
                        Connection = connection,
                        // Invert left/Rigth of group
                        LeftUpperUsagePort = groupRightConnector,
                        RigthUpperUsagePort = groupLeftConnector
                    };
                else
                    yield return new ConnectionTableProperty()
                    {
                        Connection = connection,
                        LeftUpperUsagePort = groupLeftConnector,
                        RigthUpperUsagePort = groupRightConnector
                    };
            }
        }
    }

    public static IEnumerable<SignalPortConnection> GetAllConnections(Pinstance instance, ConnectionCategory connectionKind)
    {
        // Return all connection, NOT flattening grouped ones (Twisting / Sielding)
        switch (connectionKind)
        {
            case ConnectionCategory.Assembly:
                foreach (var c in instance.Connectivity()?.Connections ?? [])
                    yield return c;
                break;
            case ConnectionCategory.Wiring:
                foreach (var c in instance.Connectivity()?.Wirings ?? [])
                    yield return c;
                break;
        }
    }
}
