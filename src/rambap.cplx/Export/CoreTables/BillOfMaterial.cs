using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Modules.SupplyChain.Outputs;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

/// <summary>
/// Produces a Bill Of Material Table (BOM)
/// </summary>
public record class BillOfMaterial : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public BillOfMaterial(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new PartTypesIterator<InstanceCost.CostPoint>()
        {
            WriteBranches = false,
            DocumentationPerimeter = perimeter ?? new(),
            PropertyIterator = (c) => CostBreakdown.EnumerateCostPoints(c, true),
        };
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
            ];
    }
}
