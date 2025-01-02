using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Costing.Outputs;
public static class CostColumns
{
    public static DelegateColumn<ComponentContent> TotalCost() =>
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
                               // And we want to keep the column total cost (when summing the cells themselves) correct
                }
            },
            i => i.Cost()?.Total.ToString("0.00"));

    public static DelegateColumn<ComponentContent> CostName(bool include_branches = false)
        => new DelegateColumn<ComponentContent>("Cost Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafProperty lpi)
                {
                    if (lpi.Property is InstanceCost.NativeCostInfo n)
                        return n.name;
                    else
                        throw new NotImplementedException();
                }
                else if (i is LeafComponent lp)
                {
                    return "unit";
                }
                else if (i is BranchComponent bp)
                {
                    if (!include_branches) return "";
                    return "total per unit";
                }
                else throw new NotImplementedException();
            });

    public static DelegateColumn<ComponentContent> UnitCost(bool include_branches = false)
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
                // TODO : remove code duplication
                // TODO : clarify wether taking care of an incoherent component group is the columns
                // responsability or not
                else if (i is LeafComponent lp)
                {
                    var costs = lp.AllComponents().Select(i => i.component.Instance.Cost()?.Total).ToList();
                    // Parts may be edited, without changing the PN => This would be a mistake, detect it
                    bool costAreCoherent = costs.Distinct().Count() <= 1;
                    if (costAreCoherent) return costs.First()?.ToString("0.00") ?? "";
                    else return "error";
                }
                else if (i is BranchComponent bp)
                {
                    if (!include_branches) return "";
                    var costs = bp.AllComponents().Select(i => i.component.Instance.Cost()?.Total).ToList();
                    // Parts may be edited, without changing the PN => This would be a mistake, detect it
                    bool costAreCoherent = costs.Distinct().Count() <= 1;
                    if (costAreCoherent) return costs.First()?.ToString("0.00") ?? "";
                    else return "error";
                }
                else throw new NotImplementedException();
            });

}