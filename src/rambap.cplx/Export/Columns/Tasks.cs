﻿using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class Tasks
{
    public static DelegateColumn<ComponentTreeItem> RecurentTaskName()
    => new DelegateColumn<ComponentTreeItem>("Task Name", ColumnTypeHint.Numeric,
        i =>
        {
            if (i is LeafProperty lp)
            {
                if (lp.Property is Concepts.InstanceTasks.NamedTask n)
                    return n.Name;
                else
                    throw new NotImplementedException();
            }
            else if (i is LeafComponent lc)
            {
                return "unit";
            }
            return "";
        });

    public static DelegateColumn<ComponentTreeItem> RecurentTaskDuration()
        => new DelegateColumn<ComponentTreeItem>("Task Total Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lp)
                {
                    if (lp.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Duration_day.ToString();
                    else
                        throw new NotImplementedException();
                }
                else if(i is LeafComponent lc)
                {
                    return lc.Component.Instance.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "";
                }
                return "";
            });


    public static DelegateColumn<PartTreeItem> TaskName()
        => new DelegateColumn<PartTreeItem>("Task Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Name;
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafPartTableItem lc)
                {
                    return "unit";
                }
                return "";
            });


    public static DelegateColumn<PartTreeItem> TaskType()
        => new DelegateColumn<PartTreeItem>("Task Type", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.IsRecurent ? "Recurrent" : "Nonrec.";
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafPartTableItem lc)
                {
                    return "Recurrent (sum)";
                }
                return "";
            });

    public static DelegateColumn<PartTreeItem> TaskCategory()
        => new DelegateColumn<PartTreeItem>("Task Category", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Category;
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<PartTreeItem> TaskTotalDuration()
        => new DelegateColumn<PartTreeItem>("Task Total Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        var total_duration = (n.IsRecurent ? lpi.Items.Count() : 1) * n.Duration_day;
                        return $"{total_duration}";
                    }
                    else
                        throw new NotImplementedException();
                }
                else if(i is LeafPartTableItem lc)
                {
                    // 
                    var intance = lc.PrimaryItem.Component.Instance;
                    var primatryTaskData = intance.Tasks();
                    if (primatryTaskData == null) return "";

                    // RecurentData
                    var recurrentDurationPerCom = primatryTaskData.TotalRecurentTaskDuration;
                    var recurrentDurationTotal = recurrentDurationPerCom * lc.Items.Count();

                    // Non RecurentData
                    var nonRecurentTotal = primatryTaskData.NativeNonRecurentTaskDuration + Concepts.InstanceTasks.GetInheritedRecurentCosts(intance);

                    // Total
                    var totalDuration = recurrentDurationTotal + nonRecurentTotal;
                    return totalDuration.ToString();
                } else if(i is BranchPartTableItem lb)
                {
                    throw new InvalidOperationException();
                }
                return "";
            });
}

