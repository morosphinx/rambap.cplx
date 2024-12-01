using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Concepts.InstanceTasks;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Concepts;

public class InstanceTasks : IInstanceConceptProperty
{
    public record NamedTask(bool IsRecurent, string Name, decimal Duration_day, string Category) { }

    public IEnumerable<NamedTask> NonRecurentTasks => nonRecurentTasks;
    internal List<NamedTask> nonRecurentTasks { private get; init; } = new();

    public IEnumerable<NamedTask> RecurentTasks => recurentTasks;
    internal List<NamedTask> recurentTasks { private get; init; } = new();

    public decimal NativeRecurentTaskDuration => RecurentTasks.Select(t => t.Duration_day).Sum();
    public required decimal ComposedRecurentTaskDuration { get; init; }
    public decimal TotalRecurentTaskDuration =>
        NativeRecurentTaskDuration + ComposedRecurentTaskDuration;
    
}

internal class TasksConcept : IConcept<InstanceTasks>
{
    public override InstanceTasks? Make(Pinstance i, Part template)
    {
        List<NamedTask> nonRecurrentTasks = new();
        ScanObjectContentFor<NonRecurrentTask>(template,
            (t, i) => nonRecurrentTasks.Add(new(false, i.Name, t.Duration_day, t.Category)));

        List<NamedTask> recurrentTasks = new();
        ScanObjectContentFor<RecurrentTask>(template,
            (t, i) => recurrentTasks.Add(new(true, i.Name, t.Duration_day, t.Category)));

        decimal totalComposedRecurentTask = i.Components.Select(c => c.Instance.Tasks()?.TotalRecurentTaskDuration ?? 0).Sum();

        bool hasAnyTask = nonRecurrentTasks.Count() > 0
            || recurrentTasks.Count() > 0
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