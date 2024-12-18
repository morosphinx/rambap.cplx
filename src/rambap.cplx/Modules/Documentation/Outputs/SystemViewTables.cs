using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class SystemViewTables
{
    public static Table<ComponentContent> ComponentTree()
        => new()
        {
            Iterator = new ComponentIterator(),
            Columns = [
IDColumns.ComponentNumberPrettyTree(),
                CommonColumns.ComponentComment(),
IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static Table<ComponentContent> ComponentTree_Stacked()
       => new()
       {
           Iterator = new PartLocationIterator(),
           Columns = [
IDColumns.ComponentNumberPrettyTree(),
                CommonColumns.GroupCount(),
                CommonColumns.ComponentComment(),
IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
           ],
       };

    public static Table<ComponentContent> ComponentInventory()
        => new()
        {
            Iterator = new PartTypesIterator()
            {
                WriteBranches = true,
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.PartNumber(),
                IDColumns.GroupCIDs(),
                DescriptionColumns.GroupDescription(),
                CommonColumns.GroupCount(),
            ],
        };
}

