using System.Collections;
using rambap.cplx.Core;


namespace rambap.cplx.Export.Tables;

public class Line : IEnumerable<string>
{
    /// <summary>
    /// What kind of line this is. Used by <see cref="ITableFormater"/>s
    /// </summary>
    public enum LineType
    {
        /// <summary> This line contains columns names </summary>
        Header,

        /// <summary> This line contains a table content data </summary>
        Content,

        /// <summary> This line contains table total data </summary>
        Total,

        /// <summary> Blank line. Cells may still contains symbols  </summary>
        Spacer,

        /// <summary> Table break. An amount of table breaking spaces (ex : CRLF) should be written instead of cells.<br/>
        /// This imply nothing about the next line. The next line may or may not be an header</summary>
        TableBreak,
    }
    public required LineType Type { get; init; }

    public required List<string> Cells { get; init; }

    public string this[int index]
    {
        get => Cells[index];
        set => Cells[index] = value;
    }
    public IEnumerator<string> GetEnumerator() => Cells.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Define how to create table contents. Used in tandem with an <see cref="ITableFormater"/>
/// </summary>
public interface ITableProducer
{
    /// <summary>
    /// Definition of the columns of the table
    /// </summary>
    public IEnumerable<IColumn> IColumns { get; }

    public IEnumerable<Line> MakeAllLines(Pinstance rootComponent);

    public Line MakeHeaderLine();
    public IEnumerable<Line> MakeContentLines(Pinstance rootComponent);
    public Line MakeTotalLine(Pinstance rootComponent);
}

/// <summary>
/// Basic table produced defining how to organise content, header lines, TODO : and table breaks
/// </summary>
public abstract record TableProducer : ITableProducer
{
    public abstract IEnumerable<IColumn> IColumns { get; }

    public bool WriteHeaderLine { get; set; } = true;
    public bool WriteTotalLine { get; set; } = false;
    public bool TotalLineOnTop { get; set; } = false;

    public IEnumerable<Line> MakeAllLines(Pinstance rootComponent)
    {
        if (WriteHeaderLine)
            yield return MakeHeaderLine();
        if (WriteTotalLine && TotalLineOnTop)
            yield return MakeTotalLine(rootComponent);
        foreach (var contentLine in MakeContentLines(rootComponent))
            yield return contentLine;
        if (WriteTotalLine && ! TotalLineOnTop)
            yield return MakeTotalLine(rootComponent);
    }

    public abstract Line MakeHeaderLine();
    public abstract IEnumerable<Line> MakeContentLines(Pinstance rootComponent);
    public abstract Line MakeTotalLine(Pinstance rootComponent);
}
