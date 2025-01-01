using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using System.Reflection;

namespace rambap.cplx.Export.Iterators;

/// <summary>
/// Produce an IEnumerable iterating over the component tree of an instance, and its properties <br/>
/// Output is structured like a tree of <see cref="ComponentContent"/>. <br/>
/// </summary>
public class ComponentIterator : IIterator<ComponentContent>
{
    /// <summary> If true, return every component encountered when traversing the tree. Otherwise, return only the final leaf components and leaf properties. </summary>
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

    public IEnumerable<ComponentContent> MakeContent(Pinstance instance)
    {
        IEnumerable<ComponentContent> Recurse(Component c, RecursionLocation location)
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
                yield return new LeafComponent(location, c) { IsLeafBecause = LeafCause.RecursionBreak};
                yield break ; // Leaf component : stop iteration here, do not write subcomponent or properties
            }

            bool willHaveAnyChildItem =
                c.Instance.Components.Any() ||
                (WriteProperties && PropertyIterator!(c.Instance).Any());
            bool isLeafDueToNoChild = ! willHaveAnyChildItem ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent(location, c) { IsLeafBecause = LeafCause.NoChild };
                yield break;
            }

            if (WriteBranches)
            {
                yield return new BranchComponent(location, c);
            }
            if (WriteProperties)
            {
                foreach (var prop in PropertyIterator!(c.Instance))
                    yield return new LeafProperty(location, c) { Property = prop };
            }
            var componentIdx = 0;
            var componentCount = c.Instance.Components.Count();
            foreach (var subcomponent in c.Instance.Components)
            {
                RecursionLocation subLocation = new()
                {
                    CIN = CID.Append(location.CIN, c.CN),
                    Multiplicity = location.Multiplicity * 1,
                    Depth = location.Depth + 1,
                    ComponentIndex = componentIdx++,
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
            Instance = instance,
            IsPublic = true,
        }; 
        RecursionLocation rootLocation = new() 
        {
            CIN = $"",
            Multiplicity = 1,
            Depth = 0,
            ComponentIndex = 0,
            ComponentCount = 1,
        };
        return Recurse(rootComponent, rootLocation);
    }
}


