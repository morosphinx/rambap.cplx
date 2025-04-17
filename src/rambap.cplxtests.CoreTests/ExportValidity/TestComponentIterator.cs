using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplxtests.CoreTests.ExportValidity;

[TestClass]
public class TestComponentIterator
{
    private void TestComponentIteration_Output_Part(bool recursive, bool writeBranches, bool groupAtSameLocation = false)
    {
        var iterator = new ComponentIterator()
        {
            RecursionCondition = (c, l) => recursive,
            WriteBranches = writeBranches,
            GroupPNsAtSameLocation = groupAtSameLocation,
        };

        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var component = part.Instantiate();
        var res = iterator.MakeContent(component);

        var debugTable = new TxtTableFile(component)
        {
            Table = new TableProducer<ICplxContent>()
            {
                Columns =
                [
                    CommonColumns.LineNumber(),
                    IDColumns.ContentLocation(),
                    IDColumns.ComponentNumberPrettyTree(),
                    IDColumns.PartNumber(),
                    IDColumns.GroupCNs(),
                    CommonColumns.ComponentTotalCount(),
                ],
                Iterator = iterator,
            },
            Formater = new FixedWidthTableFormater(),
        };
        debugTable.WriteToConsole();
    }

    [TestMethod]
    public void Test_ComponentIteration_1() => TestComponentIteration_Output_Part(false, false);
    [TestMethod]
    public void Test_ComponentIteration_2() => TestComponentIteration_Output_Part(false, true);
    [TestMethod]
    public void Test_ComponentIteration_3() => TestComponentIteration_Output_Part(true, false);
    [TestMethod]
    public void Test_ComponentIteration_4() => TestComponentIteration_Output_Part(true, true);
    [TestMethod]
    public void Test_ComponentIterationGrouped_1() => TestComponentIteration_Output_Part(false, false, true);
    [TestMethod]
    public void Test_ComponentIterationGrouped_2() => TestComponentIteration_Output_Part(false, true, true);
    [TestMethod]
    public void Test_ComponentIterationGrouped_3() => TestComponentIteration_Output_Part(true, false, true);
    [TestMethod]
    public void Test_ComponentIterationGrouped_4() => TestComponentIteration_Output_Part(true, true, true);
}
