using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;

namespace rambap.cplx.Export.Prodocs;

public class MdSystemView : SinglePInstanceCustomFile
{
    private TextTableFile ComponentTree
        => new TextTableFile(Content)
        {
            Formater = new MarkdownTableFormater(),
            Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Stacked()
        };

    public override string GetText() =>
$"""
# SYSTEM VIEW : {Content.PN}

## Identification
{CommonSections.CommonHeader(Content)}

## Component Tree :
{ComponentTree.GetAllLines().JoinStrings()}

## Descriptions :
{CommonSections.MarkdownDocLines(Content).JoinStrings()}

""";
    
}
