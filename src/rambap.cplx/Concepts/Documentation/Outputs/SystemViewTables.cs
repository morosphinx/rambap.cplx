using rambap.cplx.Export;
using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Concepts.Documentation.Outputs;

public static class SystemViewTables
{
    public static Table<ComponentContent> ComponentTree()
        => new()
        {
            Tree = new ComponentContentTree(),
            Columns = [
                ComponentTreeCommons.ComponentPrettyTree(),
                ComponentTreeCommons.ComponentComment(),
                ComponentTreeCommons.PartNumber(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static Table<PartContent> ComponentInventory()
        => new()
        {
            Tree = new PartContentList()
            {
                WriteBranches = true,
            },
            Columns = [
                PartTreeCommons.GroupNumber(),
                PartTreeCommons.GroupPN(),
                PartTreeCommons.GroupCNs(),
                DescriptionColumns.GroupDescription(),
                PartTreeCommons.GroupCount(),
            ],
        };
}

