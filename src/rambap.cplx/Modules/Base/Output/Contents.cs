using rambap.cplx.Core;
using System;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Information about the location in the component tree where a <see cref="ComponentContent"/> was created
/// </summary>
public record RecursionLocation()
{
    public required string CIN { get; init; }
    public required int Multiplicity { get; init; }
    public required int Depth { get; init; }
    public required int LocalItemIndex { get; init; }
    public required int LocalItemCount { get; init; }
}

public class LocationBuilder()
{
    public required RecursionLocation ParentLocation { get; init; }

    public required int TotalSubItemCount { get; set; }

    public int CurrentSubItemIndex { get; private set; } = 0;

    public RecursionLocation GetNextSubItem()
    {
        return ParentLocation with
        {
            Depth = ParentLocation.Depth + 1,
            LocalItemIndex = CurrentSubItemIndex++,
            LocalItemCount = TotalSubItemCount,
        };
    }
}

public interface IComponentContent
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
public abstract class ComponentContent : IComponentContent
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

    public ComponentContent(RecursionLocation loc, Component comp)
    {
        Location = loc;
        Component = comp;
    }
    public ComponentContent(RecursionLocation loc, IEnumerable<Component> allComponents)
        : this(allComponents.Select(c => (loc, c))) { }

    public ComponentContent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
    {
        if (!allComponents.Any())
            throw new InvalidOperationException($"{nameof(ComponentContent)} must be created with at least one component");
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
public class LeafComponent : ComponentContent
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
public class BranchComponent : ComponentContent
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

public class LeafComponentWithProperty<T> : LeafComponent, IPropertyContent<T>
{
    public LeafComponentWithProperty(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public LeafComponentWithProperty(RecursionLocation loc, IEnumerable<Component> allComponents)
        : base(loc, allComponents)
    { }

    public LeafComponentWithProperty(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    public required T Property { get; init; }
}

/// <summary>
/// A content of a component Tree representing a property of a component.
/// </summary>
public interface IPropertyContent<out T> : IComponentContent
{
    /// <summary>
    /// Property value. Is owned by the Component
    /// </summary>
    T Property { get; }

    LeafCause IsLeafBecause { get; }
}