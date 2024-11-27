namespace rambap.cplx.Export.Spreadsheet;

internal static partial class Helpers
{
    // GetExcelColumnName() is from here :
    // https://stackoverflow.com/questions/181596/how-to-convert-a-column-number-e-g-127-into-an-excel-column-e-g-a/182924#182924
    // See Licenses pertaining to this specific function in the link above
    internal static string GetExcelColumnName(int columnNumber)
    {
        string columnName = "";
        while (columnNumber > 0)
        {
            int modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }
        return columnName;
    }
}

