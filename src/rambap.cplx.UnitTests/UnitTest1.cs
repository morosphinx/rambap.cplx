using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Spreadsheet;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Export.Iterators;
using System.Reflection.Emit;
using static rambap.cplx.Export.Generators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public class DemoGenerator : IGenerator
        {
            public override IInstruction PrepareInstruction(Pinstance i)
            {
                return new Folder(
                [
                    ("BillOfMaterial_tree.csv", new MarkdownTableFile(i) { Table = Costing.BillOfMaterial_CompleteTree() }),
                    ("BillOfMaterial_flat.csv", new MarkdownTableFile(i) { Table = Costing.BillOfMaterial_Flat() }),
                    ("ComponentTree.csv", new FixedWidthTableFile(i) { Table = SystemView.ComponentTree()}),
                    ("CostBreakdown.csv", new MarkdownTableFile(i){ Table =  Costing.CostBreakdown(), WriteTotalLine = true }),
                    ("Tasks.csv", new MarkdownTableFile(i) { Table = Costing.BillOfTasks()}),
                    ("BillOfMaterial_tree.xlsx", new ExcelTableFile_FromTemplate(i) { Table = Costing.BillOfMaterial_CompleteTree() }),
                    ("BillOfMaterial_flat.xlsx", new ExcelTableFile_FromTemplate(i) { Table = Costing.BillOfMaterial_Flat() }),
                    ("ComponentTree.xlsx", new ExcelTableFile_FromTemplate(i) { Table = SystemView.ComponentTree()}),
                    ("CostBreakdown.xlsx", new ExcelTableFile_FromTemplate(i){ Table =  Costing.CostBreakdown()}),
                    ("Tasks.xlsx", new ExcelTableFile_FromTemplate(i) { Table = Costing.BillOfTasks()}),
                ]);
            }
        }

        public static IGenerator GetDemoGeneratorWithEverything(bool fileContentRecursion = false)
        {
            return Generators.ConfigureGenerator(
                [Content.Costing, Content.SystemView], HierarchyMode.Flat, c => fileContentRecursion);
        }

        [TestMethod]
        public void TestGeneration_ReadmeExample()
        {
            var p = new ServerAssembly();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything(true);
            generator.Do(i, "C:\\TestFolder\\ReadmeExemple");
        }

        [TestMethod]
        public void TestGeneration_ReadmeExample2()
        {
            var p = new ServerAssembly();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything(true); ;
            generator.Do(i, Path.Combine("C:\\TestFolder\\ReadmeExemple2"));
        }

        [TestMethod]
        public void TestGeneration_MaBaieIndustrielle()
        {
            var p = new MaBaieIndustrielle();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\MaBaieIndustrielle");
        }

        [TestMethod]
        public void TestGeneration_Breakout9()
        {
            var p = new Breakout9();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Breakout9");
        }

        [TestMethod]
        public void TestGeneration_Exemple3()
        {
            var p = new RackConnected1();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Exemple3");
        }

        [TestMethod]
        public void TestGeneration_Exemple4()
        {
            var p = new Exemple4_Programatic();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Exemple4");
        }
    }
}