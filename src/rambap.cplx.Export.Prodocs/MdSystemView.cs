﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;

namespace rambap.cplx.Export.Prodocs;

public class MdSystemView : IInstruction
{
    private Pinstance DocumentedPart { get; init; }

    private TextTableFile ComponentTree
        => new TextTableFile(DocumentedPart)
        {
            Formater = new MarkdownTableFormater(),
            Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Stacked()
        };

    public MdSystemView(Pinstance Target)
    {
        DocumentedPart = Target;
    }

    public void Do(string path)
    {
        using (var file = File.CreateText(path))
        {
            file.Write(FileContents);
        }
    }

    private string FileContents =>
$"""
# SYSTEM VIEW : {DocumentedPart.PN}

## Identification
{CommonSections.CommonHeader(DocumentedPart)}

## Component Tree :
{ComponentTree.GetAllLines().GetText()}

""";
    
}
