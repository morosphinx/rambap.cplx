using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Modules.Costing.InstanceTasks;
using static rambap.cplx.Core.Support;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Costing;

public class InstanceTasks : IInstanceConceptProperty
{
    public record NamedTask(bool IsRecurent, string Name, decimal Duration_day, string Category) { }

    public IEnumerable<NamedTask> NonRecurentTasks => nonRecurentTasks;
    internal List<NamedTask> nonRecurentTasks { private get; init; } = [];

    public decimal NativeNonRecurentTaskDuration => nonRecurentTasks.Select(t => t.Duration_day).Sum();

    public static decimal GetTotalNonRecurentTaskDurations(Component component)
    {
        decimal total = 0;
        var tree = new PartTypesIterator<object>();
        foreach (var i in tree.MakeContent(component))
        {
            var tasks = i.Component.Instance.Tasks();
            if (tasks != null)
            {
                total += tasks.NativeNonRecurentTaskDuration;
            }
        }
        return total;
    }

    public IEnumerable<NamedTask> RecurentTasks => recurentTasks;
    internal List<NamedTask> recurentTasks { private get; init; } = [];

    public decimal NativeRecurentTaskDuration => RecurentTasks.Select(t => t.Duration_day).Sum();
    public required decimal ComposedRecurentTaskDuration { get; init; }
    public decimal TotalRecurentTaskDuration =>
        NativeRecurentTaskDuration + ComposedRecurentTaskDuration;

}

internal class TasksConcept : IConcept<InstanceTasks>
{
    public override InstanceTasks? Make(Pinstance instance, IEnumerable<Component> subcomponents, Part template)
    {
        List<NamedTask> nonRecurrentTasks = [];
        ScanObjectContentFor<NonRecurrentTask>(template,
            (t, p) => nonRecurrentTasks.Add(new(false, p.Name, t.Duration_day, t.Category))
            );

        List<NamedTask> recurrentTasks = [];
        ScanObjectContentFor<RecurrentTask>(template,
            (t, p) => recurrentTasks.Add(new(true, p.Name, t.Duration_day, t.Category))
            );

        decimal totalComposedRecurentTask = subcomponents.Select(c => c.Instance.Tasks()?.TotalRecurentTaskDuration ?? 0).Sum();

        bool hasAnyTask = nonRecurrentTasks.Count > 0
            || recurrentTasks.Count > 0
            || totalComposedRecurentTask > 0;

        if (!hasAnyTask) return null;
        else return new InstanceTasks()
        {
            nonRecurentTasks = nonRecurrentTasks,
            recurentTasks = recurrentTasks,
            ComposedRecurentTaskDuration = totalComposedRecurentTask,
        };
    }
}