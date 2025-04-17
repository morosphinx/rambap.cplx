using rambap.cplx.Core;

namespace rambap.cplx.Export;

/// <summary>
/// Define an output to disk
/// </summary>
public abstract class IGenerator
{
    public void Do(Component component, string path)
    {
        PrepareInstruction(component).Do(path);
    }

    public abstract IInstruction PrepareInstruction(Component component);

    protected virtual string FileNamePatternFor(Component c)
        => SimplefileNameFor(c);

    public static string SimplefileNameFor(Component c)
    {
        var i = c.Instance;
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
