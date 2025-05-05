using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionTableProperty;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplx.Export.CoreTables;

public record class WiringTable : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public  WiringTable(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new ComponentPropertyIterator<ConnectionTableProperty>()
        {
            PropertyIterator = c => GetConnectivityTableProperties(
                c, ConnectionCategory.Wiring),
            WriteBranches = false,
            DocumentationPerimeter = perimeter ?? new(),
        };
        ContentTransform = cs => cs.Where(c => c is not IPureComponentContent);
        Columns = [
            ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CN", c => c.CN),
            ConnectedStructuralEquivalenceTopmostPort(PortSide.Left,"Connector", p => p.Label),
            ConnectedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CN", c => c.CN),
            ConnectedStructuralEquivalenceTopmostPort(PortSide.Rigth,"Connector", p => p.Label),
            ConnectedPort(PortSide.Rigth,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()),
        ];
    }
}
