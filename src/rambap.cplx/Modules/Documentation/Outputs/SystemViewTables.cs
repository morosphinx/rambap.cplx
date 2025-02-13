using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class SystemViewTables
{
    public static TableProducer<IComponentContent> ComponentTree_Detailled()
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

    public static TableProducer<IComponentContent> ComponentTree_Stacked()
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

    public static TableProducer<IComponentContent> ComponentInventory()
        => new()
        {
            Iterator = new PartTypesIterator<object>()
            {
                WriteBranches = true,
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                DescriptionColumns.PartCommonName(hideIfEqualPN : true),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.GroupCIDs(),
                DescriptionColumns.PartLink(),
            ],
        };
}

