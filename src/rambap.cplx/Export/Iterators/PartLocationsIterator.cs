using rambap.cplx.Core;
using rambap.cplx.PartAttributes;
using System.Reflection;

namespace rambap.cplx.Export.Iterators;

// currently a tweaked component Iterator, need some time to think about it
public class PartLocationIterator : IIterator<PartContent>
{
    bool WriteProperties => PropertyIterator != null;

    public Func<Pinstance, IEnumerable<object>>? PropertyIterator { private get; init; }

    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init ; }

    public IEnumerable<PartContent> MakeContent(Pinstance content)
    {
        IEnumerable<PartContent> Recurse(IEnumerable<Component> compos, RecursionLocation location)
        {
            var c = compos.First();
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
                yield return new LeafPartContent()
                {
                    Items =
                    [.. compos.Select(c =>
                        new LeafComponent() { Component = c, Location = location, IsLeafBecause = LeafCause.RecursionBreak})
                    ]
                };
                yield break ; // Leaf component : stop iteration here, do not write subcomponent or properties
            }

            bool willHaveAnyChildItem =
                c.Instance.Components.Any() ||
                (WriteProperties && PropertyIterator!(c.Instance).Any());
            bool isLeafDueToNoChild = ! willHaveAnyChildItem ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafPartContent()
                {
                    Items =
                    [.. compos.Select(c =>
                        new LeafComponent() { Component = c, Location = location, IsLeafBecause = LeafCause.NoChild})
                    ]
                };
                yield break;
            }

            if (true) // Always write branch properties
            {
                yield return new BranchPartContent()
                {
                    Items =
                    [.. compos.Select(c =>
                        new BranchComponent() { Component = c, Location = location })
                    ]
                };
            }
            if (WriteProperties)
            {
                foreach (var prop in PropertyIterator!(c.Instance))
                    yield return new LeafPropertyPartContent()
                    {
                        Items =
                        [.. compos.Select(c =>
                            new BranchComponent() { Component = c, Location = location })
                        ],
                        Property = prop
                    };

            }
            var subcomponents = c.Instance.Components;
            var subcomponentsTypes = subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN));

            var groupIdx = 0;
            var groupCount = subcomponentsTypes.Count();
            foreach (var g in subcomponentsTypes)
            {
                RecursionLocation subLocation = new()
                {
                    CIN = CID.Append(location.CIN, c.CN),
                    Depth = location.Depth + 1,
                    ComponentIndex = groupIdx ++,
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
            Depth = 0,
            ComponentIndex = 0,
            ComponentCount = 1,
        };
        return Recurse([rootComponent], rootLocation);
    }
}


