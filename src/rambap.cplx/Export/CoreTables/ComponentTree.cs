using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Documentation.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

public record class ComponentTree_Detailled : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public ComponentTree_Detailled()
    {
        Iterator = new ComponentIterator();
        Columns = [
            IDColumns.ComponentNumberPrettyTree(),
            IDColumns.PartNumber(),
            CommonColumns.ComponentComment(),
            DescriptionColumns.PartDescription(),
        ];
    }
}

public record class ComponentTree_Stacked : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public ComponentTree_Stacked()
    {
        Iterator = new ComponentIterator()
        {
            GroupPNsAtSameLocation = true,
        };
        Columns = [
            IDColumns.ComponentNumberPrettyTree(),
            CommonColumns.ComponentTotalCount(displayBranches : true),
            IDColumns.GroupCNs(),
            IDColumns.PartNumber(),
            DescriptionColumns.PartDescription(),
        ];
    }
}

