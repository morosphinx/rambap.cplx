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
            new Modules.Documentation.DocumentationConcept(),
            new Modules.Documentation.ManufacturerConcept(),
            new Modules.Costing.CostsConcept(),
            new Modules.Costing.TasksConcept(),
            new Modules.Racking.SlotConcept(),
            new Modules.Connectivity.ConnectionConcept(),
        ];
}
