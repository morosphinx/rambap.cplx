using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

/// <summary>
/// Table detailing the amount and duration of each individual Recurent Task. <br/>
/// </summary>
public record class TaskBreakdown : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public TaskBreakdown()
    {
        Iterator = new ComponentPropertyIterator<InstanceTasks.NamedTask>()
        {
            PropertyIterator = (c) => c.Instance.Tasks() is not null and var t ? [.. t.RecurentTasks, .. t.NonRecurentTasks] : [],
            GroupPNsAtSameLocation = true,
            StackPropertiesSingleChildBranches = true,
        };
        Columns = [
            IDColumns.ComponentNumberPrettyTree<InstanceTasks.NamedTask>(pc => pc.Property.Name),
            TaskColumns.LocalRecurentSum(),
            TaskColumns.LocalNonRecurentTotal(),
            IDColumns.ComponentID(),
            IDColumns.PartNumber(),
            TaskColumns.TaskName(),
            TaskColumns.TaskCategory(),
            TaskColumns.TaskRecurence(),
            TaskColumns.TaskDuration(),
            TaskColumns.TaskCount(),
        ];
    }
}