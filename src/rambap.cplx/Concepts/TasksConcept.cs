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
            (t, i) => recurrentTasks.Add(new(false, i.Name, t.Duration_day, t.Category)));

        bool hasAnyTask = nonRecurrentTasks.Count() > 0 || recurrentTasks.Count() > 0;
        if (!hasAnyTask) return null;
        else return new InstanceTasks()
        {
            nonRecurentTasks = nonRecurrentTasks,
            recurentTasks = recurrentTasks
        };
    }
}