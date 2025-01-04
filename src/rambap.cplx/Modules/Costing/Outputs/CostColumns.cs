using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    private static string CostToString(this decimal cost) => cost.ToString("0.00");

    public static DelegateColumn<ComponentContent> TotalCost() =>
        new DelegateColumn<ComponentContent>("Total Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                LeafProperty {Property : InstanceCost.NativeCostInfo prop } lp => (prop.value.Price * lp.ComponentTotalCount).CostToString(),
                LeafComponent lc => i.AllComponents().Select(c => c.component.Instance.Cost()?.Total ?? 0).Sum().CostToString(),
                BranchComponent bc => "", // Do not display branch costs : subcosts are displayed in properties or component leafs
                                          // And we want to keep the column total cost (when summing the cells themselves) correct
                _ => throw new NotImplementedException(),
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ComponentContent> CostName(bool include_branches = false)
        => new DelegateColumn<ComponentContent>("Cost Name", ColumnTypeHint.String,
            i => i switch
            {
                LeafProperty {Property : InstanceCost.NativeCostInfo prop} lp => prop.name,
                LeafComponent lc => "unit",
                BranchComponent bc when include_branches => "total per unit",
                BranchComponent bc when !include_branches => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ComponentContent> UnitCost(bool include_branches = false)
        => new DelegateColumn<ComponentContent>("Unit Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                LeafProperty { Property: InstanceCost.NativeCostInfo prop } lp => prop.value.Price.CostToString(),
                LeafComponent lc => lc.AllMatchOrNull(c => c.Instance.Cost()?.Total)?.CostToString() ?? "error",
                BranchComponent bc => bc.AllMatchOrNull(c => c.Instance.Cost()?.Total)?.CostToString() ?? "error",
                _ => throw new NotImplementedException(),
            });

}