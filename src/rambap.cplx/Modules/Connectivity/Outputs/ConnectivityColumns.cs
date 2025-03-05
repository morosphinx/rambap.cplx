using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ConnectivityColumns
{
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

    public static DelegateColumn<ICplxContent> ConnectedPort(
            PortSide side,
            PortIdentity identity,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => getter(c.GetConnectedPort(side,identity))
            );

    public static DelegateColumn<ICplxContent> ConnectedComponent(
            PortSide side,
            PortIdentity identity,
            string title,
            Func<Component?,string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => getter(c.GetConnectedComponent(side, identity))
            );

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalenceTopmostComponent(
            PortSide side,
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
            PortSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c =>
            {
                var port = c.GetConnectedPort(side,PortIdentity.Self);
                if (!port.HasStructuralEquivalence) return "-";
                var structuralequiv = port.GetShallowestStructuralEquivalence();
                var stuctequivtop = structuralequiv.GetUpperUsage();
                return getter(stuctequivtop);
            });

    public static DelegateColumn<ICplxContent> ConnectedStructuralEquivalence(
            PortSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format ,
            c =>
            {
                var port = c.GetConnectedPort(side, PortIdentity.Self);
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
            PortSide side,
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
           PortSide side,
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
