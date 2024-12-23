namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

internal static class Support
{
    // Table text formating functions
    public static string AggregateCells(IEnumerable<string> cells, string separator)
    {
        return string.Join(separator, cells);
    }
    public static string AggregateCells_FixedWidth(IEnumerable<string> cells, List<int> cellLengths, List<bool> cellLeftPad, string separator, char padding)
    {
        var cellTexts = cells.DefaultIfEmpty("").Select(
            (c, i) => cellLeftPad[i]
                ? c.PadLeft(cellLengths[i], padding)
                : c.PadRight(cellLengths[i], padding)
            );
        return string.Join(separator, cellTexts);
    }

    public static List<int> CalculateColumnWidths(IEnumerable<Line> cells)
    {
        // Calculate each column max size
        int columnCount = cells.First().Count();
        List<int> columnWidths = new();
        foreach (var i in Enumerable.Range(0, columnCount))
            columnWidths.Add(cells.Select(l => l[i].Count()).Max());
        return columnWidths;
    }
}