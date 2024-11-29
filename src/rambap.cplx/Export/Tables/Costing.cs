using rambap.cplx.Concepts;
using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Tables;

public static class Costing
{
    /// <summary>
    /// Enumerate the instance native costs, OR return a single 0 if there is no cost.
    /// Used to display costless part in bill of materials
    /// We want to be aware if something cost 0.0 in those
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private static IEnumerable<object> ListCostOr0(Core.Pinstance i)
    {
        if (i.Cost() != null)
        {
            var nativeCosts = i.Cost()!.NativeCosts;
            if (nativeCosts.Count() == 0)
                return [new InstanceCost.NativeCostInfo("", 0)];
            else
                return nativeCosts;
        }
        else 
            return [new InstanceCost.NativeCostInfo("", 0)];
    }

    public static Table<PartTreeItem> BillOfMaterial_CompleteTree()
        => new()
        {
            Tree = new PartTree()
            {
                WriteBranches = false,
                PropertyIterator = ListCostOr0
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

    public static Table<PartTreeItem> BillOfMaterial_Flat()
        => new()
        {
            Tree = new PartTree()
            {
                WriteBranches = false,
                RecursionCondition = (c, l) => false, // single level
                PropertyIterator = ListCostOr0
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

    public static Table<PartTreeItem> BillOfTasks()
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

