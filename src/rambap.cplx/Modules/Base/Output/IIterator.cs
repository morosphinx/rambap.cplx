using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Define an iteration from a Pinstance producing contents for a <see cref="TableProducer{T}"/>
/// </summary>
/// <typeparam name="T">Type of item produced during iteration</typeparam>
public interface IContentIterator
{
    IEnumerable<IContent> MakeContent(Component component);

    bool ShouldRecurse(IContent content);
    IEnumerable<IContent> MakeSubContent(IContent content);
}


