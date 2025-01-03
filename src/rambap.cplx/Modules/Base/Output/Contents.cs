using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Information about the location in the component tree where a <see cref="ComponentContent"/> was created
/// </summary>
public record RecursionLocation()
{
    public required string CIN { get; init; }
    public required int Multiplicity { get; init; }
    public required int Depth { get; init; }
    public required int ComponentIndex { get; init; }
    public required int ComponentCount { get; init; }
}


/// <summary>
/// Content of a Iterated table representing a component or group of component
/// </summary>
public abstract record ComponentContent
{
    public RecursionLocation Location { get; }
    public Component Component { get; }

    // This should be faster than calling AllComponents().Count(), witch iterate an enumerable
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

    public ComponentContent(RecursionLocation loc, Component comp)
    {
        Location = loc;
        Component = comp;
    }

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
}

/// <summary>
/// A content of a component Tree representing a component. Has no child content
/// </summary>
public record LeafComponent : ComponentContent
{
    public LeafComponent(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public LeafComponent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    public required LeafCause IsLeafBecause { get; init; }
}

/// <summary>
/// A content of a component Tree representing a component. Has descendants, either <see cref="LeafComponent"/> or <see cref="LeafProperty"/>
/// </summary>
public record BranchComponent : ComponentContent
{
    public BranchComponent(RecursionLocation loc, Component comp)
    : base(loc, comp)
    { }

    public BranchComponent(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }
}

/// <summary>
/// A content of a component Tree representing a property of a component.
/// </summary>
public record LeafProperty : ComponentContent
{
    public LeafProperty(RecursionLocation loc, Component comp)
        : base(loc, comp)
    { }

    public LeafProperty(IEnumerable<(RecursionLocation loc, Component comp)> allComponents)
        : base(allComponents)
    { }

    /// <summary>
    /// Property value. Is owned by the Component
    /// </summary>
    public required object? Property { get; init; } = null;
}