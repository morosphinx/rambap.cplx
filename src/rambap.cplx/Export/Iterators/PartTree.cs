using rambap.cplx.Core;

namespace rambap.cplx.Export.Iterators;

public abstract record PartTreeItem
{
    public ComponentTreeItem PrimaryItem => Items.First();
    public required List<ComponentTreeItem> Items { get; init; } = new();
}

public record LeafPartTableItem : PartTreeItem { }
public record BranchPartTableItem : PartTreeItem { }
public record LeafPartPropertyTableItem : PartTreeItem
{
    public object? Property { get; init; } = null;
}

public class PartTree : IIterator<PartTreeItem>
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

    public IEnumerable<PartTreeItem> MakeContent(Pinstance content)
    {
        // Produce a tree table of All Components, stopping on recursing condition.
        var ComponentTable = new ComponentTree()
        {
            WriteBranches = true, // We want information about branch components,
            RecursionCondition = RecursionCondition,
            PropertyIterator = null, // No property iteration when iterating the component tree
        };
        var componentsItems = ComponentTable.MakeContent(content);
        // All returned items of the tree table represent components (eg : No LeafProperty)
        // Group the components by Identity (PN & Type)
        var grouping_by_pn = componentsItems.GroupBy(c => (c.Component.Instance.PartType, c.Component.Instance.PN, c.GetType()));
        // For each group, produce a PartTreeItem
        foreach (var group in grouping_by_pn)
        {
            // Groups have same PN, same PartType, and same type of Component tree node // TODO : This last point may be false
            var itemList = group.ToList();
            var primaryItem = group.First();
            var shouldIShowThisPartContent = itemList.OfType<BranchComponent>().Any()
                || itemList.OfType<LeafComponent>().Where(c => c.IsLeafBecause == LeafCause.NoChild).Any();
            // If the components are branches OR could be recursed further on other condition,
            // We want to know their inside, Therefore we return a BranchPartTableItem
            // And apply the property iterator
            // This is due to not iterating with properties when making ComponentTable() earlier
            // => The component may have a property to iterate, but appear as a LeafComponent after the ComponentTable() 
            if (shouldIShowThisPartContent)
            {
                if (WriteBranches)
                    yield return new BranchPartTableItem { Items = itemList };
                if (IsAPropertyTable)
                {
                    var properties = PropertyIterator!.Invoke(primaryItem.Component.Instance);
                    foreach (var prop in properties)
                        yield return new LeafPartPropertyTableItem() { Items = itemList, Property = prop };
                }
            }
            // If the components are Leaf due to RecursionBreak, we don't want to know their insides
            // Therefore we return a LeafPartTableItem
            else if (primaryItem is LeafComponent lc2 && lc2.IsLeafBecause == LeafCause.RecursionBreak)
            {
                yield return new LeafPartTableItem() { Items = itemList };
            }
            else
            {
                throw new NotImplementedException(); // Cannot be a LeafProperty, we did not iterate component properties
            }
        }
    }
}


