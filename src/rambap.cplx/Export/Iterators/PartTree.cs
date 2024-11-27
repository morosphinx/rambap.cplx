using rambap.cplx.Core;

namespace rambap.cplx.Export.Iterators;

public abstract record PartTtreeItem
{
    public ComponentTreeItem PrimaryItem => Items.First();
    public required List<ComponentTreeItem> Items { get; init; } = new();
}

public record LeafPartTableItem : PartTtreeItem { }
public record BranchPartTableItem : PartTtreeItem { }
public record LeafPartPropertyTableItem : PartTtreeItem
{
    public object? Property { get; init; } = null;
}

public class PartTree : IIterator<PartTtreeItem>
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

    public IEnumerable<PartTtreeItem> MakeContent(Pinstance content)
    {
        var ComponentTable = new ComponentTree()
        {
            WriteBranches = true, // We want information about branch components,
            RecursionCondition = RecursionCondition,
            PropertyIterator = null, // No property iteration when iterating the component tree
        };
        var componentsItems = ComponentTable.MakeContent(content);
        var grouping_by_pn = componentsItems.GroupBy(c => (c.Component.Instance.PartType, c.Component.Instance.PN, c.GetType()));
        foreach(var group in grouping_by_pn)
        {
            // Groups have same PN, same PartType, and same type of Component tree node
            var itemList = group.ToList();
            var primaryItem = group.First();

            if (primaryItem is BranchComponent)
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
            else if(primaryItem is LeafComponent)
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


