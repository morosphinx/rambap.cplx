using rambap.cplx.Core;
using rambap.cplx.Attributes;
using System.Reflection;

namespace rambap.cplx.Modules.Base.Output;


/// <summary>
/// Produce an IEnumerable iterating over the component tree of an instance, and its properties <br/>
/// Output is structured like a tree of <see cref="CplxContent"/>. <br/>
/// </summary>
public class ComponentIterator : IContentIterator<IContent>
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
    public DocumentationPerimeter DocumentationPerimeter { private get; init; } = new();
    public bool AlwaysRecurseDepth0 { get; set; } = true;


    public virtual bool ShouldRecurse(IContent currentContent)
    {
        var mainComponent = currentContent.Component;
        var location = currentContent.Location;

        bool mayRecursePastThis =
            (location.Depth == 0 && AlwaysRecurseDepth0) || // Always recurse the first iteration (root node), no mater the recursion condition
            DocumentationPerimeter.ShouldThisComponentInternalsBeSeen(mainComponent);
        return mayRecursePastThis;
    }

    protected IEnumerable<IEnumerable<Component>> MakeSubComponentGroups(IEnumerable<Component> components)
    {
        var subcomponents = components.First().SubComponents;
        var subcomponentContents = GroupPNsAtSameLocation switch
        {
            false => subcomponents.Select<Component, IEnumerable<Component>>(c => [c]),
            true => subcomponents.GroupBy(c => (c.Instance.PartType, c.Instance.PN)).Select(g => g.Select(c => c)),
        };
        return subcomponentContents;
    }

    public virtual IEnumerable<IContent> MakeSubContent(IContent content)
    {
        // prepare subcomponents contents. Group them by same PartType & PN if configured :
        var subcomponentContents = MakeSubComponentGroups(content.AllComponents());
        foreach (var i in subcomponentContents)
        {
            var subItemLocation = content.GetNextLocation() ;
            var item = new CplxContent(subItemLocation, i) { ContentIterator = this };
            yield return item;
        }
    }

    public IEnumerable<IContent> MakeContent(Component rootComponent)
    {
        // Set the current documentation perimeter params
        DocumentationPerimeter.CurrentRootPartType = rootComponent.Instance.PartType;


        ContentLocation rootLocation = new()
        {
            CIN = $"",
            Multiplicity = 1,
            Depth = 0,
        };
        CplxContent rootItem = new(rootLocation, [rootComponent]){ ContentIterator = this };
        return [rootItem];
    }
}


