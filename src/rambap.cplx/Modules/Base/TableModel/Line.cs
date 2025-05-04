using System.Collections;


namespace rambap.cplx.Modules.Base.TableModel;

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
