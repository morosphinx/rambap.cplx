using rambap.cplx.Export;
using rambap.cplx.Export.Prodocs;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Connectivity.Outputs;
using static rambap.cplxtests.UsageTests.Support;

namespace rambap.cplxtests.UsageTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestGeneration_ReadmeExample()
    {
        var p = new ServerAssembly();
        var i = new Pinstance(p);
        var generator = GetDemoGenerator_AllCoreTables(true); ;
        generator.Do(i, Path.Combine("C:\\TestFolder\\ReadmeExemple"));
    }


    [TestMethod]
    public void TestGeneration_Exemple4()
    {
        var p = new Exemple4_Programatic();
        var i = new Pinstance(p);
        var generator = GetDemoGenerator_AllCoreTables();
        generator.Do(i, "C:\\TestFolder\\Exemple4");
    }

    [TestMethod]
    public void SkiaSandbox()
    {
        SkiaSvgTest.Test1();
    }

    //[TestMethod]
    //public void TestGeneration_Bench4()
    //{
    //    var p = new Bench4();
    //    var i = new Pinstance(p);
    //    var generator = GetDemoGeneratorWithEverything();
    //    generator.Do(i, "C:\\TestFolder\\Bench4");
    //}
    //class Bench4 : Bench3, IPartAdditionalDocuments
    //{
    //    public void Additional_Documentation(DocumentationBuilder Do)
    //    {
    //        Do.AddInstruction(i =>
    //        {
    //            var textFile = new TextTableFile(i)
    //            {
    //                Table = ConnectivityTables.ConnectionTable(),
    //                Formater = new Export.Tables.MarkdownTableFormater()
    //            };
    //            return ("TestFilename", textFile);
    //        });
    //    }
    //}

    //[TestMethod]
    //public void TestGeneration_DecimalPropTask()
    //{
    //    var p = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
    //    var i = new Pinstance(p);
    //    var generator = GetDemoGeneratorWithEverything();
    //    generator.Do(i, "C:\\TestFolder\\Exemple6");
    //}
}