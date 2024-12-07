﻿using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class ManufacturerColumns
{
    public static DelegateColumn<PartContent> PartManufacturer() =>
        new DelegateColumn<PartContent>("Manufacturer", ColumnTypeHint.String,
            i => i.PrimaryItem.Component.Instance.Manufacturer()?.Company?.Name ?? "");

    public static DelegateColumn<ComponentContent> ComponentManufacturer() =>
        new DelegateColumn<ComponentContent>("Manufacturer", ColumnTypeHint.String,
            i => i.Component.Instance.Manufacturer()?.Company?.Name ?? "");
}
