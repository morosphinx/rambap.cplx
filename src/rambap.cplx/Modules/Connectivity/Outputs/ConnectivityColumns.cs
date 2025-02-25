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

    public static DelegateColumn<ICplxContent> ConnectedPortName(ConnectorSide side, ConnectorIdentity identity, bool fullName = false)
        => new DelegateColumn<ICplxContent>(
            "Port",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => identity switch
                {
                    ConnectorIdentity.Immediate when !fullName => c.Property.GetImmediatePort(side).Label,
                    ConnectorIdentity.Immediate when fullName => c.Property.GetImmediatePort(side).FullDefinitionName(),
                    ConnectorIdentity.Topmost when !fullName => c.Property.GetTopMostPort(side).Label,
                    ConnectorIdentity.Topmost when fullName => c.Property.GetTopMostPort(side).FullDefinitionName(),
                    _ => throw new NotImplementedException(),
                },
                _ => "",
            });

    public static DelegateColumn<ICplxContent> ConnectedComponent(
            ConnectorSide side,
            ConnectorIdentity identity,
            string title,
            Func<Component?,string> getter,
            bool format = false)
        => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => getter.Invoke(identity switch
                {
                    ConnectorIdentity.Immediate => c.Property.GetConnectedComponent(side),
                    ConnectorIdentity.Topmost => c.Property.GetConnectedComponent(side),
                    _ => throw new NotImplementedException(),
                }),
                _ => "",
            });

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalenceTopmostComponent(
            ConnectorSide side,
            string title,
            Func<Component?, string> getter,
            bool format = false)
        => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => 
            {
                switch (i)
                {
                    case IPropertyContent<ConnectivityTableContent> c:
                        var port = c.Property.GetImmediatePort(side);
                        if (!port.HasStructuralEquivalence) return "-";
                        var structuralequiv = port.GetShallowestStructuralEquivalence();
                        var stuctequivtop = structuralequiv.GetTopMostUser();
                        var comp = stuctequivtop.Owner!.Parent;
                        return getter(comp);
                    default:
                        return "";
                }
            });

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalenceTopmostPort(
            ConnectorSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i =>
            {
                switch (i)
                {
                    case IPropertyContent<ConnectivityTableContent> c:
                        var port = c.Property.GetImmediatePort(side);
                        if (!port.HasStructuralEquivalence) return "-";
                        var structuralequiv = port.GetShallowestStructuralEquivalence();
                        var stuctequivtop = structuralequiv.GetTopMostUser();
                        return getter(stuctequivtop);
                    default:
                        return "";
                }
            });

    public static DelegateColumn<ICplxContent> CablePart(
            string title,
            Func<Component, string> getter,
            bool format = false)
        => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => c.Property.Connection switch
                {
                    Cable cable=> getter.Invoke(cable.CableComponent),
                    _ => "",
                },
                _ => "",
            });

    public static DelegateColumn<ICplxContent> CableConnector(
            ConnectorSide side,
            string title,
            Func<Component, string> getter,
            bool format = false)
        => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c =>
                c.Property.Connection switch
                {
                    Cable => getter.Invoke(c.Property.GetCableConnectionComponent(side)!),
                    _ => "",
                },
                _ => "",
            });

    public static DelegateColumn<ICplxContent> CablePort(
           ConnectorSide side,
           string title,
           Func<Port, string> getter,
           bool format = false)
       => new DelegateColumn<ICplxContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => c.Property.Connection switch
                {
                    Cable => getter.Invoke(c.Property.GetCableConnectionPort(side)!),
                    _ => "",
                },
                _ => "",
            });

    public static DelegateColumn<ICplxContent> ConnectionKind()
        => new DelegateColumn<ICplxContent>(
            "Kind",
            ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<ConnectivityTableContent> c => c.Property.GetConnectionKind.ToString(),
                _ => "",
            });
}
