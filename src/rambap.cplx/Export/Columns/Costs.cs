using rambap.cplx;
using rambap.cplx.Export.Iterators;
using System.Linq.Expressions;

namespace rambap.cplx.Export.Columns;
public static class Costs
{
    public static DelegateColumn<ComponentTreeItem> CostBreakdown_Name()
    => new DelegateColumn<ComponentTreeItem>("Detail", ColumnTypeHint.String,
        i =>
        {
            if (i is LeafProperty lp)
            {
                if (lp.Property is Concepts.InstanceCost.NativeCostInfo n)
                    return n.name;
            }
            else if (i is LeafComponent lc)
                return "unit";
            return "";
        });

    public static DelegateColumn<ComponentTreeItem> CostBreakdown_Value()
        => new DelegateColumn<ComponentTreeItem>("Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafProperty lp)
                {
                    if (lp.Property is Concepts.InstanceCost.NativeCostInfo n)
                        return n.value.price.ToString("0.00");
                }
                else if (i is LeafComponent lc)
                    return lc.Component?.Instance.Cost()?.Total.ToString("0.00") ?? "";
                return "";
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ComponentTreeItem> TotalCost() =>
        new DelegateColumn<ComponentTreeItem>("Cost", ColumnTypeHint.Numeric,
            i => i.Component.Instance.Cost()?.Total.ToString("0.00"),
            i => i.Cost()?.Total.ToString());

    public static DelegateColumn<PartTreeItem> GroupTotalCost() =>
        new DelegateColumn<PartTreeItem>("Total Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPartPropertyTableItem lp)
                {
                    if (lp.Property is Concepts.InstanceCost.NativeCostInfo prop)
                    {
                        return (prop.value.price * lp.Items.Count()).ToString("0.00");
                    }
                    else
                    {
                        return "";
                    }

                }
                else if (i is LeafPartTableItem)
                {
                    var prices = i.Items.Select(c => c.Component.Instance.Cost()?.Total ?? 0);
                    return prices.Sum().ToString("0.00");
                }
                else
                {
                    return ""; // Do not display branch costs : subcosts are displayed in properties or component leafs
                }
            });

    public static DelegateColumn<PartTreeItem> Group_CostName()
        => new DelegateColumn<PartTreeItem>("Detail", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceCost.NativeCostInfo n)
                        return n.name;
                }
                else if (i is LeafPartTableItem lp)
                {
                    return "unit";
                }
                return "";
            });

    public static DelegateColumn<PartTreeItem> Group_UnitCost()
        => new DelegateColumn<PartTreeItem>("Unit Cost", ColumnTypeHint.Numeric,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceCost.NativeCostInfo n)
                        return n.value.price.ToString("0.00");
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafPartTableItem lp)
                {
                    var costs = lp.Items.Select(i => i.Component.Instance.Cost()?.Total).ToList();
                    // Parts may be edited, without changing the PN => This would be a mistake, detect it
                    bool costAreCoherent = costs.Distinct().Count() <= 1;
                    if (costAreCoherent) return costs.First()?.ToString("0.00") ?? "";
                    else return "error";
                }
                return "";
            },
            i => i.Cost()?.Total.ToString("0.00"));

}