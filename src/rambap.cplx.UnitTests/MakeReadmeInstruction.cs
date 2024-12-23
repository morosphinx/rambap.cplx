using rambap.cplx.Export.Formating;
using rambap.cplx.Export.TextFiles;

namespace rambap.cplx.UnitTests;

class MakeReadmeInstruction : TextCustomFile
{
    public required Pinstance Content { get; init; }

    public override string GetText()
    {
        return
$"""
Test of a {nameof(TextCustomFile)}

{
    string.Join("\r\n",
    new MarkdownTableFormater().Format(
        Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Detailled(),
        Content))
}

And this is the rest of the document
""";

    }
}
