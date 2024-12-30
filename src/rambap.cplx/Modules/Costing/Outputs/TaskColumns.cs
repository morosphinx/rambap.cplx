using rambap.cplx.Modules.Costing;
using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class TaskColumns
{
    public static DelegateColumn<ComponentContent> RecurentTaskName()
    => new DelegateColumn<ComponentContent>("Task Name", ColumnTypeHint.String,
        i =>
        {
            if (i is LeafProperty lp)
            {
                if (lp.Property is InstanceTasks.NamedTask n)
                    return n.Name;
                else
                    throw new NotImplementedException();
            }
            else if (i is LeafComponent lc)
            {
                if (lc.IsLeafBecause == LeafCause.RecursionBreak)
                    return "unit";
                else
                    return "";
            }
            return "";
        });

    public static DelegateColumn<ComponentContent> RecurentTaskCategory()
    => new DelegateColumn<ComponentContent>("Task Category", ColumnTypeHint.String,
        i =>
        {
            if (i is LeafProperty lp)
            {
                if (lp.Property is InstanceTasks.NamedTask n)
                    return n.Category;
                else
                    throw new NotImplementedException();
            }
            else return "";
        });

    public static DelegateColumn<ComponentContent> RecurentTaskDuration()
        => new DelegateColumn<ComponentContent>("Recurent Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lp)
                {
                    if (lp.Property is InstanceTasks.NamedTask n)
                        return n.Duration_day.ToString();
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafComponent lc)
                {
                    return lc.Component.Instance.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "";
                }
                return "";
            },
            i =>
            {
                return i.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "";
            });


    public static DelegateColumn<ComponentContent> TaskName()
        => new DelegateColumn<ComponentContent>("Task Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
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


    public static DelegateColumn<ComponentContent> TaskRecurence()
        => new DelegateColumn<ComponentContent>("R", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
                        return n.IsRecurent ? "*" : "";
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafComponent lc)
                {
                    // TODO : clarofy, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                    throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<ComponentContent> TaskCategory()
        => new DelegateColumn<ComponentContent>("Task Category", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
                        return n.Category;
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<ComponentContent> TaskDuration()
        => new DelegateColumn<ComponentContent>("Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
                        return n.Duration_day.ToString();
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<ComponentContent> TaskCount()
        => new DelegateColumn<ComponentContent>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
                        return n.IsRecurent ? i.ComponentTotalCount.ToString() : "";
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<ComponentContent> TaskTotalDuration(bool includeNonRecurent)
        => new DelegateColumn<ComponentContent>("Task Total Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceTasks.NamedTask n)
                    {
                        var total_duration = (n.IsRecurent ? lpi.ComponentTotalCount : 1) * n.Duration_day;
                        return $"{total_duration}";
                    }
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafComponent lc)
                {
                    // 
                    var intance = lc.Component.Instance;
                    var primatryTaskData = intance.Tasks();
                    if (primatryTaskData == null) return "";

                    // RecurentData
                    var recurrentDurationPerCom = primatryTaskData.TotalRecurentTaskDuration;
                    var recurrentDurationTotal = recurrentDurationPerCom * lc.ComponentTotalCount;

                    decimal totalDuration = recurrentDurationTotal;
                    if (includeNonRecurent)
                    {
                        // Non RecurentData
                        var nonRecurentTotal = primatryTaskData.NativeNonRecurentTaskDuration + InstanceTasks.GetInheritedRecurentCosts(intance);

                        totalDuration += nonRecurentTotal;
                    }
                    return totalDuration.ToString();
                }
                else if (includeNonRecurent && i is BranchComponent lb)
                {
                    throw new NotSupportedException("This columns display a mix of intensive (NonRecurentTask) and extensive (RecurentTask) properties. Calculations have caveats, and are disabled.");
                }
                return "";
            });
}

