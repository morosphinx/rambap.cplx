using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.PartProperties;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ConnectivityColumns
{
    public enum ConnectorIdentity
    {
        Immediate, // Will display the immediate connector
        Topmost, // Will traverse the connector hierarchy and return information relative to the uppermost Expose() call
    }

    public static DelegateColumn<ConnectivityTableContent> ConnectedPortName(ConnectorSide side, ConnectorIdentity identity, bool fullName = false)
        => new DelegateColumn<ConnectivityTableContent>(
            "Port",
            ColumnTypeHint.StringExact,
            i => identity switch
            {
                ConnectorIdentity.Immediate when !fullName   => i.GetImmediatePort(side).Label,
                ConnectorIdentity.Immediate when fullName  => i.GetImmediatePort(side).FullDefinitionName(),
                ConnectorIdentity.Topmost when !fullName     => i.GetTopMostPort(side).Label,
                ConnectorIdentity.Topmost when fullName    => i.GetTopMostPort(side).FullDefinitionName(),
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ConnectivityTableContent> ConnectedComponent(
            ConnectorSide side,
            ConnectorIdentity identity,
            string title,
            Func<Component?,string> getter,
            bool format = false)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => getter.Invoke(identity switch
            {
                ConnectorIdentity.Immediate => i.GetConnectedComponent(side),
                ConnectorIdentity.Topmost   => i.GetConnectedComponent(side),
                _ => throw new NotImplementedException(),
            }));

    public static DelegateColumn<ConnectivityTableContent> ConnectedStructuralEquivalenceTopmostComponent(
            ConnectorSide side,
            string title,
            Func<Component?, string> getter,
            bool format = false)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i =>
            {
                var port = i.GetImmediatePort(side);
                if (!port.HasStructuralEquivalence) return "-";
                var structuralequiv = port.GetShallowestStructuralEquivalence();
                var stuctequivtop = structuralequiv.GetTopMostUser();
                var comp = stuctequivtop.Owner!.Parent;
                return getter(comp);
            });
    public static DelegateColumn<ConnectivityTableContent> ConnectedStructuralEquivalenceTopmostPort(
            ConnectorSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i =>
            {
                var port = i.GetImmediatePort(side);
                if (!port.HasStructuralEquivalence) return "-";
                var structuralequiv = port.GetShallowestStructuralEquivalence();
                var stuctequivtop = structuralequiv.GetTopMostUser();
                return getter(stuctequivtop);
            });

    public static DelegateColumn<ConnectivityTableContent> CablePart(
            string title,
            Func<Component,string> getter,
            bool format = false)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i.Connection switch
            {
                Cable c => getter.Invoke(c.CableComponent) ,
                _ => "",
            });

    public static DelegateColumn<ConnectivityTableContent> CableConnector(
            ConnectorSide side,
            string title,
            Func<Component, string> getter,
            bool format = false)
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i.Connection switch
            {
                Cable c => getter.Invoke(i.GetCableConnectionComponent(side)!),
                _ => "",
            });

    public static DelegateColumn<ConnectivityTableContent> CablePort(
           ConnectorSide side,
           string title,
           Func<Port, string> getter,
           bool format = false)
       => new DelegateColumn<ConnectivityTableContent>(
           title,
           format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
           i => i.Connection switch
           {
               Cable c => getter.Invoke(i.GetCableConnectionPort(side)!),
               _ => "",
           });

    public static DelegateColumn<ConnectivityTableContent> Dashes(string title = "-- Connect to --")
        => new DelegateColumn<ConnectivityTableContent>(
            title,
            ColumnTypeHint.StringExact,
            i => new string('-',title.Length));

    public static DelegateColumn<ConnectivityTableContent> EmptyColumn(string title = "")
        => new DelegateColumn<ConnectivityTableContent>(title, ColumnTypeHint.StringFormatable,
            i => "");

    public static DelegateColumn<ConnectivityTableContent> ConnectionKind()
        => new DelegateColumn<ConnectivityTableContent>(
            "Kind",
            ColumnTypeHint.StringFormatable,
            i => i.GetConnectionKind.ToString());
}
