using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public class TextTableFile : IInstruction
{
    /// <summary> Definition of the table written to the file </summary>
    public required ITable Table { protected get; init; }

    /// <summary> Definition of the table written to the file </summary>
    public required ITableFormater Formater { protected get; init; }

    /// <summary>
    /// Instance whose properties and component are written to the file
    /// CID of the file are relative to this component
    /// </summary>
    public Pinstance Content { get; init; }

    public TextTableFile(Pinstance content)
    {
        Content = content;
    }

    public void Do(string path)
    {
        var lines = Formater.Format(Table, Content);
        File.WriteAllLines(path, lines);
    }
}


