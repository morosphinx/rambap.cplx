using rambap.cplx.Core;
using rambap.cplx.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.PartInterfaces;

using FilenameAndInstruction = (string, IInstruction);

public class DocumentationBuilder
{
    internal List<FilenameAndInstruction> AdditionalInstructions { get; } = [];

    internal List<Func<Component, FilenameAndInstruction>> AdditionalInstructionBuilders { get; } = [];

    // it's wierd to pass the Pinstance here again ... will probably change
    internal IEnumerable<FilenameAndInstruction> MakeAllAdditionInstructions(Component component)
    {
        foreach(var i in AdditionalInstructions)
            yield return i;
        foreach(var f in AdditionalInstructionBuilders)
            yield return f(component);
    }

    Part ContextPart { get; }
    Pinstance ContextInstance { get; }

    // Internal constructor, prevent usage from outside assembly
    internal DocumentationBuilder(Pinstance instance, Part part)
    {
        ContextPart = part;
        ContextInstance = instance;
    }

    public void AddInstruction(string filename, IInstruction instruction)
        => AdditionalInstructions.Add((filename, instruction));
    public void AddInstruction(Func<Component, FilenameAndInstruction> instructionBuilder)
        => AdditionalInstructionBuilders.Add(instructionBuilder);
}