using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class SystemViewTables
{
    public static Table<ComponentContent> ComponentTree()
        => new()
        {
            Iterator = new ComponentContentTree(),
            Columns = [
                ComponentContentColumns.ComponentPrettyTree(),
                ComponentContentColumns.ComponentComment(),
                ComponentContentColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static Table<PartContent> ComponentInventory()
        => new()
        {
            Iterator = new PartContentList()
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

