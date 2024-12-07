﻿using rambap.cplx.Concepts;
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

    /// <summary>
    /// Table listing the amount and cost of each part kind in the instance
    /// </summary>
    /// <param name="recurse">If true, the entire component tree is returned. <br/>
    /// If false, only the immediate components are returned.</param>
    public static Table<PartContent> BillOfMaterial(bool recurse = true)
        => new()
        {
            Tree = new PartContentList()
            {
                WriteBranches = false,
                RecursionCondition = recurse ? null : (c, l) => false, // null = always recurse
                PropertyIterator = ListCostOr0
            },
            Columns = [
                PartTreeCommons.GroupNumber(),
                PartTreeCommons.GroupPN(),
                Documentations.GroupDescription(),
                Costs.Group_CostName(),
                Costs.Group_UnitCost(),
                PartTreeCommons.GroupCount(),
                Costs.GroupTotalCost(),
            ],
        };

    /// <summary>
    /// Table detailing the amount and duration of each individual Cost of the instance.
    /// </summary>
    public static Table<ComponentContent> CostBreakdown()
        => new()
        {
            Tree = new ComponentContentTree()
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


    /// <summary>
    /// Table listing the amount and duration of all tasks kind in the instance
    /// </summary>
    public static Table<PartContent> BillOfTasks()
    => new()
    {
        Tree = new PartContentList()
        {
            WriteBranches = false,
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
            Documentations.GroupDescription(),
            Tasks.TaskName(),
            Tasks.TaskCategory(),
            Tasks.TaskDuration(),
            Tasks.TaskRecurence(),
            Tasks.TaskCount(),
            Tasks.TaskTotalDuration(includeNonRecurent: true),
        ],

    };


    /// <summary>
    /// Table detailing the amount and duration of each individual Recurent Task. <br/>
    /// This does NOT list NonRecurent Task, due to NonRecurent Task begin an intensive property.
    /// </summary>
    public static Table<ComponentContent> RecurentTaskBreakdown()
        => new()
        {
            Tree = new ComponentContentTree()
            {
                WriteBranches = false,
                PropertyIterator =
                (i) => i.Tasks()?.RecurentTasks ?? [],
            },
            Columns = [
                ComponentTreeCommons.ComponentID(),
                ComponentTreeCommons.PartNumber(),
                Tasks.RecurentTaskName(),
                Tasks.RecurentTaskCategory(),
                Tasks.RecurentTaskDuration(),
            ],
        };
}

