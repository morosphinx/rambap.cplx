using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Spreadsheet;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Export.Iterators;
using System.Reflection.Emit;
using static rambap.cplx.Export.Generators;
using rambap.cplx.Export.Tables;
using rambap.cplx.UnitTests.ExportValidity;

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
                    ("BillOfMaterial_tree.xlsx", new ExcelTableFile_FromTemplate(i) { Table = Costing.BillOfMaterial() }),
                    ("BillOfMaterial_flat.xlsx", new ExcelTableFile_FromTemplate(i) { Table = Costing.BillOfMaterial() }),
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
        public static IGenerator GetDemoGeneratorWithEverything_excel(bool fileContentRecursion = false)
        {
            return Generators.ConfigureGenerator(
                i => [.. ExcelGenerators.CostingFiles(i, IGenerator.SimplefileNameFor(i)),
                      .. ExcelGenerators.SystemViewTables(i, IGenerator.SimplefileNameFor(i)) ]
                , HierarchyMode.Flat, c => fileContentRecursion);
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
        public void TestGeneration_BreakoutBox1_txt()
        {
            var p = new BreakoutBox1();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Breakout9_txt");
        }

        [TestMethod]
        public void TestGeneration_BreakoutBox1_Excel()
        {
            var p = new BreakoutBox1();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything_excel();
            generator.Do(i, "C:\\TestFolder\\Breakout9_Excel");
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

        [TestMethod]
        public void TestGeneration_DecimalPropTask()
        {
            var p = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Exemple6");
        }
    }
}