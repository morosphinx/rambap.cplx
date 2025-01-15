using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class ManufacturerColumns
{
    public static DelegateColumn<ComponentContent> PartManufacturer() =>
        new DelegateColumn<ComponentContent>("Manufacturer", ColumnTypeHint.StringFormatable,
            i => i.Component.Instance.Manufacturer()?.Company?.Name ?? "");
}

