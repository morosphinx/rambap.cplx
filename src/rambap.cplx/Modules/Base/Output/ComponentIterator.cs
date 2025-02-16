using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.PartAttributes;
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

    protected abstract class IterationSubChild()
    {
        public required RecursionLocation Location { get; init; }

        public abstract IEnumerable<IComponentContent> GetRecursionBreakContent();

        public abstract IEnumerable<IComponentContent> GetRecursionContinueContent(LocationBuilder subItemLocationBuilder, List<IterationSubChild> subItems);

    }
    protected abstract class ComponentGroupSubChild : IterationSubChild
    {
        public required bool WriteBranches { get ; init; }

        public Component MainComponent => Components.First();
        public required IEnumerable<Component> Components{ get; init; }

        public override IEnumerable<IComponentContent> GetRecursionBreakContent()
        {
            yield return new LeafComponent(Location, Components) { IsLeafBecause = LeafCause.RecursionBreak };
        }

        public override IEnumerable<IComponentContent> GetRecursionContinueContent(LocationBuilder subItemLocationBuilder, List<IterationSubChild> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count() == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent(Location, Components) { IsLeafBecause = LeafCause.NoChild };
                yield break;
            }

            // If we reached this point, the component is assured to have child content
            // Wether we output the component itself is configurable :
            if (WriteBranches)
            {
                yield return new BranchComponent(Location, Components);
            }
        }
    }

    protected sealed class SubComponentGroup : ComponentGroupSubChild { }


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

    protected virtual int ExpectedchildCount(IterationSubChild iterationTarget)
        => iterationTarget switch
        {
            SubComponentGroup group => group.MainComponent.Instance.Components.Count(), // TODO : include subcomponent group
            _ => 0
        };
    protected virtual IEnumerable<IterationSubChild> GetChilds(IterationSubChild iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is SubComponentGroup group)
        {
            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponents = group.Components.First().Instance.Components;
            var subcomponentContents = GroupPNsAtSameLocation switch
            {
                false => subcomponents.Select<Component, IEnumerable<Component>>(c => [c]),
                true => subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN)).Select(g => g.Select(c => c)),
            };
            foreach (var i in subcomponentContents)
            {
                var mainComponent = group.MainComponent;
                var currentlocation = iterationTarget.Location;
                var subItemLocation = loc.GetNextSubItem();
                subItemLocation = subItemLocation with
                {
                    // Set subPart Location and multiplicity
                    CIN = CID.Append(currentlocation.CIN, mainComponent.CN),
                    Multiplicity = currentlocation.Multiplicity * group.Components.Count(),
                };
                var item =  new SubComponentGroup() { Components = i , Location  = subItemLocation, WriteBranches = WriteBranches };
                yield return item;
            }
        }
    }

    public IEnumerable<IComponentContent> MakeContent(Pinstance instance)
    {
        // Generate the contents and subcontent for the group of components
        // The group of components must all be of same PN at the same location
        IEnumerable<IComponentContent> Recurse(IterationSubChild currentItem)
        {            
            bool mayRecursePastThis = ShouldRecurse(currentItem);
            bool isLeafDueToRecursionBreak = !mayRecursePastThis;

            if (isLeafDueToRecursionBreak)
            {
                var items = currentItem.GetRecursionBreakContent();
                foreach (var i in items)
                    yield return i;
                yield break; // Leaf component : stop iteration here, do not write subcomponent or properties
            }


            var expectedChildCount = ExpectedchildCount(currentItem);
            // Counter of subcontents, to etablish location information
            var loc = new LocationBuilder()
            {
                ParentLocation = currentItem.Location,
                TotalSubItemCount = expectedChildCount,
            };

            var childs = GetChilds(currentItem, loc).ToList();
            {
                var items = currentItem.GetRecursionContinueContent(loc, childs);
                foreach (var i in items)
                    yield return i;
            }

            // Output the subcomponents contents
            foreach (var child in childs)
            {
                foreach (var l in Recurse(child))
                    yield return l;
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
            WriteBranches = WriteBranches,
        };
        return Recurse(rootItem);
    }

    public static IEnumerable<IComponentContent> SubIterate(
        IEnumerable<IComponentContent> contents,
        Func<IComponentContent, IEnumerable<IComponentContent>> additionalComponents)
    {
        foreach (var content in contents)
        {
            var newContents = additionalComponents(content);
            IEnumerable<IComponentContent> allContents = [content, .. newContents];
            foreach(var c in allContents)
                yield return c;
        }
    }

    // public static IEnumerable<IComponentContent> SubIterateProperties(
    //     IEnumerable<IComponentContent> contents,
    //     Func<T, IEnumerable<T>> additionalProperties,
    //     bool applyRecursively = true)
    // {
    //     IEnumerable<LeafComponentWithProperty<T>> MakeAditionalContents(IPropertyContent<T> leafProperty)
    //     {
    //         var location = leafProperty.Location;
    //         T property = leafProperty.Property!;
    //         var newProperties = additionalProperties(property);
    // 
    //         var currentSubitemIndex = 0;
    //         var subItemTotalCount = newProperties.Count();
    // 
    //         foreach (var p in newProperties)
    //         {
    //             var propLocation = location with
    //             {
    //                 Depth = location.Depth + 1,
    //                 LocalItemIndex = currentSubitemIndex++,
    //                 LocalItemCount = subItemTotalCount,
    //             };
    //             var relocatedComponents = leafProperty.AllComponents().Select(c => (propLocation, c.component));
    //             yield return new LeafComponentWithProperty<T>(relocatedComponents)
    //             {
    //                 Property = p,
    //                 IsLeafBecause = LeafCause.NoChild
    //             };
    //         }
    // 
    //     }
    //     return ComponentIterator<T>.SubIterate(contents,
    //         c => c switch
    //         {
    //             IPropertyContent<T> lp => MakeAditionalContents(lp),
    //             LeafComponent lc => [],
    //             BranchComponent bc => [],
    //             _ => throw new NotImplementedException(),
    //         });
    // }
}


