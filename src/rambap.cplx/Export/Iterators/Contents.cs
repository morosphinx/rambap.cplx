using rambap.cplx.Core;

namespace rambap.cplx.Export.Iterators;

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
/// Content of a table representing a single component
/// </summary>
public abstract record ComponentContent()
{
    public required RecursionLocation Location { get; init; }
    public required Component Component { get; init; }
}

/// <summary>
/// A content of a component Tree representing a component. Has no child content
/// </summary>
public record LeafComponent : ComponentContent
{
    public required LeafCause IsLeafBecause { get; init; }
}

/// <summary>
/// A content of a component Tree representing a component. Has descendants, either <see cref="LeafComponent"/> or <see cref="LeafProperty"/>
/// </summary>
public record BranchComponent : ComponentContent { }

/// <summary>
/// A content of a component Tree representing a property of a component.
/// </summary>
public record LeafProperty : ComponentContent
{
    public object? Property { get; init; } = null;
}


/// <summary>
/// Content of a table representing a part <br/>
/// Thechnicaly, a group of component sharing the same part definition
/// </summary>
public abstract record PartContent
{
    /// <summary>
    /// Main component, that should be assumed to have value equal property to all other
    /// </summary>
    public ComponentContent PrimaryItem => Items.First();

    /// <summary>
    /// All components of the group
    /// </summary>
    public required List<ComponentContent> Items { get; init; } = new();
}

/// <summary>
/// Content of a part list represent a group of part that ALL have either no child, or the recursion was stopped on
/// </summary>
public record LeafPartContent : PartContent { }
public record BranchPartContent : PartContent { }
public record LeafPropertyPartContent : PartContent
{
    public object? Property { get; init; } = null;
}