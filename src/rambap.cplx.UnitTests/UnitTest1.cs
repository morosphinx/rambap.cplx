using rambap.cplx.Export;
using static rambap.cplx.Export.Generators;
using rambap.cplx.UnitTests.ExportValidity;
using rambap.cplx.Modules.Costing.Outputs;
using rambap.cplx.UnitTests.Connectivity;

namespace rambap.cplx.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public static IGenerator GetDemoGeneratorWithEverything(bool fileContentRecursion = false)
        {
            return Generators.ConfigureGenerator(
                [Content.Connectivity, Content.Costing, Content.SystemView, Content.DocumentationAdditionalFiles]
                , HierarchyMode.Flat, c => fileContentRecursion);
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
            var generator = GetDemoGeneratorWithEverything(true); ;
            generator.Do(i, Path.Combine("C:\\TestFolder\\ReadmeExemple"));
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

            // IInstruction WiringDescription = new WiringDescriptionGenerator<RackConnected1>(
            //     p, r => r.J11, r.J12);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Exemple3");
        }
        [TestMethod]
        public void TestGeneration_Exemple3_i()
        {
            var p = new InternalCable1();
            var i = new Pinstance(p);

            // IInstruction WiringDescription = new WiringDescriptionGenerator<RackConnected1>(
            //     p, r => r.J11, r.J12);
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

        [TestMethod]
        public void TestGeneration_Bench4()
        {
            var p = new Bench4();
            var i = new Pinstance(p);
            var generator = GetDemoGeneratorWithEverything();
            generator.Do(i, "C:\\TestFolder\\Bench4");
        }
    }
}