using rambap.cplx.Attributes;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Core;

/// <summary>
/// Component Instance. The realisation of a Component in the hierarchy, with calculated relations
/// </summary>
public class Component
{
    private static string MakeCommment(IEnumerable<ComponentDescriptionAttribute> commentAttributes)
        => string.Join("", commentAttributes.Select(c => c.Text));

    internal Component(Component? parent, Part template, AlternativesConfiguration conf)
    {
        if (template.ImplementingComponent != null)
            throw new InvalidOperationException("A Component has already been instantiated with this part");
        template.CplxImplicitInitialization(parent?.Template); // run the implicit init on this part and all subparts
        template.ImplementingComponent = this;
        // Set relationships
        Parent = parent;
        Template = template;

        // Create components from Parts properties/fields
        ScanObjectContentFor<Part>(template,
            (p, i) =>
                subComponents.Add(
                new Component(this, p, conf)
                {
                    CN = p.CNOverride ??
                        (i.IsFromAndEnumerable ? $"{i.Name}_{i.IndexInEnumerable:00}" : i.Name),
                    Comment = MakeCommment(i.Comments),
                    IsPublic = i.IsPublicOrAssembly,
                }),
            ignoredDerivedTypes: [typeof(IAlternative)] // Avoid matching on alternative, who are IEnumerable<Part>
            );

        // Select and create components from Alternatives properties/fields
        ScanObjectContentFor<IAlternative>(template,
            (a, i) =>
            {
                var selectedPart = conf.Decide(a)!;
                subComponents.Add(
                new Component(this, selectedPart, conf)
                {
                    CN = selectedPart.CNOverride ??
                        (i.IsFromAndEnumerable ? $"{i.Name}_{i.IndexInEnumerable:00}" : i.Name),
                    Comment = MakeCommment(i.Comments),
                    IsPublic = i.IsPublicOrAssembly,
                });
            });
        // Set the instance now, so that the Component <-> instance relation is correct before calculations
        Instance = new Pinstance(this, template, conf);
        // Run the concept calculations
        Instance.RunConceptEvaluation();
    }

    /// <summary>
    /// Definition if this component 
    /// </summary>
    /// Rigth now, Pinstances are created as unique C# class instance, due to Parts also begin created
    /// as unique instances of the Part class, even when reused identicaly in diferent contexts.
    /// This is wastefull, and could one day be optimised to allow components to share Pinstance class instances.
    /// Therefore a Component IS NOT a Pinstance itself, but point to one.
    public Pinstance Instance { get; }

    /// <summary>
    /// Component that contains this component in his SubComponents <br/>
    /// If null, this is the root component.
    /// </summary>
    internal Component? Parent { get; }

    // TODO : define if relevant to keep. Needed if we want to add more parts to the component during
    // concept calculation, as this require setting the parent part
    internal Part Template { get; }

    /// <summary>
    /// Component Number : Identifier of this component in its owner
    /// </summary>
    public required string CN { get; init; }

    public string CID(string separator = Core.CID.Separator)
    {
        if (Parent == null)
            return CN;
        else
            return Parent!.CID() + separator + CN;
    }

    /// <summary>
    /// Bubble to the topmost component, creating a stack of components with the root-most on top
    /// </summary>
    private Stack<Component> GetHierarchy()
    {
        Stack<Component> hierarchy = new();
        var currentComponent = this;
        do
        {
            hierarchy.Push(currentComponent);
            currentComponent = currentComponent?.Parent;
        } while (currentComponent != null);
        return hierarchy;
    }

    /// <summary>
    /// Return the CID of the current component, removing all identifier not in the documentation perimeter from the CID
    /// </summary>
    public string CID(DocumentationPerimeter perimeter, string separator = Core.CID.Separator)
    {
        var hierarchy = GetHierarchy();
        var currentComponent = hierarchy.Pop();
        var CID = currentComponent.CN;
        while (hierarchy.Count > 0
            && perimeter.ShouldThisComponentInternalsBeSeen(currentComponent))
        {
            currentComponent = hierarchy.Pop();
            CID += separator + currentComponent.CN;
        }
        return CID;
    }

    /// <summary>
    /// Comment relative to this component - eg : his purpose or usage in its owner
    /// </summary>
    public string Comment { get; init; } = "";

    /// <summary>
    /// True if this component is public or internal from this part : eg, it is visible from outside the containing Part,
    /// and therefore may be condidered to be part or the public interface of the Part.
    /// </summary>
    public required bool IsPublic { get; init; }

    /// <summary>
    /// Immediate sub-Components of this component. All are owned by this component.
    /// </summary>
    public IEnumerable<Component> SubComponents => subComponents;
    internal void AddConceptPart(Part part)
    {
        // Create Component
        var backupCN = subComponents.Count.ToString();
        var newComponent = new Component(this, part, new AlternativesConfiguration())
        {
            CN = $"@AUTO:{(string.IsNullOrEmpty(part.CNOverride) ? backupCN : part.CNOverride)}",
            IsPublic = false,
        };
        // TBD / TODO : add to another component list ? 
        subComponents.Add(newComponent);
    }

    private List<Component> subComponents { get; } = new();

    /// <summary>
    /// Wrapper for <see cref="Pinstance.PN"/> <br/>
    /// <inheritdoc cref="Pinstance.PN"/> 
    /// </summary>
    public string PN => Instance.PN;
}

