using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.UnitTests.ExportValidity;

[TestClass]
public class TestComponentIterator
{
    public void TestComponentIteration_Output_Part(bool recursive, bool writeBranches)
    {
        var iterator = new ComponentIterator()
        {
            RecursionCondition = (c, l) => recursive,
            WriteBranches = writeBranches
        };

        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var pinstance = new Pinstance(part);
        var res = iterator.MakeContent(pinstance);

        var debugTable = new TextTableFile(pinstance)
        {
            Table = new TableProducer<IComponentContent>()
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
    public void TestComponentIteration_1() => TestComponentIteration_Output_Part(false, false);

    [TestMethod]
    public void TestComponentIteration_2() => TestComponentIteration_Output_Part(true, false);

    [TestMethod]
    public void TestComponentIteration_3() => TestComponentIteration_Output_Part(false, true);

    [TestMethod]
    public void TestComponentIteration_4() => TestComponentIteration_Output_Part(true, true);
}
