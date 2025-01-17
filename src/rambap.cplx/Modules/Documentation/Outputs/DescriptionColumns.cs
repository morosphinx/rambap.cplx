﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    private static string GetDescription(Pinstance instance)
    {
        var descriptions = instance.Documentation()?.Descriptions.Select(d => d.Text)
                    ?? new List<string>();
        return string.Join(" ", descriptions);
    }

    public static DelegateColumn<IComponentContent> PartDescription() =>
        new DelegateColumn<IComponentContent>("Part Description", ColumnTypeHint.StringFormatable,
            i => GetDescription(i.Component.Instance));

    public static DelegateColumn<IComponentContent> PartCommonName(bool hideIfEqualPN =false) =>
        new DelegateColumn<IComponentContent>("Part Common Name", ColumnTypeHint.StringFormatable,
            i =>
            {
                var instance = i.Component.Instance;
                if(hideIfEqualPN)
                    return instance.CommonName == instance.PN ? "" : instance.CommonName;
                else return instance.CommonName;
            });

    public static DelegateColumn<IComponentContent> PartLink() =>
        new DelegateColumn<IComponentContent>("Link", ColumnTypeHint.StringExact,
            i => i.Component.Instance.Documentation()?.Links.FirstOrDefault()?.Text ?? "");
}

