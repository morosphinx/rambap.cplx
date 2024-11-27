using rambap.cplx.Core;

namespace rambap.cplx.Export.Text;

public class AssemblyOverview : IInstruction
{
    public Pinstance Content { get; init; }

    public AssemblyOverview(Pinstance content)
    {
        Content = content;
    }

    public void Do(string path)
    {
        List<string> lines = new();
        var m = Content.MechanicalAssembly();
        // TODO
        if (m != null)
        {
            foreach(var r in m.InstructedReceptacles)
            {
                lines.Add($"{r.Name}");
                lines.Add($"{r.Owner?.PN} {r.Type} : {r.SlotAmount}");
                foreach (var i in r.SlottedParts)
                {
                    lines.Add($"{i.Position} : {i.part.PN}");
                }
                lines.Add($"");
            }
        }
        File.WriteAllLines(path, lines);
    }
}

