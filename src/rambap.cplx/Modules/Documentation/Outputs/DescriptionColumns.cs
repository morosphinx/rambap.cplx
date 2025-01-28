using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    private static string GetAllLineDescription(Pinstance instance)
    {
        var descriptions = instance.Documentation()?.Descriptions.Select(d => d.Text)
                    ?? new List<string>();
        return string.Join("\r\n", descriptions);
    }
    private static string GetSingleLineDescription(Pinstance instance)
    {
        return instance.Documentation()?.Descriptions.FirstOrDefault()?.Text
                    ?? "";
    }

    public static DelegateColumn<IComponentContent> PartDescription(bool allLines = false) =>
        new DelegateColumn<IComponentContent>("Part Description", ColumnTypeHint.StringFormatable,
            i => allLines
                ? GetAllLineDescription(i.Component.Instance)
                : GetSingleLineDescription(i.Component.Instance));

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

