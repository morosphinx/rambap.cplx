using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.PartProperties;
using System.Diagnostics.Metrics;
using System.Drawing;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ConnectivityColumns
{
    public enum ConnectorIdentity
    {
        Immediate, // Will display the immediate connector
        Topmost, // Will traverse the connector hierarchy and return information relative to the uppermost Expose() call
    }

    public static DelegateColumn<ICplxContent> MakeConnectivityColumn(
        string columnName, bool format, Func<ConnectivityTableContent, string> getter)
        => new DelegateColumn<ICplxContent>(
            columnName,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => getter(c.Property),
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> ConnectedPortName(ConnectorSide side, ConnectorIdentity identity, bool fullName = false)
        => MakeConnectivityColumn(
            "Port",
            false,
            c =>  identity switch
            {
                ConnectorIdentity.Immediate when !fullName => c.GetImmediatePort(side).Label,
                ConnectorIdentity.Immediate when fullName => c.GetImmediatePort(side).FullDefinitionName(),
                ConnectorIdentity.Topmost when !fullName => c.GetTopMostPort(side).Label,
                ConnectorIdentity.Topmost when fullName => c.GetTopMostPort(side).FullDefinitionName(),
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> ConnectedComponent(
            ConnectorSide side,
            ConnectorIdentity identity,
            string title,
            Func<Component?,string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => getter(identity switch
            {
                ConnectorIdentity.Immediate => c.GetConnectedComponent(side),
                ConnectorIdentity.Topmost => c.GetConnectedComponent(side),
                _ => throw new NotImplementedException(),
            }));

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalenceTopmostComponent(
            ConnectorSide side,
            string title,
            Func<Component?, string> getter,
            bool format = false)
        => ConnectedStructuralEquivalenceTopmostPort(
            side,
            title,
            i => 
            {
                var component = i.Owner!.Parent;
                return getter(component);
            },
            format);

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalenceTopmostPort(
            ConnectorSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c =>
            {
                var port = c.GetImmediatePort(side);
                if (!port.HasStructuralEquivalence) return "-";
                var structuralequiv = port.GetShallowestStructuralEquivalence();
                var stuctequivtop = structuralequiv.GetTopMostUser();
                return getter(stuctequivtop);
            });

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalence(
            ConnectorSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format ,
            c =>
            {
                var port = c.GetImmediatePort(side);
                if (!port.HasStructuralEquivalence) return "-";
                var structuralequiv = port.GetShallowestStructuralEquivalence();
                return getter(structuralequiv);
            });

    public static DelegateColumn<ICplxContent> CablePart(
            string title,
            Func<Component, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => c.Connection switch
            {
                Cable cable=> getter.Invoke(cable.CableComponent),
                _ => "",
            });

    public static DelegateColumn<ICplxContent> CableConnector(
            ConnectorSide side,
            string title,
            Func<Component, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => c.Connection switch
            {
                Cable => getter.Invoke(c.GetCableConnectionComponent(side)!),
                _ => "",
            });

    public static DelegateColumn<ICplxContent> CablePort(
           ConnectorSide side,
           string title,
           Func<Port, string> getter,
           bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => c.Connection switch
            {
                Cable => getter.Invoke(c.GetCableConnectionPort(side)!),
                _ => "",
            });

    public static DelegateColumn<ICplxContent> ConnectionKind()
        => MakeConnectivityColumn(
            "Kind",
            true,
            c => c.GetConnectionKind.ToString()
            );
}
