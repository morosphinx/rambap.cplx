using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Costing.Outputs
{
    public static class TaskTables
    {
        /// <summary>
        /// Table listing the amount and duration of all tasks kind in the instance
        /// </summary>
        public static TableProducer<ComponentContent> BillOfTasks()
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
                DescriptionColumns.GroupDescription(),
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
        /// This does NOT list NonRecurent Task, due to NonRecurent Task begin an intensive property.
        /// </summary>
        public static TableProducer<ComponentContent> RecurentTaskBreakdown()
            => new()
            {
                Iterator = new ComponentIterator()
                {
                    PropertyIterator = (i) => i.Tasks()?.RecurentTasks ?? [],
                    GroupPNsAtSameLocation = true,
                },
                Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                IDColumns.ComponentID(),
                IDColumns.PartNumber(),
                TaskColumns.RecurentTaskName(),
                TaskColumns.RecurentTaskCategory(),
                TaskColumns.RecurentTaskDuration(),
                ],
            };
    }
}