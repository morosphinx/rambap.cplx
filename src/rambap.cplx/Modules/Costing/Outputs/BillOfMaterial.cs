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
                // Component CN
                CommonColumns.ComponentTotalCount(),
                // Component parents CNs
                DescriptionColumns.PartCommonName(),
                // Or part description ?
                // Spacer
                IDColumns.PartNumber(),
                // Manufacturer
                // Manufacturer Link
                // Spacer
                // Supplier (chosen)
                // Supplier SKU
                // Supplier Link
            ],
        };
}
