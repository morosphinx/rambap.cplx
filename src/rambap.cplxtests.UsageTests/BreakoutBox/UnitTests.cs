using static rambap.cplxtests.UsageTests.Support;

namespace rambap.cplxtests.UsageTests.BreakoutBox;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void TestGeneration_BreakoutBox1_txt()
    {
        var p = new BreakoutBox1();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllCoreTables();
        generator.Do(c, "C:\\TestFolder\\Breakout9_txt");
    }

    [TestMethod]
    public void TestGeneration_BreakoutBox1_Excel()
    {
        var p = new BreakoutBox1();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllCoreTables_Excel();
        generator.Do(c, "C:\\TestFolder\\Breakout9_Excel");
    }

    [TestMethod]
    public void TestGeneration_BreakoutBox1_Prodocs()
    {
        var p = new BreakoutBox1();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(c, "C:\\TestFolder\\Breakout9_Prodocs");
    }

}
