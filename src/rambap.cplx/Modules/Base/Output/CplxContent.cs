using rambap.cplx.Core;
using System;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Information about the location in the component tree where a <see cref="CplxContent"/> was created
/// </summary>
public record RecursionLocation()
{
    public required string CIN { get; init; }
    public required int Multiplicity { get; init; }
    public required int Depth { get; init; }
    public required int LocalItemIndex { get; init; }
    public int LocalItemCount { get; internal set; }
    public bool IsEnd { get; internal set; }
}

public class LocationBuilder()
{
    public required RecursionLocation LocationFrom { get; init; }

    public required int TotalSubItemCount { get; init; }

    public int CurrentSubItemIndex { get; private set; } = 0;

    public RecursionLocation GetNextSubItem()
    {
        return LocationFrom with
        {
            Depth = LocationFrom.Depth + 1,
            LocalItemIndex = CurrentSubItemIndex++,
            LocalItemCount = TotalSubItemCount,
        };
    }

    public RecursionLocation GetNextSubItem(string CNappend, int multiplicty = 1)
    {
        return GetNextSubItem() with
        {
            CIN = CID.Append(LocationFrom.CIN, CNappend),
            Multiplicity = LocationFrom.Multiplicity * multiplicty,
        };
    }
}

public interface ICplxContent
{
    RecursionLocation Location { get; }
    Component Component { get; }

    bool IsGrouping { get; }
    int ComponentLocalCount { get; }
    int ComponentTotalCount { get; }

    IEnumerable<(RecursionLocation location, Component component)> AllComponents();

    public bool AllComponentsMatch<T>(Func<Component, T> getter);
    public bool AllComponentsMatch<T>(Func<Component, T> getter, out T coherentValue);
}


/// <summary>
/// Content of a Iterated table representing a component or group of component
/// </summary>
public abstract class CplxContent : ICplxContent
{
    public RecursionLocation Location { get; init; }
    public Component Component { get; }

    // This should be faster than calling AllComponents().Count(), witch iterate an enumerable
    public bool IsGrouping => GroupedComponents.Count > 0;
    public int ComponentLocalCount => 1 + GroupedComponents.Count;
    public int ComponentTotalCount => Location.Multiplicity * ComponentLocalCount;

    // On construction, grouped component are assumed to be all instance of the same, value equal definition
    // TODO : ensure this is true. How ? The issue can happens if someone edit an instance or part
    // Without producing an unique PN for it
    private List<(RecursionLocation, Component)> GroupedComponents { get; init; } = [];
    public IEnumerable<(RecursionLocation location, Component component)> AllComponents()
    {
        yield return (Location, Component);
        foreach (var component in GroupedComponents)
            yield return component;
    }

    public bool AllComponentsMatch<T>(Func<Component, T> getter)
    {
        return AllComponentsMatch<T>(getter, out T _);
    }
    public bool AllComponentsMatch<T>(Func<Component, T> getter, out T coherentValue)
    {
        // Parts may be edited, without changing the PN => This would be a mistake, detect it
        var values = AllComponents().Select(c => getter(c.component));
        var disctinctCount = values.Distinct().Count();
        var valuesAreCoherent = disctinctCount <= 1;
        coherentValue = values.First();
        return valuesAreCoherent;
    }

    public CplxContent(RecursionLocation loc, Component comp)
    {
        Location = loc;
        Component = comp;
    }
    public CplxContent(RecursionLocation loc, IEnumerable<Component> allComponents)
        : this(allComponents.Select(c => (loc, c))) { }

    public CplxContent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
    {
        if (!allComponents.Any())
            throw new InvalidOperationException($"{nameof(CplxContent)} must be created with at least one component");
        var mainComponent = allComponents.First();
        Location = mainComponent.loc;
        Component = mainComponent.comp;
        var otherComponents = allComponents.Skip(1);
        GroupedComponents = [.. otherComponents];
    }
}

/// <summary>
/// What caused a Content to be a leaf
/// </summary>
public enum LeafCause
{
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
/// A content of a component Tree representing a component. Has no child content
/// </summary>
public sealed class LeafComponent : CplxContent, IPureComponentContent, ILeafContent
{
    public LeafComponent(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public LeafComponent(RecursionLocation loc, IEnumerable<Component> allComponents)
        : base(loc, allComponents)
    { }

    public LeafComponent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    public required LeafCause IsLeafBecause { get; init; }
}

/// <summary>
/// A content of a component Tree representing a component. Has descendants, either <see cref="LeafComponent"/> or <see cref="IPropertyContent"/>
/// </summary>
public sealed class BranchComponent : CplxContent, IPureComponentContent, IBranchContent
{
    public BranchComponent(RecursionLocation loc, Component comp)
    : base(loc, comp)
    { }

    public BranchComponent(RecursionLocation loc, IEnumerable<Component> allComponents)
    : base(loc, allComponents)
    { }

    public BranchComponent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }
}

public sealed class LeafProperty<T> : CplxContent, IPropertyContent<T>, ILeafContent
{
    public LeafProperty(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public LeafProperty(RecursionLocation loc, IEnumerable<Component> allComponents)
        : base(loc, allComponents)
    { }

    public LeafProperty(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    public required T Property { get; init; }
    public required LeafCause IsLeafBecause { get; init; }
}

public sealed class BranchProperty<T> : CplxContent, IPropertyContent<T>, IBranchContent
{
    public BranchProperty(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public BranchProperty(RecursionLocation loc, IEnumerable<Component> allComponents)
        : base(loc, allComponents)
    { }

    public BranchProperty(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    public required T Property { get; init; }
}

public interface IPureComponentContent : ICplxContent
{
}

/// <summary>
/// A content of a component Tree representing a property of a component.
/// </summary>
public interface IPropertyContent<out T> : ICplxContent
{
    /// <summary>
    /// Property value. Is owned by the Component
    /// </summary>
    T Property { get; }

}

public interface ILeafContent : ICplxContent
{
    LeafCause IsLeafBecause { get; }
}

public interface IBranchContent : ICplxContent
{
}