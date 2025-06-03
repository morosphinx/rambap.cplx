using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Base.Output.CommonColumns;
using static rambap.cplx.Modules.Connectivity.Outputs.WiringTableProperty;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplx.Export.CoreTables;

public record class WiringTable : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public WiringTable(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new ComponentPropertyIterator<WiringTableProperty>()
        {
            PropertyIterator = c => GetWiringTableProperty(c),
            WriteBranches = false,
            DocumentationPerimeter = perimeter ?? new DocumentationPerimeter_SinglePartAndItsContents(),
        };
        // TODO : Fix : there is currently no way to stop recursion on subcompoennt, and yet recurse
        // properties. (mosstly to ensure costing table stay valid)
        // as a result, subcomponent (the wires !) get included, and displayed
        // They polute display.
        ContentTransform = cs => cs.Where(c => c is not IPureComponentContent
            && ! (c is ILeafContent e && e.IsLeafBecause == LeafCause.RecursionBreak ));
        Columns = [
            ConnectedComponent(PortSide.Left,PortIdentity.UpperUsage,"CN", c => c.CN),
            ConnectedStructuralEquivalenceTopmostPort(PortSide.Left,"Connector", p => p.Label),
            ConnectedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            ConnectedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CN", c => c.CN),
            ConnectedStructuralEquivalenceTopmostPort(PortSide.Rigth,"Connector", p => p.Label),
            ConnectedPort(PortSide.Rigth,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            // MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()), // Disabled, only 
        ];
    }
}
