using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export;

using Line = List<string>;

public interface ITable
{
    public IEnumerable<IColumn> IColumns { get; }

    Line MakeHeaderLine();
    IEnumerable<Line> MakeContentLines(Pinstance rootComponent);
    Line MakeTotalLine(Pinstance rootComponent);
}

/// <summary>
/// Definition of a Table to be displayed in a file
/// Output 2D string array
/// </summary>
/// <typeparam name="T">The type of all line of the table. This may be abstract</typeparam>
public record Table<T> : ITable
{
    /// <summary> Iterator that select that lines content </summary>
    public required IIterator<T> Iterator { get; init; }

    /// <summary> Definition of the columns of the table </summary>
    public required List<IColumn<T>> Columns { get; init; }

    public IEnumerable<IColumn> IColumns => Columns;

    public Line MakeHeaderLine()
        => IColumns.Select(col => col.Title).ToList();
    private Line MakeContentLine(T item)
        => Columns.Select(col => col.CellFor(item)).ToList();

    public IEnumerable<Line> MakeContentLines(Pinstance rootComponent)
    {
        foreach (var c in Iterator.MakeContent(rootComponent))
            yield return MakeContentLine(c);
    }

    public Line MakeTotalLine(Pinstance rootComponent)
        => IColumns.Select(col => col.TotalFor(rootComponent)).ToList();
}

