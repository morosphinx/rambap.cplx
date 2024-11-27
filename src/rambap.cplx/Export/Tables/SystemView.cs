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
                ComponentTreeCommons.PartNumber(),
                ComponentTreeCommons.ComponentComment(),
            ],
        };
}

