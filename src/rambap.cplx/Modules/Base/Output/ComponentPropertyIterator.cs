using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;

class ComponentPropertyIterator<T> : ComponentIterator
{
    protected class SubComponentGroupWithProperty : SubComponentGroup
    {
        public required T Property { get; init; }
    }

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public required Func<Component, IEnumerable<T>> PropertyIterator { private get; init; }

    // TODO : Redo this behavior
    public bool StackPropertiesSingleChildBranches { private get; init; } = true;


    protected override IEnumerable<IComponentContent> GetRecursionBreakContent(IterationSubChild iterationTarget)
    {
        if (iterationTarget is SubComponentGroupWithProperty group)
        {
            yield return new LeafComponentWithProperty<T>(group.Location, group.Components)
            {
                Property = group.Property, IsLeafBecause = LeafCause.NoChild
            };
        }
        else
        {
            foreach(var i in base.GetRecursionBreakContent(iterationTarget))
                yield return i;
        }
    }

    protected override IEnumerable<IComponentContent> GetRecursionContinueContent(IterationSubChild iterationTarget, LocationBuilder subItemLocationBuilder, List<IterationSubChild> subItems)
    {
        if (iterationTarget is SubComponentGroupWithProperty group)
        {
            yield return new LeafComponentWithProperty<T>(group.Location, group.Components)
            {
                Property = group.Property,
                IsLeafBecause = LeafCause.NoChild
            };
        }
        else
        {
            foreach (var i in base.GetRecursionContinueContent(iterationTarget, subItemLocationBuilder, subItems))
                yield return i;
        }
    }

    protected override int ExpectedchildCount(IterationSubChild iterationTarget)
        =>
            base.ExpectedchildCount(iterationTarget)
            + iterationTarget switch
            {
                SubComponentGroup group => PropertyIterator(group.MainComponent).Count(),
                _ => 0,
            };

    protected override IEnumerable<IterationSubChild> GetChilds(IterationSubChild iterationTarget, LocationBuilder loc)
    {
        if (iterationTarget is SubComponentGroup group)
        {
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
                // yield return new LeafComponentWithProperty<T>(propLocation, group.Components)
                // {
                //     Property = prop, IsLeafBecause = LeafCause.NoChild
                // };
            }
        }

        // call base => return subcomponents
        foreach (var b in base.GetChilds(iterationTarget, loc))
            yield return b;
    }
}
