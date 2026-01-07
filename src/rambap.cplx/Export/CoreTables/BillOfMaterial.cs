using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Modules.SupplyChain.Outputs;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.Instantiation;

namespace rambap.cplx.Export.CoreTables;

/// <summary>
/// Produces a Bill Of Material Table (BOM)
/// </summary>
public record class BillOfMaterial : TableProducer<IContent>
{
    [SetsRequiredMembers]
    public BillOfMaterial(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new PartTypesIterator<InstanceCost.CostPoint>()
        {
            DocumentationPerimeter = perimeter ?? new(),
            PropertyIterator = (c) => CostBreakdown.EnumerateCostPoints(c, true),
        };
        ContentTransform = cs
            => cs.Where(c => c.IsLeaf); // Remove branch items
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
