namespace rambap.cplx.Export;

using SubItem = (string RelativePath, IInstruction instruction) ;

/// <summary>
/// A Generator creating a folder, then executing sub generator
/// </summary>
public class Folder : IInstruction
{
    public List<SubItem> SubItems { get; init ; }
        = new List<SubItem>();

    public void Do(string path)
    {
        if( ! Directory.Exists(path))
            Directory.CreateDirectory(path);
        foreach(var i in SubItems)
        {
            var filename = Support.GetValidFilenameFrom(i.RelativePath);
            string subItemPath = Path.Combine(path, filename);
            i.instruction.Do(subItemPath);
        }
    }

    public Folder(IEnumerable<SubItem> subItems)
    {
        SubItems = subItems.ToList();
    }
}

