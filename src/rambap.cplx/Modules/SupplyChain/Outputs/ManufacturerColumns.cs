using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.SupplyChain.Outputs;

public static class ManufacturerColumns
{
    public static DelegateColumn<ICplxContent> PartManufacturer() =>
        new DelegateColumn<ICplxContent>("Manufacturer", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Manufacturer()?.Company?.Name ?? "");
}

