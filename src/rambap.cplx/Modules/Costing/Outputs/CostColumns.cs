﻿using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    private static string CostToString(this decimal cost) => cost.ToString("0.00");

    public static DelegateColumn<ICplxContent> TotalCost() =>
        new DelegateColumn<ICplxContent>("Total Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceCost.NativeCostInfo> lp =>( lp.Property.value.Price * lp.ComponentTotalCount).CostToString(),
                LeafComponent lc => i.AllComponents().Select(c => c.component.Instance.Cost()?.Total ?? 0).Sum().CostToString(),
                BranchComponent bc => "", // Do not display branch costs : subcosts are displayed in properties or component leafs
                                          // And we want to keep the column total cost (when summing the cells themselves) correct
                _ => throw new NotImplementedException(),
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ICplxContent> CostName(bool displayBranches = false)
        => new DelegateColumn<ICplxContent>("Cost Name", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                IPropertyContent<InstanceCost.NativeCostInfo> lp => lp.Property.name,
                LeafComponent lc => "unit",
                BranchComponent bc when displayBranches => "total per unit",
                BranchComponent bc when !displayBranches => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> UnitCost(bool displayBranches = false)
        => new DelegateColumn<ICplxContent>("Unit Cost", ColumnTypeHint.Numeric,
            i => i switch
            {
                IPropertyContent<InstanceCost.NativeCostInfo> lp => lp.Property.value.Price.CostToString(),
                BranchComponent bc when !displayBranches => "",
                IPureComponentContent lc =>
                    lc.AllComponentsMatch(c => c.Instance.Cost()?.Total, out var value)
                        ? (value?.CostToString() ?? "")
                        : "error",
                _ => throw new NotImplementedException(),
            });

    public static IColumn<ICplxContent> LocalSumCost()
        => new CommonColumns.ComponentPrettyTreeColumn()
        {
            Title = "SumCost",
            GetLocationText = i => i switch
            {
                IPropertyContent<InstanceCost.NativeCostInfo> lp =>
                    lp.Property.value.Price.CostToString(), // Do not display multiplicity for properties : this is a local cost representation
                IPureComponentContent when i.Component.Instance.Cost() is not null =>
                    i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.Cost()!.Total.CostToString()}"
                        : i.Component.Instance.Cost()!.Total.CostToString(),
                _ => "",
            }
        };

}