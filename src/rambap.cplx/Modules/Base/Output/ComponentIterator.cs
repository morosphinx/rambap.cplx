﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Attributes;
using System.Reflection;

namespace rambap.cplx.Modules.Base.Output;


/// <summary>
/// Produce an IEnumerable iterating over the component tree of an instance, and its properties <br/>
/// Output is structured like a tree of <see cref="ComponentContent"/>. <br/>
/// </summary>
public class ComponentIterator : IIterator<IComponentContent>
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

    protected interface IterationSubChild
    {
        RecursionLocation Location { get; }
        abstract IEnumerable<IComponentContent> GetRecursionBreakContent();
        abstract IEnumerable<IComponentContent> GetRecursionContinueContent(List<IterationSubChild> subItems);
    }

    protected interface ComponentIterationSubChild : IterationSubChild
    {
        IEnumerable<Component> Components { get; }
    }

    protected sealed class SubComponentGroup : ComponentIterationSubChild
    {
        public required RecursionLocation Location { get; init; }
        public required bool WriteComponentBranches { get ; init; }

        public Component MainComponent => Components.First();
        public required IEnumerable<Component> Components{ get; init; }

        public IEnumerable<IComponentContent> GetRecursionBreakContent()
        {
            yield return new LeafComponent(Location, Components) {IsLeafBecause = LeafCause.RecursionBreak };
        }

        public IEnumerable<IComponentContent> GetRecursionContinueContent(List<IterationSubChild> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count() == 0; ;
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


    protected virtual bool ShouldRecurse(IterationSubChild iterationTarget)
    {
        if (iterationTarget is SubComponentGroup group)
        {
            var mainComponent = group.MainComponent;
            var location = group.Location;

            // Test wether we should recurse inside this component's subcomponents
            var stopRecurseAttrib = mainComponent.Instance.PartType.GetCustomAttribute(typeof(CplxHideContentsAttribute));
            bool mayRecursePastThis =
                location.Depth == 0 || // Always recurse the first iteration (root node), no mater the recursion condition
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

    protected IEnumerable<IEnumerable<Component>> GroupComponents(ComponentIterationSubChild group)
    {
        var subcomponents = group.Components.First().Instance.Components;
        var subcomponentContents = GroupPNsAtSameLocation switch
        {
            false => subcomponents.Select<Component, IEnumerable<Component>>(c => [c]),
            true => subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN)).Select(g => g.Select(c => c)),
        };
        return subcomponentContents;
    }
    protected virtual IEnumerable<IterationSubChild> GetChilds(IterationSubChild iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is SubComponentGroup group)
        {
            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponentContents = GroupComponents(group);
            foreach (var i in subcomponentContents)
            {
                var CN = group.MainComponent.CN;
                var multiplicity = group.Components.Count();
                var subItemLocation = loc.GetNextSubItem(CN, multiplicity);

                var item =  new SubComponentGroup()
                {
                    Location  = subItemLocation,
                    Components = i ,
                    WriteComponentBranches = WriteBranches
                };
                yield return item;
            }
        }
    }

    public IEnumerable<IComponentContent> MakeContent(Pinstance instance)
    {
        // Generate the contents and subcontent for the group of components
        // The group of components must all be of same PN at the same location
        IEnumerable<IComponentContent> Recurse(IterationSubChild currentItem, bool tagLevelEnd)
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

        // Create a dummy component to start recuring
        Component rootComponent = new(null)
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
            LocalItemIndex = 0,
            LocalItemCount = 1,
        };
        SubComponentGroup rootItem = new()
        {
            Components = [rootComponent],
            Location = rootLocation,
            WriteComponentBranches = WriteBranches,
        };
        return Recurse(rootItem, true);
    }
}


