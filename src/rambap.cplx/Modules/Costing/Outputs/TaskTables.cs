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
            Iterator = new PartTypesIterator()
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
                Iterator = new ComponentIterator()
                {
                    PropertyIterator = (i) => i.Tasks()!= null ? [.. i.Tasks()!.RecurentTasks,.. i.Tasks()!.NonRecurentTasks] : [],
                    GroupPNsAtSameLocation = true,
                    StackPropertiesSingleChildBranches = true,
                },
                Columns = [
                    IDColumns.ComponentNumberPrettyTree(pc => (pc.Property is InstanceTasks.NamedTask task) ? task.Name : "!"),
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