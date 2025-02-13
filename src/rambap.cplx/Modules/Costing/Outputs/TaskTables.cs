using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;

namespace rambap.cplx.Modules.Costing.Outputs
{
    public static class TaskTables
    {
        /// <summary>
        /// Table listing the amount and duration of all tasks kind in the instance
        /// </summary>
        public static TableProducer<IComponentContent> BillOfTasks()
        => new()
        {
            Iterator = new PartTypesIterator<InstanceTasks.NamedTask>()
            {
                WriteBranches = false,
                PropertyIterator =
                (i) =>
                {
                    var tasks = i.Instance.Tasks();
                    if (tasks != null)
                    {
                        return [.. tasks.RecurentTasks, .. tasks.NonRecurentTasks];
                    }
                    else return [];
                }
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
                TaskColumns.TaskName(),
                TaskColumns.TaskCategory(),
                TaskColumns.TaskDuration(),
                TaskColumns.TaskRecurence(),
                TaskColumns.TaskCount(),
                TaskColumns.TaskTotalDuration(includeNonRecurent: true),
            ],

        };


        /// <summary>
        /// Table detailing the amount and duration of each individual Recurent Task. <br/>
        /// </summary>
        public static TableProducer<IComponentContent> TaskBreakdown()
            => new()
            {
                Iterator = new ComponentPropertyIterator<InstanceTasks.NamedTask>()
                {
                    PropertyIterator = (c) => c.Instance.Tasks() is not null and var t ? [.. t.RecurentTasks,.. t.NonRecurentTasks] : [],
                    GroupPNsAtSameLocation = true,
                    StackPropertiesSingleChildBranches = true,
                },
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
                ],
            };
    }
}