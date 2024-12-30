namespace rambap.cplx.Export.TextFiles;

public abstract class TextCustomFile : IInstruction
{
    public abstract string GetText();

    public void Do(string path)
    {
        File.WriteAllText(path, GetText());
    }
}
