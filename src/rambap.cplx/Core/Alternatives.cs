using System.Collections;
using System.Runtime.CompilerServices;

namespace rambap.cplx.Core;

public interface IAlternative
{
    public Type PartType { get; }
    public IEnumerable<Part> Options { get; }

    // Required to allow Alternative<T> to be initialized with a collection expression
    // see rundown at https://andrewlock.net/behind-the-scenes-of-collection-expressions-part-5-adding-support-for-collection-expressions-to-your-own-types/
    public static class AlternativeBuilder
    {
        public static Alternatives<T> Create<T>(ReadOnlySpan<T> values) where T : Part 
            => new(values.ToArray()) ;
    }
}

[CollectionBuilder(typeof(IAlternative.AlternativeBuilder),nameof(IAlternative.AlternativeBuilder.Create))]
public  class Alternatives<T> : IAlternative, IEnumerable<T>
    where T : Part
{
    public Type PartType => typeof(T);

    private List<T> options = new List<T>();
    public IEnumerable<Part> Options => options;

    public Alternatives(IEnumerable<T> options)
    {
        this.options.AddRange(options);
    }

    // Implement IEnumerable required to allow type detection for initialization with collection expressions
    public IEnumerator<T> GetEnumerator() => options.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => options.GetEnumerator();
    //public void Add(T value) => options.Add(value);
}