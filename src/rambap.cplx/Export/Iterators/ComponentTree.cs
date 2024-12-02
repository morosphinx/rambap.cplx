﻿using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using System.Reflection;

namespace rambap.cplx.Export.Iterators;

public record RecursionLocation()
{
    public required string CIN { get; init; }
    public required int Depth { get; init; }
    public required int ComponentIndex { get; init; }
    public required int ComponentCount { get; init; }
}

public abstract record ComponentTreeItem()
{
    public required RecursionLocation Location { get; init; }
    public required Component Component { get; init; }
}
public enum LeafCause
{
    RecursionBreak,
    NoChild,
}
/// <summary>
/// An Item of the component Tree representing a component. Has no child itm
/// </summary>
public record LeafComponent : ComponentTreeItem
{
    public required LeafCause IsLeafBecause { get; init ; }
}

/// <summary>
/// An Item of the component Tree representing a component. Has descendants, either <see cref="LeafComponent"/> or <see cref="LeafProperty"/>
/// </summary>
public record BranchComponent : ComponentTreeItem { }

/// <summary>
/// An Item of the component Tree representing a property of a component.
/// </summary>
public record LeafProperty : ComponentTreeItem
{
    public object? Property { get; init; } = null;
}

public class ComponentTree : IIterator<ComponentTreeItem>
{
    /// <summary> If true, return every component encountered when traversing the tree. Otherwise, return only the final leaf components and properties</summary>
    public bool WriteBranches { get; init; } = true;

    /// <summary> If true, iterate on the component properties </summary>
    bool WriteProperties => PropertyIterator != null;

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public Func<Pinstance, IEnumerable<object>>? PropertyIterator { private get; init; }

    /// <summary>
    /// Define when to recurse on components (will return properties items and subcomponents items) and when not to (will only return the component item)
    /// If null, always recurse
    /// </summary>
    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init ; }

    public IEnumerable<ComponentTreeItem> MakeContent(Pinstance content)
    {
        IEnumerable<ComponentTreeItem> Recurse(Component c, RecursionLocation location)
        {
            var stopRecurseAttrib = c.Instance.PartType.GetCustomAttribute(typeof(CplxHideContentsAttribute));
            bool mayRecursePastThis =
                stopRecurseAttrib == null &&
                (
                    RecursionCondition == null
                    || RecursionCondition(c, location)
                    || location.Depth == 0 // Always recurse the first iteration (root node), no mater the recursion condition
                ); 
            bool isLeafDueToRecursionBreak = ! mayRecursePastThis ;
            if (isLeafDueToRecursionBreak)
            {
                yield return new LeafComponent() { Component = c, Location = location, IsLeafBecause = LeafCause.RecursionBreak};
                yield break ; // Leaf component : stop iteration here, do not write subcomponent or properties
            }

            bool willHaveAnyChildItem =
                c.Instance.Components.Any() ||
                (WriteProperties && PropertyIterator!(c.Instance).Any());
            bool isLeafDueToNoChild = ! willHaveAnyChildItem ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent() { Component = c, Location = location, IsLeafBecause = LeafCause.NoChild };
                yield break;
            }

            if (WriteBranches)
            {
                yield return new BranchComponent() { Component = c, Location = location };
            }
            if (WriteProperties)
            {
                foreach (var prop in PropertyIterator!(c.Instance))
                    yield return new LeafProperty() { Component = c, Location = location, Property = prop};
            }
            var componentIdx = 0;
            var componentCount = c.Instance.Components.Count();
            foreach (var subcomponent in c.Instance.Components)
            {
                RecursionLocation subLocation = new()
                {
                    CIN = CID.Append(location.CIN, c.CN),
                    Depth = location.Depth + 1,
                    ComponentIndex = componentIdx ++,
                    ComponentCount = componentCount,
                };
                foreach(var l in Recurse(subcomponent, subLocation))
                    yield return l;
            }
        }
        // Create a dummy component to start recursing
        Component rootComponent = new()
        {
            CN = $"*",
            Instance = content
        }; 
        RecursionLocation rootLocation = new() 
        {
            CIN = $"",
            Depth = 0,
            ComponentIndex = 0,
            ComponentCount = 1,
        };
        return Recurse(rootComponent, rootLocation);
    }
}


