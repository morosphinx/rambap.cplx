using rambap.cplx.Modules.Costing;
using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class TaskColumns
{
    public static DelegateColumn<IComponentContent> RecurentTaskName()
        => new DelegateColumn<IComponentContent>("Task Name", ColumnTypeHint.String,
            i => i switch
            {
                IPropertyContent { Property : InstanceTasks.NamedTask  prop} lp => prop.Name,
                LeafComponent lc when lc.IsLeafBecause == LeafCause.RecursionBreak => "unit",
                LeafComponent lc when lc.IsLeafBecause != LeafCause.RecursionBreak => "",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> RecurentTaskCategory()
        => new DelegateColumn<IComponentContent>("Task Category", ColumnTypeHint.String,
            i => i switch 
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } lp => prop.Category,
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


    public static DelegateColumn<IComponentContent> TaskName()
        => new DelegateColumn<IComponentContent>("Task Name", ColumnTypeHint.String,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.Name,
                LeafComponent lc => "unit",
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });


    public static DelegateColumn<IComponentContent> TaskRecurence()
        => new DelegateColumn<IComponentContent>("R", ColumnTypeHint.String,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.IsRecurent ? "*" : "",
                LeafComponent lc => throw new NotImplementedException(),
                    // TODO : clarify LeafComponentBehavior, it's not possible to represent both NonRecurent and Recurent duration in the same total unambigiously
                BranchComponent bc => "",
                _ => throw new NotImplementedException()
            });

    public static DelegateColumn<IComponentContent> TaskCategory()
        => new DelegateColumn<IComponentContent>("Task Category", ColumnTypeHint.String,
            i => i switch
            {
                IPropertyContent { Property: InstanceTasks.NamedTask prop } => prop.Category,
                LeafComponent lc => "",
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
                        var totalNonRecurentDuration = instanceTasks.NativeRecurentTaskDuration + InstanceTasks.GetInheritedRecurentCosts(c.Instance);
                        return includeNonRecurent
                            ? totalRecurentDuration + totalNonRecurentDuration
                            : totalRecurentDuration;
                    }, out var value)
                        ? value.ToString()
                        : "error",
                BranchComponent bc when includeNonRecurent=>
                    throw new NotSupportedException("This columns display a mix of intensive (NonRecurentTask) and extensive (RecurentTask) properties. Calculations have caveats, and are disabled."),
                BranchComponent bc when ! includeNonRecurent => "",
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
}

