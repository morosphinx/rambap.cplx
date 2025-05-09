﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Base.Output;


/// <summary>
/// Produce an IEnumerable iterating over parts used as subcomponents of an instance, and properties of those parts. <br/>
/// Output is structured like a list of <see cref="PartContent"/>.
/// </summary>
public class PartTypesIterator<T> : IIterator<ICplxContent>
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
    public Func<Component, IEnumerable<T>>? PropertyIterator { private get; init; }
    private bool IsAPropertyTable => PropertyIterator != null;

    /// <summary>
    /// Two <see cref="CplxContent"/> with the same <see cref="ComponentTemplateUnicityIdentifier"/> should be assumed
    /// to have the same template part <br/>
    /// We test the the Type of <see cref="CplxContent"/> to avoid mixing leaf and branch contents.
    /// </summary>
    static (Type, string, Type) ComponentTemplateUnicityIdentifier(ICplxContent c)
        => (c.Component.Instance.PartType, c.Component.Instance.PN, c.GetType());

    public IEnumerable<ICplxContent> MakeContent(Pinstance instance)
    {
        // Produce a tree table of All Components, stopping on recursing condition.
        var ComponentTable = new ComponentIterator()
        {
            WriteBranches = true, // We want information about all tree components
            RecursionCondition = RecursionCondition,
            // No property iteration when iterating the component tree
            // => Will return only LeafComponent or BranchComponent
        };
        var componentsItems = ComponentTable.MakeContent(instance);
        // All returned items of the tree table represent components (eg : No LeafProperty)
        // Group the components by Identity (PN & Type & content kind)
        var grouping_by_pn = componentsItems.GroupBy(c => ComponentTemplateUnicityIdentifier(c));
        // For each group, produce a PartTreeItem
        foreach (var group in grouping_by_pn)
        {
            // Groups have same PN, same PartType
            var itemList = group.ToList();
            var componentGroup = itemList.SelectMany(c => c.AllComponents());
            var primaryItem = group.First();
            // Recursion on the ComponentTree may depend on part location.
            // So we may have a mix of BranchContent and LeafContent here
            var shouldHideThisGroupContent = itemList.All(c => (c as LeafComponent)?.IsLeafBecause == LeafCause.RecursionBreak) ;
            if (shouldHideThisGroupContent)
            {
                // Group is solely made of LeafComponent that blocked recursion
                // => We did not want to see what's inside
                yield return new LeafComponent(componentGroup) { IsLeafBecause = LeafCause.RecursionBreak };
            }
            else
            {
                if (IsAPropertyTable)
                {
                    var properties = PropertyIterator!.Invoke(primaryItem.Component);
                    bool hasProperties = properties.Any();
                    if (hasProperties)
                    {
                        // Part have has some property items that we want to enumerate into
                        if (WriteBranches)
                            yield return new BranchComponent(componentGroup);
                        foreach (var prop in properties)
                            yield return new LeafProperty<T>(componentGroup) { Property = prop, IsLeafBecause = LeafCause.NoChild };
                    } else
                    {
                        // art have no property item child
                        yield return new LeafComponent(componentGroup) { IsLeafBecause = LeafCause.NoChild };
                    }
                }
                else // Not a property table. All Components are returned as leaf with no child
                {
                    yield return new LeafComponent(componentGroup) { IsLeafBecause = LeafCause.NoChild };
                }
            }
        }
    }
}


