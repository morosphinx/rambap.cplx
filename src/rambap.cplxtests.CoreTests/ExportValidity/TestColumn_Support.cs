using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplxtests.CoreTests.ExportValidity;

internal static class TestColumn_Support
{
    public static void TestDecimalColumn_SumCoherence<T>(
        Pinstance pinstance,
        IIterator<ICplxContent> iterator,
        decimal expectedTotal,
        IColumn<ICplxContent> testedColumn,
        Func<IPropertyContent<T>, string> propertyNaming,
        IEnumerable<IColumn<ICplxContent>> debugDataColumns)
    {
        var res = iterator.MakeContent(pinstance);
        var values = res.Select(testedColumn.CellFor);
        var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();

        // Write table in console for debug
        var debugTable = new TextTableFile(pinstance)
        {
            Table = new TableProducer<ICplxContent>()
            {
                Columns =
                [
                    CommonColumns.LineNumber(),
                    IDColumns.ContentLocation(),
                    IDColumns.ComponentNumberPrettyTree<T>(propertyNaming),
                    IDColumns.PartNumber(),
                    IDColumns.GroupCNs(),
                    CommonColumns.ComponentTotalCount(),
                    .. debugDataColumns,
                    testedColumn,
                ],
                Iterator = iterator,
            },
            Formater = new FixedWidthTableFormater(),
        };
        debugTable.WriteToConsole();
        
        Assert.AreEqual(expectedTotal, total, $"Incoherent column sum");
    }

    public static void TestDecimalColumn_SelfTotal(
        Pinstance pinstance,
        decimal expectedTotal,
        IColumn<ICplxContent> testedColumn)
    {
        var columnTotal = Convert.ToDecimal(testedColumn.TotalFor(pinstance));
        Assert.AreEqual(expectedTotal, columnTotal, $"Incoherent column autocalculated sum");
    }
}

