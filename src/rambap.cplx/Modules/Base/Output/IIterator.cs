using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Define an iteration from a Pinstance producing contents for a <see cref="TableProducer{T}"/>
/// </summary>
/// <typeparam name="T">Type of item produced during iteration</typeparam>
public interface IContentIterator<out T>
    where T : IContent
{
    IEnumerable<T> MakeContent(Component component);
    IEnumerable<T> MakeContent_AsFlat(Component component)
    {
        var componentsItems = MakeContent(component);
        return (IEnumerable<T>) componentsItems.SelectMany(c => c.AsFlatContent());
    }

    bool ShouldTryRecurse(IContent content);
    IEnumerable<T> MakeSubContent(IContent content);
}


class DoNothingIterator : IContentIterator<IContent>
{
    public IEnumerable<IContent> MakeContent(Component component) => [];

    public IEnumerable<IContent> MakeSubContent(IContent content) => [];

    public bool ShouldTryRecurse(IContent content) => false;
}