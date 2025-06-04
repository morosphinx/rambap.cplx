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
            StackPropertiesSingleChildBranches = false, 
            DocumentationPerimeter = perimeter ?? new DocumentationPerimeter_SinglePartAndItsContents(),
        };
        ContentTransform = cs
            => cs.Where(c => c is not IPureComponentContent); // Remove branch items
        Columns = [
            // 
            // The link end on the connector, on the unexposed pin connector
            LinkedComponent(PortSide.Left,PortIdentity.UpperUsage,"CN", c => c.CN),
            // The endpoint component is the connector container, that expose its port
            EndpointPort(PortSide.Left,"Connector", p => p.Label),
            // A connector C01, that has an exposed connectable port J01
            //    J01 is the endpoint identity, is part of the * component. The Endpoint component is *
            //    Wireable / pin of the connector are not exposed, and thus are upperused by C01. The link endpoint id C01
            //
            LinkedPort(PortSide.Left,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            LinkedComponent(PortSide.Rigth,PortIdentity.UpperUsage,"CN", c => c.CN),
            EndpointPort(PortSide.Rigth,"Connector", p => p.Label),
            LinkedPort(PortSide.Rigth,PortIdentity.UpperExposition,"Pin",p => p.FullDefinitionName()),
            Dashes("--"),
            // MakeConnectivityColumn("Signal", false, c => c.GetLikelySignal()), // Disabled, only 
        ];
    }
}
