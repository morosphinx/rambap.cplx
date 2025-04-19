using rambap.cplx.Export.Tables;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.CoreTests.ExportValidity;

[TestClass]
public class TestPartTypesIterator
{
    private void TestPartTypeIterator_Output(bool recursive, bool writeBranches)
    {
        var iterator = new PartTypesIterator<object>()
        {
            RecursionCondition = (c, l) => recursive,
            WriteBranches = writeBranches,
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
    public void Test_PartTypeIteration_1() => TestPartTypeIterator_Output(false, false);
    [TestMethod]
    public void Test_PartTypeIteration_2() => TestPartTypeIterator_Output(false, true);
    [TestMethod]
    public void Test_PartTypeIteration_3() => TestPartTypeIterator_Output(true, false);
    [TestMethod]
    public void Test_PartTypeIteration_4() => TestPartTypeIterator_Output(true, true);
}
