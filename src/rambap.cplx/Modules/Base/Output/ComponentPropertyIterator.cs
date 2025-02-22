using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;

public class ComponentPropertyIterator<T> : ComponentIterator
{
    protected sealed class SubComponentGroupWithProperty : IterationSubChild
    {
        public required RecursionLocation Location { get; init; }

        public required IEnumerable<Component> Components { get; init; }
        public required T Property { get; init; }

        public IEnumerable<IComponentContent> GetRecursionBreakContent()
        {
            yield return new LeafComponentWithProperty<T>(Location, Components)
            {
                Property = Property,
                IsLeafBecause = LeafCause.RecursionBreak
            };
        }

        public IEnumerable<IComponentContent> GetRecursionContinueContent(List<IterationSubChild> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count() == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponentWithProperty<T>(Location, Components)
                {
                    Property = Property,
                    IsLeafBecause = LeafCause.NoChild
                };
            }
            else
            {
                yield return new BranchComponentWithProperty<T>(Location, Components)
                {
                    Property = Property,
                };
            }
        }
    }

    protected sealed class SubComponentGroupWithSingleProperty : ComponentIterationSubChild
    {
        public required RecursionLocation Location { get; init; }
        public required IEnumerable<Component> Components { get; init; }
        public required T Property { get; init; }

        public IEnumerable<IComponentContent> GetRecursionBreakContent()
        {
            yield return new LeafComponent(Location, Components) { IsLeafBecause = LeafCause.RecursionBreak };
        }

        public IEnumerable<IComponentContent> GetRecursionContinueContent(List<IterationSubChild> subItems)
        {
            bool isLeafDueToNoChild = subItems.Count() == 0; ;
            if (isLeafDueToNoChild)
            {
                yield return new LeafComponentWithProperty<T>(Location, Components)
                {
                    Property = Property,
                    IsLeafBecause = LeafCause.SingleStackedPropertyChild
                };
            } else
            {
                yield return new BranchComponentWithProperty<T>(Location, Components)
                {
                    Property = Property,
                };
            }
        }
    }

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public required Func<Component, IEnumerable<T>> PropertyIterator { private get; init; }

    public bool StackPropertiesSingleChildBranches { private get; init; } = true;

    public Func<T, IEnumerable<T>>? PropertySubIterator { private get; init; }

    protected override IEnumerable<IterationSubChild> GetChilds(IterationSubChild iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is SubComponentGroup group)
        {
            // Properties of Components
            var localMultiplicity = group.Components.Count();
            var mainComponent = group.MainComponent;

            var propertiesContents = PropertyIterator(mainComponent);

            foreach (var prop in propertiesContents)
            {
                var propLocation = loc.GetNextSubItem();

                yield return new SubComponentGroupWithProperty()
                {
                    Components = group.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }

            // Components, same as parent except that it can return a SubComponentGroupWithSingleProperty
            // in some specific cases
            // prepare subcomponents contents. Group them by same PartType & PN if configured :
            var subcomponentContents = GroupComponents(group);
            foreach (var i in subcomponentContents)
            {
                var CN = group.MainComponent.CN;
                var multiplicity = group.Components.Count();
                var subItemLocation = loc.GetNextSubItem(CN, multiplicity);

                var properties = PropertyIterator(group.MainComponent);

                if(StackPropertiesSingleChildBranches 
                    && properties.Count() == 1 // Only a single property
                    && i.First().Instance.Components.Count() == 0) // No other child
                {
                    // If this would only have a single property as a child, return
                    // A special item that will compact both the component and the property on a single line
                    var item = new SubComponentGroupWithSingleProperty()
                    {
                        Location = subItemLocation,
                        Components = i,
                        Property = properties.Single()
                    };
                    yield return item;

                } else
                {
                    var item = new SubComponentGroup()
                    {
                        Location = subItemLocation,
                        Components = i,
                        WriteComponentBranches = WriteBranches
                    };
                    yield return item;
                }
            }
        }
        else if(iterationTarget is SubComponentGroupWithSingleProperty soloSubPropItem
            && PropertySubIterator != null)
        {
            var properties = PropertySubIterator(soloSubPropItem.Property);
            foreach(var prop in properties)
            {
                var propLocation = loc.GetNextSubItem();
                yield return new SubComponentGroupWithProperty()
                {
                    Components = soloSubPropItem.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }
        }
        else if(iterationTarget is SubComponentGroupWithProperty propItem
            && PropertySubIterator != null)
        {
            var properties = PropertySubIterator(propItem.Property);
            foreach (var prop in properties)
            {
                var propLocation = loc.GetNextSubItem();
                yield return new SubComponentGroupWithProperty()
                {
                    Components = propItem.Components,
                    Location = propLocation,
                    Property = prop,
                };
            }
        }
    }
}
