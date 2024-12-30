using rambap.cplx.Core;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export.Iterators;

/// <summary>
/// Define an iteration from a Pinstance producing contents for a <see cref="TableProducer{T}"/>
/// </summary>
/// <typeparam name="T">Type of item produced during iteration</typeparam>
public interface IIterator<T>
 {
     public IEnumerable<T> MakeContent(Pinstance content);
 }


