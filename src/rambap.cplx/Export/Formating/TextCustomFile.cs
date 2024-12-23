using rambap.cplx.Core;

namespace rambap.cplx.Export.Formating;

public abstract class TextCustomFile : IInstruction
{
    public abstract string GetText();

    public void Do(string path)
    {
        File.WriteAllText(path, GetText());
    }
}
