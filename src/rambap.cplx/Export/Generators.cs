using rambap.cplx.Core;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing.Outputs;
using System.Linq;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;

namespace rambap.cplx.Export;

public static class FileGroups
{
    public static IEnumerable<(string, IInstruction)> CostingFiles(Component c, string filenamePattern)
    {
        return [
                ($"BOMR_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = CostTables.BillOfMaterial() with
                    {
                        WriteTotalLine = true,
                        TotalLineOnTop = true,
                    },
                    Formater = new MarkdownTableFormater()
                }),
                ($"RecurentCosts_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = CostTables.CostBreakdown() with
                    {
                        WriteTotalLine = true,
                        TotalLineOnTop = true,
                    },
                    Formater = new MarkdownTableFormater(),
                }),
                ($"BOTR_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = TaskTables.BillOfTasks() with
                    {
                        WriteTotalLine = true,
                        TotalLineOnTop = true,
                    },
                    Formater = new MarkdownTableFormater(),
                }),
                ($"RecurentTasks_{filenamePattern}.csv", new TxtTableFile(c) {
                    Table = TaskTables.TaskBreakdown() with
                    {
                        WriteTotalLine = true,
                        TotalLineOnTop = true,
                    },
                    Formater = new MarkdownTableFormater(),
                }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> SystemViewTables(Component c, string filenamePattern)
    {
        return [
                ($"Tree_Detailled_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Detailled(),
                    Formater = new FixedWidthTableFormater()
                }),
                ($"Tree_Stacked_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentTree_Stacked(),
                    Formater = new FixedWidthTableFormater()
                }),
                // TODO : Fix performance issue when generating this file on the 1000x parts exemple
                ($"Inventory_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Documentation.Outputs.SystemViewTables.ComponentInventory(),
                    Formater = new MarkdownTableFormater()
                }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> ConnectivityTables(Component c, string filenamePattern)
    {
        return [
                ($"Connections_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Connectivity.Outputs.ConnectivityTables.ConnectionTable(),
                    Formater = new MarkdownTableFormater()
                }),
                ($"Wirings_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Connectivity.Outputs.ConnectivityTables.WiringTable(),
                    Formater = new MarkdownTableFormater()
                }),
                ($"ICD_{filenamePattern}.csv", new TxtTableFile(c)
                {
                    Table = Modules.Connectivity.Outputs.ConnectivityTables.InterfaceControlDocumentTable(),
                    Formater = new MarkdownTableFormater()
                }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> DocumentationAdditionalInstructions(Component c)
    {
        var documentation = c.Instance.Documentation();
        if (documentation?.MakeAdditionalDocuments != null)
        {
            foreach(var d in documentation.MakeAdditionalDocuments(c))
            {
                yield return (d.Item1, d.Item2);
            }
        }
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
        Connectivity,
        Costing,
        SystemView,
        DocumentationAdditionalFiles,
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
        Func<Component, IEnumerable<(string, IInstruction)>> makeInstanceFiles =
            (c) =>
            [
                .. (contents.Contains(Content.Connectivity) ? FileGroups.ConnectivityTables(c, IGenerator.SimplefileNameFor(c)) : []),
                .. (contents.Contains(Content.Costing) ? FileGroups.CostingFiles(c, IGenerator.SimplefileNameFor(c)) : []),
                .. (contents.Contains(Content.SystemView) ? FileGroups.SystemViewTables(c, IGenerator.SimplefileNameFor(c)) : []),
                .. (contents.Contains(Content.DocumentationAdditionalFiles)
                    ? FileGroups.DocumentationAdditionalInstructions(c) : []),
            ];
        return ConfigureGenerator(makeInstanceFiles, hierarchyMode, subComponentInclusionCondition);
    }

    public static IGenerator ConfigureGenerator(
        Func<Component, IEnumerable<(string, IInstruction)>> makeInstanceFiles,
        HierarchyMode hierarchyMode,
        Func<Component, bool>? subComponentInclusionCondition = null)
    {
        return hierarchyMode switch
        {
            HierarchyMode.Flat => new FlattenedDocumentationTreeGenerator()
            {
                MakeComponentDocumentInstructions = makeInstanceFiles,
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
    public required Func<Component, IEnumerable<(string, IInstruction)>> MakeInstanceFiles { private get; init; }

    private Folder MakeRecursiveContentForComponent(Component c)
    {
        var iteratedSubcomponents = SubComponentInclusionCondition != null ?
            c.Instance.Components.Where(subc => SubComponentInclusionCondition(subc)) : [];
        var subcomponentFolders = iteratedSubcomponents.Select(subcomponent =>
            (FileNamePatternFor(subcomponent), MakeRecursiveContentForComponent(subcomponent)) );
        return new Folder([
                .. MakeInstanceFiles(c),
                .. subcomponentFolders
            ]);
    }

    public override IInstruction PrepareInstruction(Component rootComponent)
    {
        var rootFolder = FileNamePatternFor(rootComponent);
        return new Folder([
                (rootFolder, MakeRecursiveContentForComponent(rootComponent))
            ]);
    }
}

public class FlattenedDocumentationTreeGenerator : IGenerator
{
    public Func<Component, bool>? SubComponentInclusionCondition { private get; init; }
    public required Func<Component, IEnumerable<(string, IInstruction)>> MakeComponentDocumentInstructions { private get; init; }

    public override IInstruction PrepareInstruction(Component rootComponent)
    {
        var partTree= new PartTypesIterator<object>()
        {
            RecursionCondition = (c, l) => this.SubComponentInclusionCondition?.Invoke(c) ?? false
        };
        var content = partTree.MakeContent(rootComponent);
        var partFolders = content.Select(c => c.Component)
                                 .Select(c => 
                                 (FileNamePatternFor(c),new Folder(
                                     [
                                     .. MakeComponentDocumentInstructions(c)
                                     ]
                                 )));
        return new Folder([.. partFolders]);
    }
}