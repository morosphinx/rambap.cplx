using static rambap.cplxtests.UsageTests.Support;

namespace rambap.cplxtests.UsageTests.ElectricalHarnesses;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void TestGeneration_BoxAssembly1_Prodocs()
    {
        var p = new BoxAssembly1();
        var i = new Pinstance(p);

        // IInstruction WiringDescription = new WiringDescriptionGenerator<RackConnected1>(
        //     p, r => r.J11, r.J12);
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(i, "C:\\TestFolder\\ElectricalHarnesses");
    }
    [TestMethod]
    public void TestGeneration_InternalHarness1_Prodocs()
    {
        var p = new InternalHarness1();
        var i = new Pinstance(p);

        // IInstruction WiringDescription = new WiringDescriptionGenerator<RackConnected1>(
        //     p, r => r.J11, r.J12);
        var generator = GetDemoGenerator_AllProdocs();
        generator.Do(i, "C:\\TestFolder\\ElectricalHarnesses");
    }

}
