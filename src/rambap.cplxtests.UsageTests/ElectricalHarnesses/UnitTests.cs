using static rambap.cplxtests.UsageTests.Support;

namespace rambap.cplxtests.UsageTests.ElectricalHarnesses;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void TestGeneration_BoxAssembly1_Prodocs()
    {
        var p = new BoxAssembly1();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(c, "C:\\TestFolder\\ElectricalHarnesses");
    }
    [TestMethod]
    public void TestGeneration_InternalHarness1_Prodocs()
    {
        var p = new InternalHarness1();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(c, "C:\\TestFolder\\ElectricalHarnesses");
    }
    [TestMethod]
    public void TestGeneration_BoxAssembly2_Prodocs()
    {
        var p = new BoxAssembly2();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(c, "C:\\TestFolder\\ElectricalHarnesses");
    }
    [TestMethod]
    public void TestGeneration_InternalHarness2_Prodocs()
    {
        var p = new InternalHarness2();
        var c = p.Instantiate();
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(c, "C:\\TestFolder\\ElectricalHarnesses");
    }
}
