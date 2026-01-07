using rambap.cplx.Instantiation;

namespace rambap.cplx.Modules.Base.Output;

/// <summary>
/// Enumerate component trees like a <see cref="ComponentIterator"/>, plus, on each component, iterate properties with <br/>
/// 1 - <see cref="PropertyIterator"/> <br/>
/// 2 (Optional) - <see cref="PropertySubIterator"/> <br/>
/// </summary>
/// <typeparam name="P">Enumerated property Type.</typeparam>
public class ComponentPropertyIterator<P> : ComponentIterator
{

    /// <summary>
    /// Define a final level of iteration on of components
    /// Leave this empty to return no properties items
    /// </summary>
    public required Func<Component, IEnumerable<P>> PropertyIterator { private get; init; }

    public bool StackPropertiesSingleChildBranches { private get; init; } = true;

    public Func<P, IEnumerable<P>>? PropertySubIterator { private get; init; }

    public override IEnumerable<IContent> MakeSubContent(IContent content)
    {
        if(content is not BranchProperty<P>)
        {
            // Is NOT a property content : regular component iteration first
            foreach (var cg in base.MakeSubContent(content))
            {
                if (StackPropertiesSingleChildBranches)
                {
                    var mainComponent = cg.Component;
                    // May stack the property if applicable
                    var expectedChilds = base.MakeContent(mainComponent);
                    var expectedProperties = PropertyIterator(mainComponent);
                    if(expectedProperties.Count() == 1 && expectedChilds.Count() == 0)
                        yield return new BranchProperty<P>(cg.Location,cg.AllComponents())
                        {
                            ContentIterator = this,
                            Property = expectedProperties.Single(),
                            IsSingleStackedPropertyChild = true,
                        };
                    else
                        yield return cg;
                }
                else
                    yield return cg;
            }
            // Iterate Properties
            var propertiesContents = PropertyIterator(content.Component);
            var nextLocation = content.GetNextLocation();
            foreach (var prop in propertiesContents)
            {
                yield return new BranchProperty<P>(nextLocation, content.AllComponents())
                {
                    ContentIterator = this,
                    Property = prop,
                };
            }
        }
        else if(content is BranchProperty<P> bp)
        {
            // SubIterate properties
            if (PropertySubIterator is null)
                yield break;
            var propertiesContents = PropertySubIterator(bp.Property);
            var nextLocation = content.GetNextLocation();
            foreach (var prop in propertiesContents)
            {
                yield return new BranchProperty<P>(nextLocation, content.AllComponents())
                {
                    ContentIterator = this,
                    Property = prop,
                };
            }
        }
    }
}
