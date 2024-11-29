using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Tables;

public static class SystemView
{
    public static Table<ComponentTreeItem> ComponentTree()
        => new()
        {
            Tree = new ComponentTree(),
            Columns = [
                ComponentTreeCommons.ComponentPrettyTree(),
                ComponentTreeCommons.ComponentComment(),
                ComponentTreeCommons.PartNumber(),
                Documentations.PartDescription(),
            ],
        };

    public static Table<PartTreeItem> ComponentInventory()
        => new()
        {
            Tree = new PartTree()
            {
                WriteBranches = true,
            },
            Columns = [
                PartTreeCommons.GroupNumber(),
                PartTreeCommons.GroupPN(),
                PartTreeCommons.GroupCNs(),
                Documentations.GroupDescription(),
                PartTreeCommons.GroupCount(),
            ],
        };
}

