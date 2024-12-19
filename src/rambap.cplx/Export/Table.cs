using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export;

using static rambap.cplx.Export.Generators;
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

    /// <summary>
    /// Enumerable transformation applied while running <see cref="MakeContentLines(Pinstance)"/> </br>
    /// Can be used to add filterign to the table data.
    /// </summary>
    public Func<IEnumerable<T>, IEnumerable<T>>? ContentTransform { get; init; } = null;

    /// <summary>
    /// If true, all text are converted from CamelCase to normal case. Exemple : <br/>
    /// "PartName" => "Part Name"
    /// </summary>
    public bool RemoveCamelCase { get; init; } = true;
    private static string CamelCaseToNormalCase(string camelCaseString)
    {
        string result = string.Empty;
        bool previousLower = false;
        // <!> Upper and lower case are not complementary, for exemple, '-' is neither upper or lower
        foreach(var c in camelCaseString)
        {
            var currentUpper = char.IsUpper(c);
            if (previousLower && currentUpper)
                result += " " + c;
            else 
                result += c;
            previousLower = char.IsLower(c);
        }
        return result;
    }
    private static bool CamelCasePossible(ColumnTypeHint typeHint)
        => typeHint switch
        {
            ColumnTypeHint.String => true,
            ColumnTypeHint.Numeric => false,
            _ => throw new NotImplementedException()
        };

    public IEnumerable<IColumn> IColumns => Columns;

    public Line MakeHeaderLine()
        => IColumns.Select(col => col.Title).ToList();
    private Line MakeContentLine(T item) {
        if (RemoveCamelCase)
        {
            return Columns.Select(col =>
            {
                var text = col.CellFor(item);
                if (CamelCasePossible(col.TypeHint))
                    return CamelCaseToNormalCase(text);
                else
                    return text;
            }).ToList();
        }
        else
            return Columns.Select(col => col.CellFor(item)).ToList();
    }

    public IEnumerable<Line> MakeContentLines(Pinstance rootComponent)
    {
        if(ContentTransform is null)
        {
            foreach (var c in Iterator.MakeContent(rootComponent))
                yield return MakeContentLine(c);
        } else
        {
            var content = Iterator.MakeContent(rootComponent);
            content = ContentTransform(content);
            foreach (var c in content)
                yield return MakeContentLine(c);
        }
    }

    public Line MakeTotalLine(Pinstance rootComponent)
        => IColumns.Select(col => col.TotalFor(rootComponent)).ToList();
}

