using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.SupplyChain.Outputs;

public static class SupplierColumns
{
    public static DelegateColumn<ICplxContent> SupplierName() =>
        new DelegateColumn<ICplxContent>("Supplier", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Cost()?.SelectedOffer?.Supplier.Company.Name ?? "");
    public static DelegateColumn<ICplxContent> SupplierPN() =>
        new DelegateColumn<ICplxContent>("SupplierPN", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Cost()?.SelectedOffer?.SKU ?? "");
    public static DelegateColumn<ICplxContent> SupplierLink() =>
        new DelegateColumn<ICplxContent>("SupplierLink", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Cost()?.SelectedOffer?.Link ?? "");
}
