using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

public abstract class SinglePInstanceCustomFile : IInstruction
{
    public required Pinstance Content { get; init; }

    public abstract string GetText();

    public void Do(string path)
    {
        File.WriteAllText(path, GetText());
    }
}
