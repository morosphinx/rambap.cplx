using rambap.cplx.Modules.Costing;
using rambap.cplx.Core;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Mass;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class TaskColumns
{
    public static DelegateColumn<IContent> TaskName()
        => new DelegateColumn<IContent>("Task Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Name,
                IContent lc when lc.IsLeaf && lc.IsLeafBecause == LeafCause.RecursionBreak => "unit",
                IContent => "",
            });

    public static DelegateColumn<IContent> TaskCategory()
        => new DelegateColumn<IContent>("Task Category", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Category,
                IContent c => "",
            });

    public static DelegateColumn<IContent> TaskRecurence()
        => new DelegateColumn<IContent>("R", ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.IsRecurent ? "*" : "",
                IContent lc when lc.IsLeaf => "?",
                // TODO : clarify LeafComponentBehavior, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                IContent bc => "",
            });

    public static DelegateColumn<IContent> TaskDuration()
        => new DelegateColumn<IContent>("Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Duration_day.ToString(),
                IContent c => "",
            });

    public static DelegateColumn<IContent> RecurentTaskUnitDuration()
        => new DelegateColumn<IContent>("Recurent Unit Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp => lp.Property.Duration_day.ToString(),
                IContent lc when lc.IsLeaf=> lc.Component.Instance.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "",
                IContent bc => "",
            });

    public static DelegateColumn<IContent> TaskCount()
        => new DelegateColumn<IContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp =>
                    lp.Property.IsRecurent ? i.ComponentTotalCount.ToString() : "",
                IContent c => "",
            });

    public static DelegateColumn<IContent> TaskTotalDuration(bool includeNonRecurent)
        => new DelegateColumn<IContent>("Task Total Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when lp.Property.IsRecurent =>
                    (lp.ComponentTotalCount * lp.Property.Duration_day).ToString(),
                IPropertyContent<InstanceTasks.NamedTask> lp when ! lp.Property.IsRecurent =>
                    lp.Property.Duration_day.ToString(),
                IContent lc when lc.IsLeaf =>
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
                IContent bc when ! includeNonRecurent => "", // Branch component have no property
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

    public static IColumn<IContent> LocalRecurentSum()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "RecurentTaskSum",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when lp.Property.IsRecurent =>
                        lp.Property.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                IContent when i.Component.Instance.Tasks() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString()}"
                        : i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString(),
                _ => "",
            }
        };

    public static IColumn<IContent> LocalNonRecurentTotal()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "NonRecurentTaskBreakdown",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceTasks.NamedTask> lp when ! lp.Property.IsRecurent =>
                    lp.Property.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                IContent =>
                    i.Location.Depth == 0  || i.Component.Instance.Tasks() != null 
                        ? InstanceTasks.GetTotalNonRecurentTaskDurations(i.Component).ToString()
                        : "",
            }
        };
}

