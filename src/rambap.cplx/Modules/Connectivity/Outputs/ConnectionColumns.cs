using rambap.cplx.Instantiation;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using System.Reflection.Emit;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ConnectionColumns
{
    public static DelegateColumn<IContent> MakeConnectivityColumn(
        string columnName, bool format, Func<ConnectivityTableProperty, string> getter)
        => new DelegateColumn<IContent>(
            columnName,
            format ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ConnectivityTableProperty> c => getter(c.Property),
                _ => throw new NotImplementedException(),
            });
    public static DelegateColumn<IContent> LinkedComponent(
            PortSide side,
            PortIdentity identity,
            string title,
            Func<Component, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => getter(c.GetLinkedComponent(side, identity))
            );

    public static DelegateColumn<IContent> LinkedPort(
            PortSide side,
            PortIdentity identity,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => getter(c.GetLinkPort(side,identity))
            );

    public static DelegateColumn<IContent> EndpointComponent(
            PortSide side,
            string title,
            Func<Component?, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c =>
            {
                var endpointPort = c.GetEndpointPort(side);
                var component = endpointPort.Owner.User;
                return getter(component);
            });

    public static DelegateColumn<IContent> EndpointPort(
            PortSide side,
            string title,
            Func<Port, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c =>
            {
                var endpointPort = c.GetEndpointPort(side);
                return getter(endpointPort);
            });


    public static DelegateColumn<IContent> CablePart(
            string title,
            Func<Component, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => ""
            //c => c.Connection switch
            //{
            //    // TEMP DISABLE
            //    // Cable cable=> getter.Invoke(cable.CableComponent),
            //    _ => "",
            //}
            );

    public static DelegateColumn<IContent> CableConnector(
            PortSide side,
            string title,
            Func<Component, string> getter,
            bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => ""
            //c => c.Connection switch
            //{
            //    // TEMP DISABLE
            //    // Cable => getter.Invoke(c.GetCableConnectionComponent(side)!),
            //    _ => "",
            //}
            );

    public static DelegateColumn<IContent> CablePort(
           PortSide side,
           string title,
           Func<Port, string> getter,
           bool format = false)
        => MakeConnectivityColumn(
            title,
            format,
            c => ""
            //c => c.Connection switch
            //{
            //    // TEMP DISABLE
            //    // Cable => getter.Invoke(c.GetCableConnectionPort(side)!),
            //    _ => "",
            //}
            );

    public static DelegateColumn<IContent> ConnectionKind()
        => MakeConnectivityColumn(
            "Kind",
            true,
            c => c.ConnectionKind.ToString()
            );
}
