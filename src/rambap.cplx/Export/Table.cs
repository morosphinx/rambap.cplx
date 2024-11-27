using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export;

using Line = List<string>;

public interface ITable
{
    Line MakeHeaderLine();
    IEnumerable<Line> MakeContentLines(Pinstance rootComponent);
    Line MakeTotalLine(Pinstance rootComponent);

    /// TBD : Can't expose type Column<T> list int the ITable interface. Workarounds :
    int ColumunCount { get; }
    ColumnTypeHint ColumnTypeHint(int columnIndex);
    IEnumerable<ColumnTypeHint> ColumnTypeHints();
}

/// <summary>
/// Definition of a Table to be displayed in a file
/// Output 2D string array
/// </summary>
/// <typeparam name="T">The type of all line of the table. This may be abstract</typeparam>
public partial class Table<T> : ITable
{
    /// <summary> Iterator that select that lines content </summary>
    public required IIterator<T> Tree { get; init; }

    /// <summary> Definition of the columns of the table </summary>
    public required List<IColumn<T>> Columns { get; init; }
    public int ColumunCount => Columns.Count();
    public ColumnTypeHint ColumnTypeHint(int columnIndex) => Columns[columnIndex].TypeHint;
    public IEnumerable<ColumnTypeHint> ColumnTypeHints() => Columns.Select(x => x.TypeHint);

    public Line MakeHeaderLine()
        => Columns.Select(col => col.Title).ToList();
    private Line MakeContentLine(T item)
        => Columns.Select(col => col.CellFor(item)).ToList();

    public IEnumerable<Line> MakeContentLines(Pinstance rootComponent)
    {
        foreach (var c in Tree.MakeContent(rootComponent))
            yield return MakeContentLine(c);
    }

    public Line MakeTotalLine(Pinstance rootComponent)
        => Columns.Select(col => col.TotalFor(rootComponent)).ToList();
}

