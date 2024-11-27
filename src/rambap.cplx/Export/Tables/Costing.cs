using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Tables;

public static class Costing
{
    public static Table<PartTtreeItem> BillOfMaterial_CompleteTree()
        => new()
        {
            Tree = new PartTree()
            {
                WriteBranches = false,
                PropertyIterator = i => i.Cost()?.NativeCosts ?? new()
            },
            Columns = [
                PartTreeCommons.GroupNumber(),
                PartTreeCommons.GroupPN(),
                PartTreeCommons.GroupCNs(),
                Documentations.GroupDescription(),
                PartTreeCommons.GroupCount(),
                Costs.Group_CostName(),
                Costs.Group_UnitCost(),
                Costs.GroupTotalCost(),
            ],
        };

    public static Table<PartTtreeItem> BillOfMaterial_Flat()
        => new()
        {
            Tree = new PartTree()
            {
                WriteBranches = false,
                RecursionCondition = (c, l) => false, // single level
                PropertyIterator = i => i.Cost()?.NativeCosts ?? new()
            },
            Columns = [
                PartTreeCommons.GroupNumber(),
                PartTreeCommons.GroupPN(),
                PartTreeCommons.GroupCNs(),
                Documentations.GroupDescription(),
                PartTreeCommons.GroupCount(),
                Costs.Group_CostName(),
                Costs.Group_UnitCost(),
                Costs.GroupTotalCost(),
            ],
        };
    public static Table<ComponentTreeItem> CostBreakdown()
        => new()
        {
            Tree = new ComponentTree()
            {
                WriteBranches = false,
                PropertyIterator =
                (i) => i.Cost()?.NativeCosts ?? new(),
            },
            Columns = [
                ComponentTreeCommons.ComponentID(),
                ComponentTreeCommons.PartNumber(),
                Costs.CostBreakdown_Name(),
                Costs.CostBreakdown_Value(),
            ],
        };

    public static Table<PartTtreeItem> BillOfTasks()
    => new()
    {
        Tree = new PartTree()
        {
            WriteBranches = true,
            PropertyIterator =
            (i) =>
            {
                var tasks = i.Tasks();
                if (tasks != null)
                {
                    return [.. tasks.RecurentTasks, .. tasks.NonRecurentTasks];
                }
                else return [];
            }
        },
        Columns = [
            PartTreeCommons.GroupNumber(),
            PartTreeCommons.GroupPN(),
            PartTreeCommons.GroupCNs(),
            Documentations.GroupDescription(),
            PartTreeCommons.GroupCount(),
            Tasks.TaskName(),
            Tasks.TaskType(),
            Tasks.TaskTotalDuration(),
        ],

    };
}

