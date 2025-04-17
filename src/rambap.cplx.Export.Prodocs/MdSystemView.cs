﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.Text;

namespace rambap.cplx.Export.Prodocs;

public class MdSystemView : TxtPInstanceFile
{
    private TxtTableFile ComponentTree
        => new TxtTableFile(Content)
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
