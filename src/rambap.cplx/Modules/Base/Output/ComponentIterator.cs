using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Attributes;
using System.Reflection;

namespace rambap.cplx.Modules.Base.Output;


/// <summary>
/// Produce an IEnumerable iterating over the component tree of an instance, and its properties <br/>
/// Output is structured like a tree of <see cref="CplxContent"/>. <br/>
/// </summary>
public class ComponentIterator : IIterator<ICplxContent>
{
    /// <summary>
    /// If False, each subcomponent produce its own content.
    /// If True, identical subcomponent at each location are grouped in the same content. <br/>
    /// </summary>
    public bool GroupPNsAtSameLocation { get; init; } = false;

    /// <summary> If true, return every component encountered when traversing the tree. Otherwise, return only the final leaf components and leaf properties. </summary>
    public bool WriteBranches { get; init; } = true;

    /// <summary>
    /// Define when to recurse on components (will return properties items and subcomponents items) and when not to (will only return the component item)
    /// If null, always recurse
    /// </summary>
    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init; }
    public bool AlwaysRecurseDepth0 { get; set; } = true;

    protected interface IIterationItem
    {
        RecursionLocation Location { get; }
        abstract IEnumerable<ICplxContent> GetRecursionBreakContent();
        abstract IEnumerable<ICplxContent> GetRecursionContinueContent(List<IIterationItem> subItems);
    }

    protected interface IIterationItem_ComponentGroup : IIterationItem
    {
        IEnumerable<Component> Components { get; }
    }

    protected class IterationItem_ComponentGroup : IIterationItem_ComponentGroup
    {
        public required RecursionLocation Location { get; init; }
        public required bool WriteComponentBranches { get ; init; }

        public Component MainComponent => Components.First();
        public required IEnumerable<Component> Components{ get; init; }

        public IEnumerable<ICplxContent> GetRecursionBreakContent()
        {
            yield return new LeafComponent(Location, Components) {IsLeafBecause = LeafCause.RecursionBreak };
        }

        public IEnumerable<ICplxContent> GetRecursionContinueContent(List<IIterationItem> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent(Location, Components) { IsLeafBecause = LeafCause.NoChild };
            }
            else if (WriteComponentBranches)
            {
                // If we reached this point, the component is assured to have child content
                // Wether we output the component itself is configurable :
                yield return new BranchComponent(Location, Components);
            }
        }
    }

    protected virtual bool ShouldRecurse(IIterationItem iterationTarget)
    {
        if (iterationTarget is IterationItem_ComponentGroup group)
        {
            var mainComponent = group.MainComponent;
            var location = group.Location;

            // Test wether we should recurse inside this component's subcomponents
            var stopRecurseAttrib = mainComponent.Instance.PartType.GetCustomAttribute(typeof(CplxHideContentsAttribute));
            bool mayRecursePastThis =
                (location.Depth == 0 && AlwaysRecurseDepth0) || // Always recurse the first iteration (root node), no mater the recursion condition
                (
                    stopRecurseAttrib == null && // CplxHideContentsAttribute must not be present
                    (RecursionCondition == null || RecursionCondition(mainComponent, location))
                );
            return mayRecursePastThis;
        }
        else
        {
            // Always recurse on other types
            return true;
        }
    }

    protected IEnumerable<IEnumerable<Component>> GetSubcomponentsAsGroup(IIterationItem_ComponentGroup group)
    {
        var subcomponents = group.Components.First().SubComponents;
        var subcomponentContents = GroupPNsAtSameLocation switch
        {
            false => subcomponents.Select<Component, IEnumerable<Component>>(c => [c]),
            true => subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN)).Select(g => g.Select(c => c)),
        };
        return subcomponentContents;
    }
    protected virtual IEnumerable<IIterationItem> GetChilds(IIterationItem iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is IterationItem_ComponentGroup group)
        {
            var localCN = group.MainComponent.CN;
            var localMultiplicity = group.Components.Count();
            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponentContents = GetSubcomponentsAsGroup(group);
            foreach (var i in subcomponentContents)
            {
                var subItemLocation = loc.GetNextSubItem(localCN, localMultiplicity);

                var item =  new IterationItem_ComponentGroup()
                {
                    Location  = subItemLocation,
                    Components = i ,
                    WriteComponentBranches = WriteBranches
                };
                yield return item;
            }
        }
    }

    public IEnumerable<ICplxContent> MakeContent(Component rootComponent)
    {
        // Generate the contents and subcontent for the group of components
        // The group of components must all be of same PN at the same location
        IEnumerable<ICplxContent> Recurse(IIterationItem currentItem, bool tagLevelEnd)
        {            
            bool mayRecursePastThis = ShouldRecurse(currentItem);
            bool isLeafDueToRecursionBreak = !mayRecursePastThis;

            if (isLeafDueToRecursionBreak)
            {
                var currentItemContent = currentItem.GetRecursionBreakContent();

                var c1 = currentItemContent.ToList();
                foreach (var content in currentItemContent)
                {

                    if (tagLevelEnd && content == c1.Last())
                        content.Location.IsEnd = true;
                    yield return content;
                }
            }
            else
            {
                // var expectedChildCount = ExpectedchildCount(currentItem);
                // Counter of subcontents, to etablish location information
                var loc = new LocationBuilder()
                {
                    LocationFrom = currentItem.Location,
                    TotalSubItemCount = 0,
                };

                var childs = GetChilds(currentItem, loc).ToList();

                var currentItemContent = currentItem.GetRecursionContinueContent(childs);

                var c1 = currentItemContent.ToList();
                foreach (var content in c1)
                {
                    if(tagLevelEnd && content == c1.Last())
                        content.Location.IsEnd = true;
                    yield return content;
                }

                // Output the subcomponents contents
                var c2 = childs.ToList();
                foreach (var child in c2)
                {
                    var isLastChild = child == c2.Last();
                    foreach (var l in Recurse(child, isLastChild))
                    {
                        yield return l;
                    }
                }
            }
        }

        RecursionLocation rootLocation = new()
        {
            CIN = $"",
            Multiplicity = 1,
            Depth = 0,
            LocalItemIndex = 0,
            LocalItemCount = 1,
        };
        IterationItem_ComponentGroup rootItem = new()
        {
            Components = [rootComponent],
            Location = rootLocation,
            WriteComponentBranches = WriteBranches,
        };
        return Recurse(rootItem, true);
    }
}


