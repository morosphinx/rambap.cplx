using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Tables;

public static class SystemView
{
    public static Table<ComponentContent> ComponentTree()
        => new()
        {
            Tree = new ComponentContentTree(),
            Columns = [
                ComponentTreeCommons.ComponentPrettyTree(),
                ComponentTreeCommons.ComponentComment(),
                ComponentTreeCommons.PartNumber(),
                Documentations.PartDescription(),
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
                Documentations.GroupDescription(),
                PartTreeCommons.GroupCount(),
            ],
        };
}

