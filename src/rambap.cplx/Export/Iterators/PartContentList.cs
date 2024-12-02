using rambap.cplx.Core;

namespace rambap.cplx.Export.Iterators;

public abstract record PartContent
{
    public ComponentContent PrimaryItem => Items.First();
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

/// <summary>
/// Produce an IEnumerable iterating over parts used as subcomponents of an instance, and properties of those parts. <br/>
/// Output is structured like a list of <see cref="PartContent"/>.
/// </summary>
public class PartContentList : IIterator<PartContent>
{

    public bool WriteBranches { get; init; } = true;

    /// <summary>
    /// Define when to recurse on components (will return properties items and subcomponents items) and when not to (will only return the component item)
    /// If null, always recurse
    /// </summary>
    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init; }

    /// <summary>
    /// Define a final level of iteration on of parts that return properties
    /// Leave this empty to return no properties items
    /// </summary>
    public Func<Pinstance, IEnumerable<object>>? PropertyIterator { private get; init; }
    private bool IsAPropertyTable => PropertyIterator != null;

    public IEnumerable<PartContent> MakeContent(Pinstance content)
    {
        // Produce a tree table of All Components, stopping on recursing condition.
        var ComponentTable = new ComponentContentTree()
        {
            WriteBranches = true, // We want information about all tree components
            RecursionCondition = RecursionCondition,
            PropertyIterator = null, // No property iteration when iterating the component tree
                                     // => Will return only LeafComponent or BranchComponent
        };
        var componentsItems = ComponentTable.MakeContent(content);
        // All returned items of the tree table represent components (eg : No LeafProperty)
        // Group the components by Identity (PN & Type)
        var grouping_by_pn = componentsItems.GroupBy(c => (c.Component.Instance.PartType, c.Component.Instance.PN, c.GetType()));
        // For each group, produce a PartTreeItem
        foreach (var group in grouping_by_pn)
        {
            // Groups have same PN, same PartType
            var itemList = group.ToList();
            var primaryItem = group.First();
            // Recursion on the ComponentTree may depend on part location.
            // So we may have a mix of BranchContent and LeafContent here
            var shouldShowThisGroupContent = itemList.OfType<BranchComponent>().Any()
                || itemList.OfType<LeafComponent>().Where(c => c.IsLeafBecause == LeafCause.NoChild).Any();
            if (shouldShowThisGroupContent)
            {
                // Group has some items that we want to enumerate into
                if (WriteBranches)
                    yield return new BranchPartContent { Items = itemList };
                if (IsAPropertyTable)
                {
                    var properties = PropertyIterator!.Invoke(primaryItem.Component.Instance);
                    foreach (var prop in properties)
                        yield return new LeafPropertyPartContent() { Items = itemList, Property = prop };
                }
            }
            else
            {
                // Group is solely made of LeafPartContant that blocked recursion
                // => We did not want to see what's inside
                yield return new LeafPartContent() { Items = itemList };
            }
        }
    }
}


