using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplx.Export.CoreTables;

public record class ConnectionTable : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public ConnectionTable(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new ComponentPropertyIterator<ConnectionTableProperty>()
        {
            PropertyIterator = c => GetConnectivityTableProperties(
                c, ConnectionCategory.Assembly),
            WriteBranches = false,
            DocumentationPerimeter = perimeter ?? new(),
        };
        ContentTransform = cs => cs.Where(c => c is not IPureComponentContent);
        Columns = [
            MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()),
            Dashes("--"),
            ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CID", c => c.CID(" / ")),
            ConnectedPort(PortSide.Left,PortIdentity.UpperUsage,"Port", p => p.Label),
            Dashes("--"),
            CablePart("Cable",c => c.CN),
            Dashes("--"),
            ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CID", c => c.CID(" / ")),
            ConnectedPort(PortSide.Rigth,PortIdentity.UpperUsage,"Port", p => p.Label),
            Dashes("--"),
            CablePart("Cable",c => c.PN),
            CablePart("Description",c => c.Instance.Documentation()?.Descriptions.FirstOrDefault()?.Text ?? ""),
            CablePort(PortSide.Left, "L", c => c.GetLowerExposition().Owner.PN),
            CablePort(PortSide.Rigth, "R", c => c.GetLowerExposition().Owner.PN),
        ];
    }
}
