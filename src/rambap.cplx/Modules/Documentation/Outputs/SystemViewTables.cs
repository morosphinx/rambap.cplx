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
                ComponentContentColumns.ComponentPrettyTree(),
                ComponentContentColumns.ComponentComment(),
                ComponentContentColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static Table<PartContent> ComponentTree_Stacked()
       => new()
       {
           Iterator = new PartLocationIterator(),
           Columns = [
                PartContentColumns.MainComponentInfo(ComponentContentColumns.ComponentPrettyTree()),
                PartContentColumns.GroupCount(),
                PartContentColumns.MainComponentInfo(ComponentContentColumns.ComponentComment()),
                PartContentColumns.MainComponentInfo(ComponentContentColumns.PartNumber()),
                PartContentColumns.MainComponentInfo(DescriptionColumns.PartDescription()),
           ],
       };

    public static Table<PartContent> ComponentInventory()
        => new()
        {
            Iterator = new PartTypesIterator()
            {
                WriteBranches = true,
            },
            Columns = [
                PartContentColumns.GroupNumber(),
                PartContentColumns.GroupPN(),
                PartContentColumns.GroupCNs(),
                DescriptionColumns.GroupDescription(),
                PartContentColumns.GroupCount(),
            ],
        };
}

