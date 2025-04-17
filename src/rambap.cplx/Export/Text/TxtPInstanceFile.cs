using rambap.cplx.Core;

namespace rambap.cplx.Export.Text;

public abstract class TxtPInstanceFile : IInstruction
{
    public required Component Content { get; init; }

    public abstract string GetText();

    public void Do(string path)
    {
        File.WriteAllText(path, GetText());
    }
}
