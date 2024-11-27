using rambap.cplx.Core;

namespace rambap.cplx.Export;

/// <summary>
/// Define an output to disk
/// </summary>
public abstract class IGenerator
{
    public void Do(Pinstance instance, string path)
    {
        PrepareInstruction(instance).Do(path);
    }

    public abstract IInstruction PrepareInstruction(Pinstance instance);

    protected virtual string FileNamePatternFor(Pinstance i)
        => SimplefileNameFor(i);

    public static string SimplefileNameFor(Pinstance i)
    {
        string PN = i.PN;
        string revision = i.Revision;
        if (revision != "") return $"{PN}_{revision}";
        else return $"{PN}";
    }
}


/// <summary>
/// Define an action generating a file or folder on disk
/// </summary>
public interface IInstruction
{
    /// <summary>
    /// Execute the Instruction and create the content at path <br/>
    /// </summary>
    /// <param name="path">Target path. It must not be occupied.</param>
    void Do(string path);
}
