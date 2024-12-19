using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Modules.Costing.Outputs;

namespace rambap.cplx.Export;

public static class FileGroups
{
    public static IEnumerable<(string, IInstruction)> CostingFiles(Pinstance i, string filenamePattern)
    {
        return [
                ($"BOMR_{filenamePattern}.csv", new MarkdownTableFile(i)
                {
                    Table = CostTables.BillOfMaterial(),
                    WriteTotalLine = true,
                    TotalLineOnTop = true,
                }),
                ($"RecurentCosts_{filenamePattern}.csv", new MarkdownTableFile(i)
                {
                    Table = CostTables.CostBreakdown(),
                    WriteTotalLine = true,
                    TotalLineOnTop = true,
                }),
                ($"BOTR_{filenamePattern}.csv", new MarkdownTableFile(i)
                {
                    Table = TaskTables.BillOfTasks(),
                    WriteTotalLine = true,
                    TotalLineOnTop = true,
                }),
                ($"RecurentTasks_{filenamePattern}.csv", new MarkdownTableFile(i) {
                    Table = TaskTables.RecurentTaskBreakdown(),
                    WriteTotalLine = true,
                    TotalLineOnTop = true,
                }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> SystemViewTables(Pinstance i, string filenamePattern)
    {
        return [
                ($"Tree_Detailled_{filenamePattern}.csv", new FixedWidthTableFile(i)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Detailled()
                }),
                ($"Tree_Stacked_{filenamePattern}.csv", new FixedWidthTableFile(i)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Stacked()
                }),
                ($"Inventory_{filenamePattern}.csv", new MarkdownTableFile(i)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentInventory()
                }),
                ];
    }
}

public static class Generators
{
    /// <summary>
    /// Helper function to configure a generator for the cplx core contents
    /// </summary>
    /// <returns></returns>
     
    public enum Content
    {
        Costing,
        SystemView
    }

    public enum HierarchyMode
    {
        Flat,
        Hierarchical
    }

    public static IGenerator ConfigureGenerator(
        Content content,
        HierarchyMode hierarchyMode,
        Func<Component, bool>? subComponentInclusionCondition = null)
    {
        return ConfigureGenerator([content], hierarchyMode, subComponentInclusionCondition);
    }

    public static IGenerator ConfigureGenerator(
        IEnumerable<Content> contents,
        HierarchyMode hierarchyMode,
        Func<Component, bool>? subComponentInclusionCondition = null)
    {
        Func<Pinstance, IEnumerable<(string, IInstruction)>> makeInstanceFiles =
            (i) =>
            [
                .. (contents.Contains(Content.Costing) ? FileGroups.CostingFiles(i, IGenerator.SimplefileNameFor(i)) : []),
                .. (contents.Contains(Content.SystemView) ? FileGroups.SystemViewTables(i, IGenerator.SimplefileNameFor(i)) : []),
            ];
        return ConfigureGenerator(makeInstanceFiles, hierarchyMode, subComponentInclusionCondition);
    }

    public static IGenerator ConfigureGenerator(
        Func<Pinstance, IEnumerable<(string, IInstruction)>> makeInstanceFiles,
        HierarchyMode hierarchyMode,
        Func<Component, bool>? subComponentInclusionCondition = null)
    {
        return hierarchyMode switch
        {
            HierarchyMode.Flat => new FlattenedDocumentationTreeGenerator()
            {
                MakeInstanceFiles = makeInstanceFiles,
                SubComponentInclusionCondition = subComponentInclusionCondition
            },
            HierarchyMode.Hierarchical => new HierarchicalDocumentationTreeGenerator()
            {
                MakeInstanceFiles = makeInstanceFiles,
                SubComponentInclusionCondition = subComponentInclusionCondition
            },
            _ => throw new NotImplementedException()
        };
    }
}

public class HierarchicalDocumentationTreeGenerator : IGenerator
{
    public Func<Component, bool>? SubComponentInclusionCondition { private get; init; }
    public required Func<Pinstance, IEnumerable<(string, IInstruction)>> MakeInstanceFiles { private get; init; }

    public virtual IEnumerable<(string, IInstruction)> MakeFilesForInstance(Pinstance i)
        => FileGroups.CostingFiles(i, FileNamePatternFor(i));

    public virtual IEnumerable<(string, IInstruction)> MakeFolderForComponent(Component c)
        => [ (FileNamePatternFor(c.Instance), MakeRecursiveContentForInstance(c.Instance))];

    private Folder MakeRecursiveContentForInstance(Pinstance i)
    {

        var iteratedComponents = SubComponentInclusionCondition != null ?
            i.Components.Where(c => SubComponentInclusionCondition(c)) : [];
        return new Folder([
                .. MakeInstanceFiles(i),
                .. iteratedComponents.SelectMany(MakeFolderForComponent)
            ]);
    }

    public override IInstruction PrepareInstruction(Pinstance i)
    {
        var rootFolder = FileNamePatternFor(i);
        return new Folder([
                (rootFolder, MakeRecursiveContentForInstance(i))
            ]);
    }
}

public class FlattenedDocumentationTreeGenerator : IGenerator
{
    public Func<Component, bool>? SubComponentInclusionCondition { private get; init; }
    public required Func<Pinstance, IEnumerable<(string, IInstruction)>> MakeInstanceFiles { private get; init; }

    public override IInstruction PrepareInstruction(Pinstance i)
    {
        var partTree= new PartTypesIterator()
        {
            RecursionCondition = (c, l) => this.SubComponentInclusionCondition?.Invoke(c) ?? false
        };
        var content = partTree.MakeContent(i);
        var partFolders = content.Select(c => c.Component.Instance)
                                 .Select(p => 
                                 (FileNamePatternFor(i),new Folder(
                                     [
                                     .. MakeInstanceFiles(i)
                                     ]
                                 )));
        return new Folder([.. partFolders]);
    }
}