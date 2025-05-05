using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using rambap.cplx.Modules.Documentation.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;


/// <summary>
/// Table listing the amount and duration of all tasks kind in the instance
/// </summary>
public record class BillOfTasks : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public BillOfTasks()
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
        };
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
        ];
    }
}
