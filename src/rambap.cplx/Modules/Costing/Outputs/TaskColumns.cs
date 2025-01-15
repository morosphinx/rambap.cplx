using rambap.cplx.Modules.Costing;
using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Mass;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class TaskColumns
{
    public static DelegateColumn<IComponentContent> TaskName()
        => new DelegateColumn<IComponentContent>("Task Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent { Property : InstanceTasks.NamedTask  prop} lp => prop.Name,
                LeafComponent lc when lc.IsLeafBecause == LeafCause.RecursionBreak => "unit",
                LeafComponent lc when lc.IsLeafBecause != LeafCause.RecursionBreak => "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskCategory()
        => new DelegateColumn<IComponentContent>("Task Category", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.Category,
                LeafComponent lc => "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskRecurence()
        => new DelegateColumn<IComponentContent>("R", ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.IsRecurent ? "*" : "",
                LeafComponent lc => "?",
                    // TODO : clarify LeafComponentBehavior, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskDuration()
        => new DelegateColumn<IComponentContent>("Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.Duration_day.ToString(),
                LeafComponent lc => "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> RecurentTaskUnitDuration()
        => new DelegateColumn<IComponentContent>("Recurent Unit Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } lp => prop.Duration_day.ToString(),
                LeafComponent lc => lc.Component.Instance.Tasks()?.TotalRecurentTaskDuration.ToString() ?? "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskCount()
        => new DelegateColumn<IComponentContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } =>
                    prop.IsRecurent ? i.ComponentTotalCount.ToString() : "",
                LeafComponent lc => "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskTotalDuration(bool includeNonRecurent)
        => new DelegateColumn<IComponentContent>("Task Total Duration", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } lp when prop.IsRecurent =>
                    (lp.ComponentTotalCount * prop.Duration_day).ToString(),
                IPropertyContent { Property: InstanceTasks.NamedTask prop } lp when ! prop.IsRecurent =>
                    prop.Duration_day.ToString(),
                LeafComponent lc =>
                    lc.AllComponentsMatch(c =>
                    {
                        var instanceTasks = c.Instance.Tasks();
                        if (instanceTasks  == null) return 0M;
                        var totalRecurentDuration = instanceTasks.TotalRecurentTaskDuration * lc.ComponentTotalCount;
                        var totalNonRecurentDuration = InstanceTasks.GetTotalNonRecurentTaskDurations(c.Instance);
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

    public static IColumn<IComponentContent> LocalRecurentSum()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "RecurentTaskSum",
            GetLocationText = i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask task } when task.IsRecurent =>
                        task.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                BranchComponent when i.Component.Instance.Tasks() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString()}"
                        : i.Component.Instance.Tasks()!.TotalRecurentTaskDuration.ToString(),
                _ => "",
            }
        };

    public static IColumn<IComponentContent> LocalNonRecurentTotal()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "NonRecurentTaskBreakdown",
            GetLocationText = i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask task } when ! task.IsRecurent=>
                    task.Duration_day.ToString(), // Do not display multiplicity for properties : this is a local duration representation
                BranchComponent =>
                    i.Location.Depth == 0  || i.Component.Instance.Tasks() != null 
                        ? InstanceTasks.GetTotalNonRecurentTaskDurations(i.Component.Instance).ToString()
                        : "",
                _ => ""
            }
        };
}

