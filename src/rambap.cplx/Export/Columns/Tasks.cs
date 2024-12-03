using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class Tasks
{

    public static DelegateColumn<ComponentContent> RecurentTaskName()
    => new DelegateColumn<ComponentContent>("Task Name", ColumnTypeHint.String,
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
                if (lp.Property is Concepts.InstanceTasks.NamedTask n)
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
            },
            i =>
            {
                return i.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "";
            });


    public static DelegateColumn<PartContent> TaskName()
        => new DelegateColumn<PartContent>("Task Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Name;
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafPartContent lc)
                {
                    return "unit";
                }
                return "";
            });


    public static DelegateColumn<PartContent> TaskRecurence()
        => new DelegateColumn<PartContent>("R", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.IsRecurent ? "*" : "";
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafPartContent lc)
                {
                    // TODO : clarofy, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                    throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<PartContent> TaskCategory()
        => new DelegateColumn<PartContent>("Task Category", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Category;
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<PartContent> TaskDuration()
        => new DelegateColumn<PartContent>("Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.Duration_day.ToString();
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<PartContent> TaskCount()
        => new DelegateColumn<PartContent>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                        return n.IsRecurent ? i.Items.Count().ToString() : "";
                    else
                        throw new NotImplementedException();
                }
                return "";
            });

    public static DelegateColumn<PartContent> TaskTotalDuration(bool includeNonRecurent)
        => new DelegateColumn<PartContent>("Task Total Duration", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPropertyPartContent lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        var total_duration = (n.IsRecurent ? lpi.Items.Count() : 1) * n.Duration_day;
                        return $"{total_duration}";
                    }
                    else
                        throw new NotImplementedException();
                }
                else if(i is LeafPartContent lc)
                {
                    // 
                    var intance = lc.PrimaryItem.Component.Instance;
                    var primatryTaskData = intance.Tasks();
                    if (primatryTaskData == null) return "";

                    // RecurentData
                    var recurrentDurationPerCom = primatryTaskData.TotalRecurentTaskDuration;
                    var recurrentDurationTotal = recurrentDurationPerCom * lc.Items.Count();

                    decimal totalDuration = recurrentDurationTotal;
                    if (includeNonRecurent)
                    {
                        // Non RecurentData
                        var nonRecurentTotal = primatryTaskData.NativeNonRecurentTaskDuration + Concepts.InstanceTasks.GetInheritedRecurentCosts(intance);

                        totalDuration += nonRecurentTotal;
                    }
                    return totalDuration.ToString();
                } else if(includeNonRecurent && i is BranchPartContent lb)
                {
                    throw new NotSupportedException("This columns display a mix of intensive (NonRecurentTask) and extensive (RecurentTask) properties. Calculations have caveats, and are disabled.");
                }
                return "";
            });
}

