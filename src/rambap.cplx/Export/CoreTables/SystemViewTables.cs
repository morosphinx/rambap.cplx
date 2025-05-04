using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Documentation.Outputs;

namespace rambap.cplx.Export.CoreTables;

public static class SystemViewTables
{
    public static TableProducer<ICplxContent> ComponentTree_Detailled()
        => new()
        {
            Iterator = new ComponentIterator(),
            Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                IDColumns.PartNumber(),
                CommonColumns.ComponentComment(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static TableProducer<ICplxContent> ComponentTree_Stacked()
       => new()
       {
           Iterator = new ComponentIterator()
           {
               GroupPNsAtSameLocation = true,
           },
           Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                CommonColumns.ComponentTotalCount(displayBranches : true),
                IDColumns.GroupCNs(),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
           ],
       };

    public static TableProducer<ICplxContent> ComponentInventory(DocumentationPerimeter? perimeter = null)
        => new()
        {
            Iterator = new PartTypesIterator<object>()
            {
                WriteBranches = true,
                DocumentationPerimeter = perimeter ?? new(),
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.PartCommonName(usePnAsBackup : false),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.GroupCIDs(),
                DescriptionColumns.PartLink(),
            ],
        };
}

