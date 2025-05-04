using rambap.cplx.Modules.Costing;
using rambap.cplx.Core;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Mass;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class TaskColumns
{
    public static DelegateColumn<ICplxContent> TaskName()
        => new DelegateColumn<ICplxContent>("Task Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Name,
                LeafComponent lc when lc.IsLeafBecause == LeafCause.RecursionBreak => "unit",
                IPureComponentContent => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> TaskCategory()
        => new DelegateColumn<ICplxContent>("Task Category", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Category,
                IPureComponentContent c => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> TaskRecurence()
        => new DelegateColumn<ICplxContent>("R", ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.IsRecurent ? "*" : "",
                LeafComponent lc => "?",
                    // TODO : clarify LeafComponentBehavior, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> TaskDuration()
        => new DelegateColumn<ICplxContent>("Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Duration_day.ToString(),
                IPureComponentContent c => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> RecurentTaskUnitDuration()
        => new DelegateColumn<ICplxContent>("Recurent Unit Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Duration_day.ToString(),
                LeafComponent lc => lc.Component.Instance.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> TaskCount()
        => new DelegateColumn<ICplxContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp =>
                    lp.Property.IsRecurent ? i.ComponentTotalCount.ToString() : "",
                IPureComponentContent c => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<ICplxContent> TaskTotalDuration(bool includeNonRecurent)
        => new DelegateColumn<ICplxContent>("Task Total Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when lp.Property.IsRecurent =>
                    (lp.ComponentTotalCount * lp.Property.Duration_day).ToString(),
                IPropertyContent<InstanceTasks.NamedTask> lp when ! lp.Property.IsRecurent =>
                    lp.Property.Duration_day.ToString(),
                LeafComponent lc =>
                    lc.AllComponentsMatch(c =>
                    {
                        var instanceTasks = c.Instance.Tasks();
                        if (instanceTasks  == null) return 0M;
                        var totalRecurentDuration = instanceTasks.TotalRecurentTaskDuration * lc.ComponentTotalCount;
                        var totalNonRecurentDuration = InstanceTasks.GetTotalNonRecurentTaskDurations(c);
                        return includeNonRecurent
                            ? totalRecurentDuration + totalNonRecurentDuration
                            : totalRecurentDuration;
                    }, out var value)
                        ? value.ToString()
                        : "error",
                BranchComponent bc when ! includeNonRecurent => "", // Branch component have no property
                _ => throw new NotImplementedException()
            },
            i =>
            {
                // Return sum only when displaying sum of recurent task.
                // Ambiguity when mixing recurent and non recurrent tasks togethers
                if (!includeNonRecurent)
                    return i.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "";
                else return "";
            });

    public static IColumn<ICplxContent> LocalRecurentSum()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "RecurentTaskSum",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when lp.Property.IsRecurent =>
                        lp.Property.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                BranchComponent when i.Component.Instance.Tasks() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString()}"
                        : i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString(),
                _ => "",
            }
        };

    public static IColumn<ICplxContent> LocalNonRecurentTotal()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "NonRecurentTaskBreakdown",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when ! lp.Property.IsRecurent =>
                    lp.Property.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                BranchComponent =>
                    i.Location.Depth == 0  || i.Component.Instance.Tasks() != null 
                        ? InstanceTasks.GetTotalNonRecurentTaskDurations(i.Component).ToString()
                        : "",
                _ => ""
            }
        };
}

