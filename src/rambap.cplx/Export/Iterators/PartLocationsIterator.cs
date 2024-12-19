using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using System.Reflection;

namespace rambap.cplx.Export.Iterators;

// currently a tweaked component Iterator, need some time to think about it
public class PartLocationIterator : IIterator<ComponentContent>
{
    bool WriteProperties => PropertyIterator != null;

    public Func<Pinstance, IEnumerable<object>>? PropertyIterator { private get; init; }

    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init ; }

    public IEnumerable<ComponentContent> MakeContent(Pinstance content)
    {
        IEnumerable<ComponentContent> Recurse(IEnumerable<Component> compos, RecursionLocation location)
        {
            var components = compos.Select(c => (location, c));
            var mainComponent = compos.First();
            var stopRecurseAttrib = mainComponent.Instance.PartType.GetCustomAttribute(typeof(CplxHideContentsAttribute));
            bool mayRecursePastThis =
                stopRecurseAttrib == null &&
                (
                    RecursionCondition == null
                    || RecursionCondition(mainComponent, location)
                    || location.Depth == 0 // Always recurse the first iteration (root node), no mater the recursion condition
                ); 
            bool isLeafDueToRecursionBreak = ! mayRecursePastThis ;
            if (isLeafDueToRecursionBreak)
            {
                yield return new LeafComponent(components) { IsLeafBecause = LeafCause.RecursionBreak};
                yield break ; // Leaf component : stop iteration here, do not write subcomponent or properties
            }

            bool willHaveAnyChildItem =
                mainComponent.Instance.Components.Any() ||
                (WriteProperties && PropertyIterator!(mainComponent.Instance).Any());
            bool isLeafDueToNoChild = ! willHaveAnyChildItem ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent(components) { IsLeafBecause = LeafCause.NoChild };
                yield break;
            }

            if (true) // Always write branch properties
            {
                yield return new BranchComponent(components);
            }
            if (WriteProperties)
            {
                foreach (var prop in PropertyIterator!(mainComponent.Instance))
                    yield return new LeafProperty(components) { Property = prop };

            }
            var subcomponents = mainComponent.Instance.Components;
            var subcomponentsTypes = subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN));

            var groupIdx = 0;
            var groupCount = subcomponentsTypes.Count();
            foreach (var g in subcomponentsTypes)
            {
                RecursionLocation subLocation = new()
                {
                    CIN = CID.Append(location.CIN, mainComponent.CN),
                    Multiplicity = location.Multiplicity * compos.Count(),
                    Depth = location.Depth + 1,
                    ComponentIndex = groupIdx++,
                    ComponentCount = groupCount,
                };
                foreach(var l in Recurse(g, subLocation))
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
            Multiplicity = 1,
            Depth = 0,
            ComponentIndex = 0,
            ComponentCount = 1,
        };
        return Recurse([rootComponent], rootLocation);
    }
}


