using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Tables;
using rambap.cplx.Core;
using rambap.cplx.Modules.SupplyChain.Outputs;

namespace rambap.cplx.Modules.Costing.Outputs;
public static partial class CostTables
{
    /// <summary>
    /// Produces a Bill Of Material Table (BOM)
    /// </summary>
    public static TableProducer<ICplxContent> BillOfMaterial(DocumentationPerimeter? perimeter = null)
        => new()
        {
            Iterator = new PartTypesIterator<InstanceCost.CostPoint>()
            {
                WriteBranches = false,
                DocumentationPerimeter = perimeter ?? new(),
                PropertyIterator = (c) => EnumerateCostPoints(c, true),
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.GroupCNs(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.ComponentParentCNs(),
                IDColumns.PartCommonName(), // Or part description ?
                // Manufacturer Info
                CommonColumns.Dashes(""),
                IDColumns.PartNumber(),
                ManufacturerColumns.PartManufacturer(),
                DescriptionColumns.PartLink(), //#TODO : do not reference another concept here
                // Supplier info
                CommonColumns.Dashes(""),
                CostColumns.SelectedOfferSupplier(),
                CostColumns.SelectedOfferSKU(),
                CostColumns.SelectedOfferLink(),
            ],
        };
}
