using rambap.cplx.Core;

namespace rambap.cplx;

public static class Globals
{
    /// <summary>
    /// Global list of all concepts evaluated when constructing a <see cref="Pinstance"/><br/>
    /// This list must be set before calling a <see cref="Pinstance"/> constructor.<br/>
    /// Concept are evaluated in order, witch matter if one of them rely on another's <see cref="IInstanceConceptProperty"/>
    /// </summary>
    public static List<IConcept> EvaluatedConcepts =
        [
            new Concepts.Documentation.DocumentationConcept(),
            new Concepts.Documentation.ManufacturerConcept(),
            new Concepts.Costing.CostsConcept(),
            new Concepts.Costing.TasksConcept(),
            new Concepts.Connectivity.SlotConcept(),
            new Concepts.Connectivity.ConnectionConcept(),
        ];
}
