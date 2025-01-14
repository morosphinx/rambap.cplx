using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    private static string CostToString(this decimal cost) => cost.ToString("0.00");

    public static DelegateColumn<IComponentContent> TotalCost() =>
        new DelegateColumn<IComponentContent>("Total Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent {Property : InstanceCost.NativeCostInfo prop } lp => (prop.value.Price * lp.ComponentTotalCount).CostToString(),
                LeafComponent lc => i.AllComponents().Select(c => c.component.Instance.Cost()?.Total ?? 0).Sum().CostToString(),
                BranchComponent bc => "", // Do not display branch costs : subcosts are displayed in properties or component leafs
                                          // And we want to keep the column total cost (when summing the cells themselves) correct
                _ => throw new NotImplementedException(),
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<IComponentContent> CostName(bool include_branches = false)
        => new DelegateColumn<IComponentContent>("Cost Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent {Property : InstanceCost.NativeCostInfo prop} lp => prop.name,
                LeafComponent lc => "unit",
                BranchComponent bc when include_branches => "total per unit",
                BranchComponent bc when !include_branches => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<IComponentContent> UnitCost(bool include_branches = false)
        => new DelegateColumn<IComponentContent>("Unit Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent { Property: InstanceCost.NativeCostInfo prop } lp => prop.value.Price.CostToString(),
                LeafComponent lc =>
                    lc.AllComponentsMatch(c => c.Instance.Cost()?.Total, out var value)
                        ? (value?.CostToString() ?? "")
                        : "error",
                BranchComponent bc when include_branches =>
                    bc.AllComponentsMatch(c => c.Instance.Cost()?.Total, out var value)
                        ? (value?.CostToString() ?? "")
                        : "error",
                BranchComponent bc when !include_branches => "",
                _ => throw new NotImplementedException(),
            });

    public static IColumn<IComponentContent> LocalSumCost()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "SumCost",
            GetLocationText = i => i switch
            {
                IPropertyContent { Property: InstanceCost.NativeCostInfo cost } =>
                        cost.value.Price.CostToString(), // Do not display multiplicity for properties : this is a local cost representation
                (LeafComponent or BranchComponent) when i.Component.Instance.Cost() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Cost()!.Total.CostToString()}"
                        : i.Component.Instance.Cost()!.Total.CostToString(),
                _ => "",
            }
        };

}