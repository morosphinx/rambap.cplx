using rambap.cplx.Core;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export.Text;

public class TxtTableFile : IInstruction
{
    /// <summary> Definition of the table written to the file </summary>
    public required ITableProducer Table { protected get; init; }

    /// <summary> Definition of the table written to the file </summary>
    public required ITableFormater Formater { protected get; init; }

    /// <summary>
    /// Instance whose properties and component are written to the file
    /// CID of the file are relative to this component
    /// </summary>
    public Component Content { get; init; }

    public IEnumerable<string> GetAllLines()
        => Formater.Format(Table, Content);

    public TxtTableFile(Component content)
    {
        Content = content;
    }

    public void Do(string path)
    {
        var lines = GetAllLines();
        File.WriteAllLines(path, lines);
    }

    public void WriteToConsole()
    {
        var lines = Formater.Format(Table, Content);
        foreach (var l in lines)
            Console.WriteLine(l);
    }
}


