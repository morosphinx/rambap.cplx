using System.Collections;
using System.Runtime.CompilerServices;

namespace rambap.cplx.Core;

/// <summary>
/// Define how to construct a part tree when <see cref="IAlternative"/>s are present. <br/>
/// </summary>
public class PartConfiguration
{
    public abstract class Preference
    {
        /// <summary> Try to decide of an option on an <see cref="IAlternative"/> </summary>
        /// <param name="alternative">Alternative begin decided on</param>
        /// <returns>The selected part if this preference apply, null if it does not apply</returns>
        public abstract Part? SelectFrom(IAlternative alternative);
    }

    /// <summary> Preference rule matching an Item by index on a specific <see cref="IAlternative"/></summary>
    /// <param name="targetAlternative"> The alternative that define this preference</param>
    /// <param name="index">The index to select in this alternative</param>
    public class Preference_Index(IAlternative targetAlternative, int index) : Preference
    {
        public override Part? SelectFrom(IAlternative alternative)
            => alternative == targetAlternative ? alternative.Options.ElementAt(index) : null;
    }

    /// <summary> Preference rule priortizing a type of Part over anything else </summary>
    /// <typeparam name="PREF">Prefered type</typeparam>
    public class Preference_Type<PREF> : Preference
        where PREF : Part
    {
        public override Part? SelectFrom(IAlternative alternative)
            => alternative.Options.OfType<PREF>().FirstOrDefault(); // Return null if type not found
    }

    /// <summary> Preference rule priortizing a type of Part over another when both are encountered in an <see cref="IAlternative"/> </summary>
    /// <typeparam name="PREF">Prefered type</typeparam>
    /// <typeparam name="OVER">Type to </typeparam>
    public class Preference_TypeOver<PREF,OVER> : Preference
        where PREF : Part
        where OVER : Part
    {
        public override Part? SelectFrom(IAlternative alternative)
        {
            var prefered = alternative.Options.OfType<PREF>().FirstOrDefault();
            // Iterate in reverse order in order to avoid matching twice the same item in case there is a type relatation between PREF and OVER
            var over = alternative.Options.Reverse().OfType<OVER>().FirstOrDefault();
            // If a 'prefered' part is found, return it over the 'over' part
            if (prefered != null && over != null && prefered != over)
                return prefered;
            else return null;
        }
    }

    /// <summary> Preference rule selecting the first Item</summary>
    public class Preference_First : Preference
    {
        public override Part? SelectFrom(IAlternative alternative)
            => alternative.Options.FirstOrDefault() ; // Return null on empty Options list
    }

    /// <summary> Rules, applied in order, so select an Option from an Alternative </summary>
    public IEnumerable<Preference> Preferences => preferences;
    private List<Preference> preferences { get; init; } = new(){};

    public PartConfiguration() : this([]) { }
    public PartConfiguration(IEnumerable<Preference> preferences)
    {
        this.preferences.AddRange(preferences);
        // Default preference behavior : use the first item of each alternative
        this.preferences.Add(new Preference_First());
    }

    /// <summary> Try to decide on an option for an <see cref="IAlternative"/> </summary>
    /// <param name="alternative">Alternative begin decided on</param>
    /// <returns>Part produced by the first the preference match, null if no preference match</returns>
    public Part? Decide(IAlternative alternative)
        => preferences
            .Select(p => p.SelectFrom(alternative))
            .Where(o => o != null)
            .FirstOrDefault();
} 

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