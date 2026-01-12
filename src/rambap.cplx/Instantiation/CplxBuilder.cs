using rambap.cplx.Attributes;
using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Instantiation;

public class CplxBuilder
{
    /// <summary>
    /// List of all concepts evaluated when constructing a <see cref="Pinstance"/><br/>
    /// Concept are evaluated in order, witch matter if one of them rely on another's <see cref="IInstanceConceptProperty"/>
    /// </summary>
    public List<IConcept> EvaluatedConcepts { get; init; } =
        [
            new Modules.Documentation.DocumentationConcept(),
            new Modules.SupplyChain.ManufacturerConcept(),
            new Modules.Costing.CostsConcept(),
            new Modules.Costing.TasksConcept(),
            new Modules.Racking.SlotConcept(),
            new Modules.Connectivity.ConnectionConcept(),
        ];

    /// <summary>
    /// Execution Date. <br/>
    /// All generated files should use this parameter as date info. <br/>
    /// </summary>
    public DateTimeOffset GenerationDate { get; init; } = DateTimeOffset.Now;

    /// <summary>
    /// Executing machine name <br/>
    /// </summary>
    public string GenerationMachine { get; init; } = System.Environment.MachineName;

    public AlternativesConfiguration AlternativesConfiguration { get; init; }
        = new AlternativesConfiguration();

    private static string MakeCommment(IEnumerable<ComponentDescriptionAttribute> commentAttributes)
        => string.Join("", commentAttributes.Select(c => c.Text));

    private void MakeSubcomponentsOf(Component owner)
    {
        Part template = owner.Instance.Template;
        // Create components from Parts properties/fields
        ScanObjectContentFor<Part>(template,
            (p, i) =>
            {
                var newComponent = InstantiateComponent_WithProp(owner, p, i);
                owner.AddComponent(newComponent);
            },
            ignoredDerivedTypes: [typeof(IAlternative)] // Avoid matching on alternative, who are IEnumerable<Part>
            );
        // Select and create components from Alternatives properties/fields
        ScanObjectContentFor<IAlternative>(template,
            (a, i) =>
            {
                var selectedPart = AlternativesConfiguration.Decide(a)!;
                var newComponent = InstantiateComponent_WithProp(owner, selectedPart, i);
                owner.AddComponent(newComponent);
            });
    }
    
    internal class ComponentConstructionParam()
    {
        public required string CN { get; init; }
        public string Comment { get; init; } = "";
        public required bool IsPublic { get; init; }
    }

    internal Component InstantiateComponent_WithProp(Component? parent, Part template, PropertyOrFieldInfo i)
        => InstantiateComponent(parent, template, new()
        {
            CN = template.CNOverride ??
                        (i.IsFromAndEnumerable ? $"{i.Name}_{i.IndexInEnumerable:00}" : i.Name),
            Comment = MakeCommment(i.Comments),
            IsPublic = i.IsPublicOrAssembly,
        });
    internal Component InstantiateComponent(Component? parent, Part template, ComponentConstructionParam param)
    {
        if (template.ImplementingComponent != null)
            throw new InvalidOperationException("A Component has already been instantiated with this part");
        template.CplxImplicitInitialization(template); // run the implicit init on this part and all subparts

        // Pinstance is always created new, each component has his own Pinstance ... for now
        var instance = new Pinstance(template);

        Component component = new(parent, instance)
        {
            CN = param.CN,
            Comment = param.Comment,
            IsPublic = param.IsPublic,
        };
        instance.SetUser(component);
        MakeSubcomponentsOf(component);
        // Concepts Evaluation ... as Component have their own instance, for now it's a mixup
        // Concepts Evaluation comes after making subcomponents : Some concepts relies on it
        foreach (var concept in EvaluatedConcepts)
        {
            var property = concept.MakeBase(component);
            if (property != null) instance.properties.Add(property);
        }
        return component;
    }
}
