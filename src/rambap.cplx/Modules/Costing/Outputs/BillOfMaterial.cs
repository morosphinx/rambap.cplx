using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Tables;
using rambap.cplx.Core;

namespace rambap.cplx.Modules.Costing.Outputs;
public static partial class CostTables
{
    /// <summary>
    /// Produces a Bill Of Material Table (BOM)
    /// </summary>
    public static TableProducer<ICplxContent> BillOfMaterial(DocumentationPerimeter? perimeter = null)
        => new()
        {
            Iterator = new PartTypesIterator<InstanceCost.NativeCostInfo>()
            {
                WriteBranches = false,
                DocumentationPerimeter = perimeter ?? new(),
                PropertyIterator = ListCostOr0
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.GroupCNs(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.ComponentParentCNs(),
                IDColumns.PartCommonName(),
                // Or part description ?
                CommonColumns.Dashes(""),
                IDColumns.PartNumber(),
                // Manufacturer
                DescriptionColumns.PartLink(), //#TODO : do not reference another concept here
                CommonColumns.Dashes(""),
                // Supplier (chosen)
                // Supplier SKU
                // Supplier Link
            ],
        };
}
