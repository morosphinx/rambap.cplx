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

    /// <summary> If true, iterate on the component properties </summary>
    bool WriteProperties => PropertyIterator != null;

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public Func<Component, IEnumerable<object>>? PropertyIterator { private get; set; }

    public bool StackPropertiesSingleChildBranches { private get; init; } = true;

    /// <summary>
    /// Define when to recurse on components (will return properties items and subcomponents items) and when not to (will only return the component item)
    /// If null, always recurse
    /// </summary>
    public Func<Component, RecursionLocation, bool>? RecursionCondition { private get; init; }

    public IEnumerable<IComponentContent> MakeContent(Pinstance instance)
    {
        // Generate the contents and subcontent for the group of components
        // The group of components must all be of same PN at the same location
        IEnumerable<ComponentContent> Recurse(IEnumerable<Component> components, RecursionLocation location)
        {
            var localMultiplicity = components.Count();
            var componentsAndLocation = components.Select(c => (location, c));
            var mainComponent = components.First();
            // Test wether we should recurse inside this component's subcomponents
            var stopRecurseAttrib = mainComponent.Instance.PartType.GetCustomAttribute(typeof(CplxHideContentsAttribute));
            bool mayRecursePastThis =
                stopRecurseAttrib == null &&
                (
                    RecursionCondition == null
                    || RecursionCondition(mainComponent, location)
                    || location.Depth == 0 // Always recurse the first iteration (root node), no mater the recursion condition
                );
            bool isLeafDueToRecursionBreak = !mayRecursePastThis;
            if (isLeafDueToRecursionBreak)
            {
                yield return new LeafComponent(componentsAndLocation) { IsLeafBecause = LeafCause.RecursionBreak };
                yield break; // Leaf component : stop iteration here, do not write subcomponent or properties
            }

            // Test wether this component will have any child content
            bool willHaveAnyChildItem =
                mainComponent.Instance.Components.Any() ||
                WriteProperties && PropertyIterator!(mainComponent).Any();
            bool isLeafDueToNoChild = !willHaveAnyChildItem;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponent(componentsAndLocation) { IsLeafBecause = LeafCause.NoChild };
                yield break;
            }

            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponents = mainComponent.Instance.Components;
            var subcomponentContents = GroupPNsAtSameLocation switch
            {
                false => subcomponents.Select<Component, IEnumerable<Component>>(c => [c]),
                true => subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN)).Select(g => g.Select(c => c)),
            };

            // If there is only a single property child content, return a Leaf with it instead of a Branch+Leaf
            // This behavior is toggleable. It makes  prettiers trees
            var onlyHasASingleProperty = subcomponentContents.Count() == 0 && WriteProperties && PropertyIterator!(mainComponent).Count() == 1;
            if(onlyHasASingleProperty && StackPropertiesSingleChildBranches)
            {
                var property = PropertyIterator!(mainComponent).Single();
                var propLocation = location; // Same location, we replace the branch
                yield return new LeafComponentWithProperty(componentsAndLocation) { Property = property, IsLeafBecause = LeafCause.SingleStackedPropertyChild };
                yield break;
            }

            // If we reached this point, the component is assured to have child content
            // Wether we output the component itself is configurable :
            if (WriteBranches)
            {
                yield return new BranchComponent(componentsAndLocation);
            }

            // Counter of subcontents, to etablish location information
            var currentSubitemIndex = 0;
            var subItemTotalCount = subcomponentContents.Count();
            // Output the properties content, if configured :
            if (WriteProperties)
            {
                var propertiesContents = PropertyIterator!(mainComponent);
                subItemTotalCount += propertiesContents.Count();
                foreach (var prop in propertiesContents)
                {
                    var propLocation = location with
                    {
                        Depth = location.Depth + 1,
                        LocalItemIndex = currentSubitemIndex ++,
                        LocalItemCount = subItemTotalCount,
                    };
                    var componentsWithPropLocation = componentsAndLocation.Select(t => (propLocation, t.c));
                    yield return new LeafComponentWithProperty(componentsWithPropLocation) { Property = prop, IsLeafBecause = LeafCause.NoChild };
                }
            }
            // Output the subcomponents contents
            foreach (var subcontent in subcomponentContents)
            {
                RecursionLocation subLocation = new()
                {
                    CIN = CID.Append(location.CIN, mainComponent.CN),
                    Multiplicity = location.Multiplicity * localMultiplicity,
                    Depth = location.Depth + 1,
                    LocalItemIndex = currentSubitemIndex++,
                    LocalItemCount = subItemTotalCount,
                };
                foreach (var l in Recurse(subcontent, subLocation))
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
        return Recurse([rootComponent], rootLocation);
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

    public static IEnumerable<IComponentContent> SubIterateProperties<T>(
        IEnumerable<IComponentContent> contents,
        Func<T, IEnumerable<T>> additionalProperties,
        bool applyRecursively = true)
    {
        IEnumerable<LeafComponentWithProperty> MakeAditionalContents(IPropertyContent leafProperty)
        {
            var location = leafProperty.Location;
            T property = (T)leafProperty.Property!;
            var newProperties = additionalProperties(property);

            var currentSubitemIndex = 0;
            var subItemTotalCount = newProperties.Count();

            foreach (var p in newProperties)
            {
                var propLocation = location with
                {
                    Depth = location.Depth + 1,
                    LocalItemIndex = currentSubitemIndex++,
                    LocalItemCount = subItemTotalCount,
                };
                var relocatedComponents = leafProperty.AllComponents().Select(c => (propLocation, c.component));
                yield return new LeafComponentWithProperty(relocatedComponents)
                {
                    Property = p,
                    IsLeafBecause = LeafCause.NoChild
                };
            }

        }
        return ComponentIterator.SubIterate(contents,
            c => c switch
            {
                IPropertyContent { Property: T prop } lp => MakeAditionalContents(lp),
                LeafComponent lc => [],
                BranchComponent bc => [],
                _ => throw new NotImplementedException(),
            });
    }
}


