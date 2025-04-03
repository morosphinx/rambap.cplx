using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

/// <summary>
/// Define an iteration from a Pinstance producing contents for a <see cref="TableProducer{T}"/>
/// </summary>
/// <typeparam name="T">Type of item produced during iteration</typeparam>
public interface IIterator<out T>
{
    public IEnumerable<T> MakeContent(Pinstance instance);
}


