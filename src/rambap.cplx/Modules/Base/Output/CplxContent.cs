using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Export.Generators;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Information about the location in the component tree where a <see cref="CplxContent"/> was created
/// </summary>
public record ContentLocation()
{
    public required string CIN { get; init; }
    public required int Multiplicity { get; init; }
    public required int Depth { get; init; }
}

public record FlattenedContentLocation()
{
    public required int LocalItemIndex { get; init; }
    public int LocalItemCount { get; internal set; }
    public bool IsEnd => LocalItemCount-1 == LocalItemIndex;
}

/// <summary>
/// Main abstraction used to represent data contained in a cplx table line <br/>
/// Is a data of single component, or a data of a group of component whose relevant characteristics are all equal.
/// </summary>
public interface IContent
{
    IEnumerable<IContent> SubContents { get; }
    FlattenedContentLocation? LocationWhenFlattened { get; }
    IEnumerable<IContent> AsFlatContent();

    ContentLocation Location { get; }
    ContentLocation GetNextLocation();
    Component Component { get; }


    public LeafCause IsLeafBecause { get; }
    bool IsLeaf { get; }
    bool IsBranch { get; }
    bool IsRecursionBreak { get; }


    bool IsGrouping { get; }
    int ComponentLocalCount { get; }
    int ComponentTotalCount { get; }

    IEnumerable<Component> AllComponents();

    public bool AllComponentsMatch<T>(Func<Component, T> getter);
    public bool AllComponentsMatch<T>(Func<Component, T> getter, out T coherentValue);
}


/// <summary>
/// Content of a Iterated table representing a component or group of component
/// </summary>
public class CplxContent : IContent
{
    public LeafCause IsLeafBecause => GetLeafCause();
    protected virtual LeafCause GetLeafCause()
    {
        if (SubContents.Any())
            return LeafCause.NotALeaf_Continued;
        if (IsRecursionBreak)
            return LeafCause.RecursionBreak;
        if (!SubContents.Any())
            return LeafCause.NoChild;
        else
            throw new NotImplementedException();
    }

    public bool IsLeaf => SubContents.Count() == 0;
    public bool IsBranch => !IsLeaf;
    public bool IsRecursionBreak => !ContentIterator.ShouldTryRecurse(this);

    public IEnumerable<IContent> SubContents => ComputeSubcontents();
    public IEnumerable<IContent> ComputeSubcontents()
    {
        if (IsRecursionBreak)
            yield break;
        foreach (var subContent in ContentIterator.MakeSubContent(this))
            yield return subContent;
    }

    public ContentLocation Location { get; init; }
    public ContentLocation GetNextLocation()
    {
        var localCN = Component.CN;
        var localMultiplicity = ComponentLocalCount;
        var localLocation = Location;
        return new ContentLocation()
        {
            CIN = CID.Append(Location.CIN, localCN),
            Multiplicity = Location.Multiplicity * localMultiplicity,
            Depth = Location.Depth + 1,
        };
    }

    public Component Component => GroupedComponents.First();
    public bool IsGrouping => GroupedComponents.Count > 1;
    public int ComponentLocalCount => GroupedComponents.Count;
    public int ComponentTotalCount => Location.Multiplicity * ComponentLocalCount;

    // On construction, grouped component are assumed to be all instance of the same, value equal definition
    // TODO : ensure this is true. How ? The issue can happens if someone edit an instance or part
    // Without producing an unique PN for it
    private List<Component> GroupedComponents { get; init; } = [];
    public IEnumerable<Component> AllComponents() => GroupedComponents;


    public required IContentIterator<IContent> ContentIterator { private get; init; }
    public bool AllComponentsMatch<T>(Func<Component, T> getter)
    {
        return AllComponentsMatch(getter, out T _);
    }
    public bool AllComponentsMatch<T>(Func<Component, T> getter, out T coherentValue)
    {
        // Parts may be edited, without changing the PN => This would be a mistake, detect it
        var values = AllComponents().Select(getter);
        var disctinctCount = values.Distinct().Count();
        var valuesAreCoherent = disctinctCount <= 1;
        coherentValue = values.First();
        return valuesAreCoherent;
    }

    public FlattenedContentLocation? LocationWhenFlattened { get; private set; }
    public IEnumerable<IContent> AsFlatContent()
    {
        yield return this;
        var subContents = SubContents.ToList();
        int totalSubCount = subContents.Count;
        int ctn = 0;
        foreach(var sub in subContents)
        {
            ((CplxContent)sub).LocationWhenFlattened = new()
            {
                LocalItemIndex = ctn++,
                LocalItemCount = totalSubCount,
            };
            foreach(var subcc in sub.AsFlatContent())
                yield return subcc;
        }
    }

    public CplxContent(ContentLocation loc, Component comp)
        : this(loc, [comp]) { }
    public CplxContent(ContentLocation loc, IEnumerable<Component> allComponents)
    {
        if (!allComponents.Any())
            throw new InvalidOperationException($"{nameof(CplxContent)} must be created with at least one component");
        Location = loc;
        GroupedComponents = [.. allComponents];
    }
}

/// <summary>
/// What caused a Content to be a leaf
/// </summary>
public enum LeafCause
{
    NotALeaf_Continued,

    /// <summary>
    /// Recursion was here stopped on user-defined purpose
    /// </summary>
    RecursionBreak,

    /// <summary>
    /// Recursion was here stopped because there is no component or prperty to recurse to
    /// </summary>
    NoChild,

    /// <summary>
    /// Recursion would have yielded only a signel property child, it was integrated into this
    /// </summary>
    SingleStackedPropertyChild
}

/// <summary>
/// A content of a component Tree representing a property of a component.
/// </summary>
public interface IPropertyContent<out T> : IContent
{
    /// <summary>
    /// Property value. Is owned by the Component
    /// </summary>
    T Property { get; }
}

public sealed class BranchProperty<T> : CplxContent, IPropertyContent<T>
{
    public bool IsSingleStackedPropertyChild { private get; init; } = false;
    public required T Property { get; init; }

    protected override LeafCause GetLeafCause()
    {
        if (IsSingleStackedPropertyChild)
            return LeafCause.SingleStackedPropertyChild;
        else
            return base.GetLeafCause();
    }
    public BranchProperty(ContentLocation loc, Component comp)
        : base(loc, comp)
    { }

    public BranchProperty(ContentLocation loc, IEnumerable<Component> allComponents)
        : base(loc, allComponents)
    { }
}

