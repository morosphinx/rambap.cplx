using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Enumerate component trees like a <see cref="ComponentIterator"/>, plus, on each component, iterate properties with <br/>
/// 1 - <see cref="PropertyIterator"/> <br/>
/// 2 (Optional) - <see cref="PropertySubIterator"/> <br/>
/// </summary>
/// <typeparam name="P">Enumerated property Type.</typeparam>
public class ComponentPropertyIterator<P> : ComponentIterator
{
    protected sealed class IterationItem_Property : IIterationItem
    {
        public required RecursionLocation Location { get; init; }

        public required IEnumerable<Component> Components { get; init; }
        public required P Property { get; init; }

        public IEnumerable<ICplxContent> GetRecursionBreakContent()
        {
            yield return new LeafProperty<P>(Location, Components)
            {
                Property = Property,
                IsLeafBecause = LeafCause.RecursionBreak
            };
        }

        public IEnumerable<ICplxContent> GetRecursionContinueContent(List<IIterationItem> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafProperty<P>(Location, Components)
                {
                    Property = Property,
                    IsLeafBecause = LeafCause.NoChild
                };
            }
            else
            {
                yield return new BranchProperty<P>(Location, Components)
                {
                    Property = Property,
                };
            }
        }
    }

    protected sealed class IterationItem_GroupWithSingleProperty : IIterationItem_ComponentGroup
    {
        public required RecursionLocation Location { get; init; }
        public required IEnumerable<Component> Components { get; init; }
        public required P Property { get; init; }

        public IEnumerable<ICplxContent> GetRecursionBreakContent()
        {
            yield return new LeafComponent(Location, Components) { IsLeafBecause = LeafCause.RecursionBreak };
        }

        public IEnumerable<ICplxContent> GetRecursionContinueContent(List<IIterationItem> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafProperty<P>(Location, Components)
                {
                    Property = Property,
                    IsLeafBecause = LeafCause.SingleStackedPropertyChild
                };
            } else
            {
                yield return new BranchProperty<P>(Location, Components)
                {
                    Property = Property,
                };
            }
        }
    }
    protected sealed class IterationItem_GroupWithPrecomputedProperties : IterationItem_ComponentGroup
    {
        public required List<P> Properties { get; init; }
    }


    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public required Func<Component, IEnumerable<P>> PropertyIterator { private get; init; }

    public bool StackPropertiesSingleChildBranches { private get; init; } = true;

    public Func<P, IEnumerable<P>>? PropertySubIterator { private get; init; }

    protected override IEnumerable<IIterationItem> GetChilds(IIterationItem iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is IterationItem_ComponentGroup group)
        {
            var localCN = group.MainComponent.CN;
            var localMultiplicity = group.Components.Count();
            var mainComponent = group.MainComponent;

            // Properties of Components may have been precomputed by parent
            var propertiesContents = group switch
            {
                IterationItem_GroupWithPrecomputedProperties p => p.Properties,
                _ => PropertyIterator(mainComponent).ToList(),
            };

            foreach (var prop in propertiesContents)
            {
                var propLocation = loc.GetNextSubItem();
                yield return new IterationItem_Property()
                {
                    Components = group.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }

            // Components, same as parent except that it can return a SubComponentGroupWithSingleProperty
            // in some specific cases
            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponentContents = GetSubcomponentsAsGroup(group);
            foreach (var subgroup in subcomponentContents)
            {
                var subLocation = loc.GetNextSubItem(localCN, localMultiplicity);

                var subMainComponent = subgroup.First();
                var subproperties = PropertyIterator(subMainComponent).ToList();

                if (StackPropertiesSingleChildBranches
                    && subproperties.Count == 1 // Only a single property
                    && !subMainComponent.Instance.Components.Any()) // No other child
                {
                    // If this would only have a single property as a child, return
                    // A special item that will compact both the component and the property on a single line
                    var item = new IterationItem_GroupWithSingleProperty()
                    {
                        Location = subLocation,
                        Components = subgroup,
                        Property = subproperties.Single()
                    };
                    yield return item;

                }
                else
                {
                    var item = new IterationItem_GroupWithPrecomputedProperties()
                    {
                        Location = subLocation,
                        Components = subgroup,
                        WriteComponentBranches = WriteBranches,
                        Properties = subproperties,
                    };
                    yield return item;
                }
            }
        }
        else if(iterationTarget is IterationItem_GroupWithSingleProperty soloSubPropItem
            && PropertySubIterator != null)
        {
            var properties = PropertySubIterator(soloSubPropItem.Property);
            foreach(var prop in properties)
            {
                var propLocation = loc.GetNextSubItem();
                yield return new IterationItem_Property()
                {
                    Components = soloSubPropItem.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }
        }
        else if(iterationTarget is IterationItem_Property propItem
            && PropertySubIterator != null)
        {
            var properties = PropertySubIterator(propItem.Property);
            foreach (var prop in properties)
            {
                var propLocation = loc.GetNextSubItem();
                yield return new IterationItem_Property()
                {
                    Components = propItem.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }
        }
    }
}
