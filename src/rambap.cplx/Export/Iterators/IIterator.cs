using rambap.cplx.Core;

namespace rambap.cplx.Export.Iterators;

 /// <summary>
 /// Define an iteration from a Pinstance producing contents for a <see cref="Table{T}"/>
 /// </summary>
 /// <typeparam name="T">Type of item produced during iteration</typeparam>
 public interface IIterator<T>
 {
     public IEnumerable<T> MakeContent(Pinstance content);
 }


