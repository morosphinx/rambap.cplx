﻿using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    public static DelegateColumn<ComponentContent> CostBreakdown_Name()
    => new DelegateColumn<ComponentContent>("Cost Name", ColumnTypeHint.String,
        i =>
        {
            if (i is LeafProperty lp)
            {
                if (lp.Property is InstanceCost.NativeCostInfo n)
                    return n.name;
            }
            else if (i is LeafComponent lc)
            {
                if (lc.IsLeafBecause == LeafCause.RecursionBreak)
                    return "unit";
                else
                    return "";
            }
            return "";
        });

    public static DelegateColumn<ComponentContent> CostBreakdown_Value()
        => new DelegateColumn<ComponentContent>("Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lp)
                {
                    if (lp.Property is InstanceCost.NativeCostInfo n)
                        return n.value.Price.ToString("0.00");
                }
                else if (i is LeafComponent lc)
                    return lc.Component?.Instance.Cost()?.Total.ToString("0.00") ?? "";
                return "";
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ComponentContent> TotalCost() =>
        new DelegateColumn<ComponentContent>("Cost", ColumnTypeHint.Numeric,
            i => i.Component.Instance.Cost()?.Total.ToString("0.00"),
            i => i.Cost()?.Total.ToString());

    public static DelegateColumn<ComponentContent> GroupTotalCost() =>
        new DelegateColumn<ComponentContent>("Total Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lp)
                {
                    if (lp.Property is InstanceCost.NativeCostInfo prop)
                    {
                        return (prop.value.Price * lp.ComponentTotalCount).ToString("0.00");
                    }
                    else
                    {
                        return "";
                    }

                }
                else if (i is LeafComponent)
                {
                    var prices = i.AllComponents().Select(c => c.component.Instance.Cost()?.Total ?? 0);
                    return prices.Sum().ToString("0.00");
                }
                else
                {
                    return ""; // Do not display branch costs : subcosts are displayed in properties or component leafs
                }
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ComponentContent> Group_CostName()
        => new DelegateColumn<ComponentContent>("Cost Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceCost.NativeCostInfo n)
                        return n.name;
                }
                else if (i is LeafComponent lp)
                {
                    return "unit";
                }
                return "";
            });

    public static DelegateColumn<ComponentContent> Group_UnitCost()
        => new DelegateColumn<ComponentContent>("Unit Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceCost.NativeCostInfo n)
                        return n.value.Price.ToString("0.00");
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafComponent lp)
                {
                    var costs = lp.AllComponents().Select(i => i.component.Instance.Cost()?.Total).ToList();
                    // Parts may be edited, without changing the PN => This would be a mistake, detect it
                    bool costAreCoherent = costs.Distinct().Count() <= 1;
                    if (costAreCoherent) return costs.First()?.ToString("0.00") ?? "";
                    else return "error";
                }
                return "";
            });

}