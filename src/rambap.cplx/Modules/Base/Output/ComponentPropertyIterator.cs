using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;

class ComponentPropertyIterator<T> : ComponentIterator
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
            // TODO : may be branch component, if prperty recursion
            yield return new LeafComponentWithProperty<T>(Location, Components)
            {
                Property = Property,
                IsLeafBecause = LeafCause.NoChild
            };
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
            // TODO : may be branch component, if prperty recursion
            yield return new LeafComponentWithProperty<T>(Location, Components)
            {
                Property = Property,
                IsLeafBecause = LeafCause.SingleStackedPropertyChild
            };
        }
    }

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public required Func<Component, IEnumerable<T>> PropertyIterator { private get; init; }

    public bool StackPropertiesSingleChildBranches { private get; init; } = true;

    public Func<T, IEnumerable<T>>? PropertySubIterator { private get; init; }

    protected override int ExpectedchildCount(IterationSubChild iterationTarget)
        =>
            iterationTarget switch
            {
                SubComponentGroup group => PropertyIterator(group.MainComponent).Count()
                                          + base.ExpectedchildCount(iterationTarget),
                SubComponentGroupWithSingleProperty so => PropertySubIterator?.Invoke(so.Property).Count() ?? 0,
                SubComponentGroupWithProperty so => PropertySubIterator?.Invoke(so.Property).Count() ?? 0,
                _ => 0,
            };

    protected override IEnumerable<IterationSubChild> GetChilds(IterationSubChild iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is SubComponentGroup group)
        {
            // Properties

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
                        WriteBranches = WriteBranches
                    };
                    yield return item;
                }
            }
        }
        else if(iterationTarget is SubComponentGroupWithSingleProperty soloSubProp
            && PropertySubIterator != null)
        {
            // TODO : subiterate properties
        }
        else if(iterationTarget is SubComponentGroupWithProperty prop
            && PropertySubIterator != null)
        {
            // TODO : subiterate properties
        }
    }
}
