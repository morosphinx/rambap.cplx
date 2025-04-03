using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.SupplyChain.Outputs;

public static class ManufacturerColumns
{
    public static DelegateColumn<ICplxContent> PartManufacturer() =>
        new DelegateColumn<ICplxContent>("Manufacturer", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Manufacturer()?.Company?.Name ?? "");
}

