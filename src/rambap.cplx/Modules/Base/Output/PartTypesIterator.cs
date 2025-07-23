using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.Output;


/// <summary>
/// Produce an IEnumerable iterating over parts used as subcomponents of an instance, and properties of those parts. <br/>
/// Output is structured like a list of <see cref="PartContent"/>.
/// </summary>
/// <typeparam name="P">Enumerated property Type. Set to object if none</typeparam>
public class PartTypesIterator<P> : IContentIterator<IContent>
{
    /// <summary>
    /// Define when to recurse on components (will return properties items and subcomponents items) and when not to (will only return the component item)
    /// If null, always recurse
    /// </summary>
    public DocumentationPerimeter DocumentationPerimeter { private get; init; } = new();

    /// <summary>
    /// Define a final level of iteration on of parts that return properties
    /// Leave this empty to return no properties items
    /// </summary>
    public Func<Component, IEnumerable<P>>? PropertyIterator { private get; init; }
    private bool IsAPropertyTable => PropertyIterator != null;

    /// <summary>
    /// Two <see cref="CplxContent"/> with the same <see cref="ComponentTemplateUnicityIdentifier"/> should be assumed
    /// to have the same template part <br/>
    /// We test the the Type of <see cref="CplxContent"/> to avoid mixing leaf and branch contents.
    /// </summary>
    static (Type, string, Type) ComponentTemplateUnicityIdentifier(IContent c)
        => (c.Component.Instance.PartType, c.Component.Instance.PN, c.GetType());

    public IEnumerable<IContent> MakeContent(Component component)
    {
        // Produce a tree table of All Components, stopping on recursing condition.
        var ComponentTable = new ComponentIterator()
        {
            DocumentationPerimeter = DocumentationPerimeter,
            AlwaysRecurseDepth0 = false
            // No property iteration when iterating the component tree
            // => Will return only LeafComponent or BranchComponent
        };
        var componentsItems = ((IContentIterator<IContent>)ComponentTable).MakeContent_AsFlat(component);

        // All returned items of the tree table represent components (eg : No LeafProperty)
        // Group the components by Identity (PN & Type & content kind)
        var grouping_by_pn = componentsItems.GroupBy(ComponentTemplateUnicityIdentifier);
        // For each group, produce a PartTreeItem
        foreach (var pnGroup in grouping_by_pn)
        {
            // Groups have same PN, same PartType
            var pnGroupComponents = pnGroup.SelectMany(c => c.AllComponents());
            ContentLocation pnGroupLocation = new()
            {
                CIN = $"",
                Multiplicity = pnGroup.Sum(c => c.Location.Multiplicity),
                Depth = 0,
            };

            // Recursion on the ComponentTree may depend on part location.
            // So we may have a mix of broken and non broken recursion here
            var isPnGroupRecursionBreak = pnGroup.All(c => c.IsRecursionBreak) ;
            if (isPnGroupRecursionBreak)
            {
                // Group is solely made of content that blocked recursion
                // => We do not want to see what's inside
                yield return new CplxContent(pnGroupLocation, pnGroupComponents) {  ContentIterator = new DoNothingIterator() };
            }
            else
            {
                // PN Group may me recursed into
                // => Return a content with this has Iterator, so we may SubIterate
                yield return new CplxContent(pnGroupLocation, pnGroupComponents) { ContentIterator = this };
            }
        }
    }

    public IEnumerable<IContent> MakeSubContent(IContent content)
    {
        // Iterate properties
        if(PropertyIterator != null && ! content.IsRecursionBreak && content is not IPropertyContent<P>)
        {
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
        // TBD : subiterate properties ?
    }

    public bool ShouldTryRecurse(IContent content)
        => true;
}


