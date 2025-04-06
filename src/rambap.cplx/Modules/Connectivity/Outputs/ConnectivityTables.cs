using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;
using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ConnectivityTables
{
    // Connection tables

    public enum ConnectionKind
    {
        Assembly,
        Wiring
    }

    private static IEnumerable<ConnectionTableProperty> GetConnectivityTableProperties(
        Component c,
        ConnectionKind iteratedConnectionKind)
    {
        var instance = c.Instance;
        var connectivity = instance.Connectivity()!; // Throw if no connectivity definition
        var connections = GetAllConnections(instance, iteratedConnectionKind);
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

    public static IEnumerable<SignalPortConnection> GetAllConnections(Pinstance instance, ConnectionKind connectionKind)
    {
        // Return all connection, NOT flattening grouped ones (Twisting / Sielding)
        switch (connectionKind)
        {
            case ConnectionKind.Assembly:
                foreach (var c in instance.Connectivity()?.Connections ?? [])
                    yield return c;
                break;
            case ConnectionKind.Wiring:
                foreach (var c in instance.Connectivity()?.Wirings ?? [])
                    yield return c;
                break;
        }
    }

    public static TableProducer<ICplxContent> ConnectionTable(bool recurse = true)
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ConnectionTableProperty>()
            {
                PropertyIterator = c => GetConnectivityTableProperties(
                    c, ConnectionKind.Assembly),
                WriteBranches = false,
                RecursionCondition = (c, l) => recurse,
            },
            ContentTransform = cs => cs.Where(c => c is not IPureComponentContent),
            Columns = [
                    MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()),
                    Dashes("--"),
                    ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPort(PortSide.Left,PortIdentity.UpperUsage,"Port", c => c.Label),
                    Dashes("--"),
                    CablePart("Cable",c => c.CN),
                    Dashes("--"),
                    ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CID", c => c?.CID(" / ") ?? "."),
                    ConnectedPort(PortSide.Rigth,PortIdentity.UpperUsage,"Port", c => c.Label),
                    Dashes("--"),
                    CablePart("Cable",c => c.Instance.PN),
                    CablePart("Description",c => c.Instance.Documentation()?.Descriptions.FirstOrDefault()?.Text ?? ""),
                    CablePort(PortSide.Left, "L", c => c.GetLowerExposition().Owner.PN),
                    CablePort(PortSide.Rigth, "R", c => c.GetLowerExposition().Owner.PN),
                ]
        };

    public static TableProducer<ICplxContent> WiringTable(bool recurse = true)
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ConnectionTableProperty>()
            {
                PropertyIterator = c => GetConnectivityTableProperties(
                    c, ConnectionKind.Wiring),
                WriteBranches = false,
                RecursionCondition = (c,l) => recurse,
            },
            ContentTransform = cs => cs.Where(c => c is not IPureComponentContent),
            Columns = [
                    ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(PortSide.Left,"Connector", c => c.Label),
                    ConnectedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
                    Dashes("--"),
                    ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CN", c => c.CN),
                    ConnectedStructuralEquivalenceTopmostPort(PortSide.Rigth,"Connector", c => c.Label),
                    ConnectedPort(PortSide.Rigth,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
                    Dashes("--"),
                    MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()),
                ]
        };

    // ICD Table

    private static IEnumerable<ICDTableProperty> SelectPublicTopmostConnectors(Component component)
    {
        var connectivity = component.Instance.Connectivity();
        if (connectivity != null)
        {
            var publicConnectors = connectivity.Connectors.Where(c => c.IsPublic);
            var publicTopMostConnectors = publicConnectors.Where(c => c.GetUpperUsage() == c);
            foreach (var con in publicTopMostConnectors)
            {
                yield return new ICDTableProperty() { Port = con };
            }
        }
    }
    private static IEnumerable<ICDTableProperty> SelectConnectorSubs(ICDTableProperty content)
    {
        var port = content.Port;
        // if (port.Definition is PortDefinition_Combined def)
        {
            var subConnectors = port.Definition!.SubPorts;
            foreach (var con in subConnectors)
            {
                yield return new ICDTableProperty() { Port = con };
            }
        }
    }

    public static TableProducer<ICplxContent> InterfaceControlDocumentTable()
        => new TableProducer<ICplxContent>()
        {
            Iterator = new ComponentPropertyIterator<ICDTableProperty>()
            {
                PropertyIterator = SelectPublicTopmostConnectors,
                PropertySubIterator = SelectConnectorSubs,
                RecursionCondition = (c, l) => c.IsPublic,
                WriteBranches = true,
                GroupPNsAtSameLocation = false,
                StackPropertiesSingleChildBranches = false,
            },
            ContentTransform = contents => contents.Where(c =>
                c switch
                {
                    IPropertyContent<ICDTableProperty> lp => true,
                    _ => c.Component.IsPublic, // Private part are still present as leaf, we remove them
                }),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree<ICDTableProperty>(
                        i =>
                        {
                            var prop = i.Property;
                            if(prop.Port.HasStructuralEquivalence)
                               return prop.Port.GetShallowestStructuralEquivalence().GetUpperExposition().Label ?? "";
                            return prop.Port.Label ?? "";
                        }),
                    ICDColumns.TopMostPortPart(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ],
        };
}
