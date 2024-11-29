using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class Manufacturers
{
    public static DelegateColumn<PartTreeItem> PartManufacturer() =>
        new DelegateColumn<PartTreeItem>("Manufacturer", ColumnTypeHint.String,
            i => i.PrimaryItem.Component.Instance.Manufacturer()?.Company?.Name ?? "");

    public static DelegateColumn<ComponentTreeItem> ComponentManufacturer() =>
        new DelegateColumn<ComponentTreeItem>("Manufacturer", ColumnTypeHint.String,
            i => i.Component.Instance.Manufacturer()?.Company?.Name ?? "");
}

