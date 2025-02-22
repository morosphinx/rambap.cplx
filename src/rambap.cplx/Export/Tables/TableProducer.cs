using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

/// <summary>
/// Definition of a Table to be displayed in a file
/// Output 2D string array
/// </summary>
/// <typeparam name="T">The type of all line of the table. This may be abstract</typeparam>
public record TableProducer<T> : TableProducer
{
    /// <summary> Iterator that select that lines content </summary>
    public required IIterator<T> Iterator { get; init; }

    /// <summary> Definition of the columns of the table </summary>
    public required List<IColumn<T>> Columns { get; init; }
    public override IEnumerable<IColumn> IColumns => Columns;

    /// <summary>
    /// Enumerable transformation applied while running <see cref="MakeContentLines(Pinstance)"/> </br>
    /// Can be used to add filtering to the table data.
    /// </summary>
    public Func<IEnumerable<T>, IEnumerable<T>>? ContentTransform { get; init; } = null;

    public Func<T, T, bool>? AddSpacerCondition { get; init; } = null;
    public Func<T, T, bool>? AddTableBreakCondition { get; init; } = null;


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
        foreach (var c in camelCaseString)
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
            ColumnTypeHint.StringExact => false,
            ColumnTypeHint.StringFormatable => true,
            ColumnTypeHint.Numeric => false,
            _ => throw new NotImplementedException()
        };


    public override Line MakeHeaderLine()
        => new ()
        {
            Type = Line.LineType.Header,
            Cells = IColumns.Select(col => col.Title).ToList(),
        };
        
    private Line MakeContentLine(T item)
    {
        if (RemoveCamelCase)
        {
            return new()
            {
                Type = Line.LineType.Content,
                Cells = Columns.Select(col =>
                {
                    var text = col.CellFor(item);
                    if (CamelCasePossible(col.TypeHint))
                        return CamelCaseToNormalCase(text);
                    else
                        return text;
                }).ToList()
            };
        }
        else
        {
            return new()
            {
                Type = Line.LineType.Content,
                Cells = Columns.Select(col => col.CellFor(item)).ToList(),
            };
        }
    }


    private Line MakeBreakLine()
    {
        return new()
        {
            Type = Line.LineType.TableBreak,
            Cells = Columns.Select(c => "").ToList()
        };
    }
    private Line MakeSpacerLine()
    {
        return new()
        {
            Type = Line.LineType.Spacer,
            Cells = Columns.Select(c => "").ToList()
        };
    }
    public override IEnumerable<Line> MakeContentLines(Pinstance rootComponent)
    {
        var contents = Iterator.MakeContent(rootComponent);
        // Apply content transform
        if(ContentTransform is not null)
            contents = ContentTransform(contents);
        // Add additional breaks f required
        T? previousContent = default;
        foreach (var c in contents)
        {
            if (previousContent is null)
            {
                // First iteration
                yield return MakeContentLine(c);
            } else
            {
                // Add spacers if required
                bool doSpace = AddSpacerCondition?.Invoke(previousContent,c) ?? false;
                bool doBreak = AddTableBreakCondition?.Invoke(previousContent, c) ?? false;
                if (doBreak)
                    yield return MakeBreakLine();
                else if (doSpace)
                    yield return MakeSpacerLine();
                // Content Line
                yield return MakeContentLine(c);
            }
            previousContent = c;
        }
    }

    public override Line MakeTotalLine(Pinstance rootComponent)
        => new()
        {
            Type = Line.LineType.Total,
            Cells = IColumns.Select(col => col.TotalFor(rootComponent)).ToList(),
        };
}

