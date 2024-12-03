using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public abstract class AbstractTableFile : IInstruction
{
    /// <summary> Definition of the table written to the file </summary>
    public required ITable Table { protected get; init; }

    /// <summary>
    /// Instance whose properties and component are written to the file
    /// CID of the file are relative to this component
    /// </summary>
    public Pinstance Content { get; init; }

    public AbstractTableFile(Pinstance content)
    {
        Content = content;
    }


    // File Writting functions
    protected static string AggregateCells(IEnumerable<string> cells, string separator)
    {
        return string.Join(separator, cells);
    }
    protected static string AggregateCells_FixedWidth(IEnumerable<string> cells, List<int> cellLengths, List<bool> cellLeftPad, string separator, char padding)
    {
        var cellTexts = cells.DefaultIfEmpty("").Select(
            (c, i) => cellLeftPad[i]
                ? c.PadLeft(cellLengths[i], padding) 
                : c.PadRight(cellLengths[i], padding)
            );
        return string.Join(separator, cellTexts);
    }

    protected static List<int> CalculateColumnWidths(IEnumerable<Line> cells)
    {
        // Calculate each column max size
        int columnCount = cells.First().Count();
        List<int> columnWidths = new();
        foreach (var i in Enumerable.Range(0, columnCount))
            columnWidths.Add(cells.Select(l => l[i].Count()).Max());
        return columnWidths;
    }

    public abstract void Do(string path);
}


