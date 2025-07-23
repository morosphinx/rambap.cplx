using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    private static string CostToString(this decimal cost) => cost.ToString("0.00");

    public static DelegateColumn<IContent> TotalCost() =>
        new DelegateColumn<IContent>("Total Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceCost.CostPoint> lp => (lp.Property.Value.Price * lp.ComponentTotalCount).CostToString(),
                BranchComponent lc when lc.IsLeaf => i.AllComponents().Select(c => c.component.Instance.Cost()?.TotalCost ?? 0).Sum().CostToString(),
                BranchComponent bc when bc.IsBranch => "", // Do not display branch costs : subcosts are displayed in properties or component leafs
                                          // And we want to keep the column total cost (when summing the cells themselves) correct
                _ => throw new NotImplementedException(),
            },
            i => i.Cost()?.TotalCost.ToString("0.00"));

    public static DelegateColumn<IContent> CostName(bool displayBranches = false)
        => new DelegateColumn<IContent>("Cost Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceCost.CostPoint> lp => lp.Property.Name,
                BranchComponent lc when lc.IsLeaf => "unit",
                BranchComponent bc when displayBranches => "total per unit",
                BranchComponent bc when !displayBranches => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<IContent> UnitCost(bool displayBranches = false)
        => new DelegateColumn<IContent>("Unit Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceCost.CostPoint> lp => lp.Property.Value.Price.CostToString(),
                BranchComponent bc when bc.IsBranch && !displayBranches => "",
                BranchComponent lc when lc.IsLeaf =>
                    lc.AllComponentsMatch(c => c.Instance.Cost()?.TotalCost, out var value)
                        ? (value?.CostToString() ?? "")
                        : "error",
                _ => throw new NotImplementedException(),
            });

    public static IColumn<IContent> LocalSumCost()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "SumCost",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceCost.CostPoint> lp =>
                    lp.Property.Value.Price.CostToString(), // Do not display multiplicity for properties : this is a local cost representation
                BranchComponent when i.Component.Instance.Cost() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Cost()!.TotalCost.CostToString()}"
                        : i.Component.Instance.Cost()!.TotalCost.CostToString(),
                _ => "",
            }
        };

    public static DelegateColumn<IContent> SelectedOfferSupplier()
        => new DelegateColumn<IContent>("Supplier", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Cost()?.SelectedOffer?.Supplier.Company.Name ?? "" );
    public static DelegateColumn<IContent> SelectedOfferSKU()
        => new DelegateColumn<IContent>("SKU", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Cost()?.SelectedOffer?.SKU ?? "");
    public static DelegateColumn<IContent> SelectedOfferLink()
    => new DelegateColumn<IContent>("Supplier Link", ColumnTypeHint.StringFormatable,
        i => i.Component.Instance.Cost()?.SelectedOffer?.Link ?? "");

}