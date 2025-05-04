using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplxtests.CoreTests.ExportValidity;

internal static class TestColumn_Support
{
    public static void TestDecimalColumn_SumCoherence<T>(
        Component component,
        IIterator<ICplxContent> iterator,
        decimal expectedTotal,
        IColumn<ICplxContent> testedColumn,
        Func<IPropertyContent<T>, string> propertyNaming,
        IEnumerable<IColumn<ICplxContent>> debugDataColumns)
    {
        var res = iterator.MakeContent(component);
        var values = res.Select(testedColumn.CellFor);
        var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();

        // Write table in console for debug
        var debugTable = new TxtTableFile(component)
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
        Component component,
        decimal expectedTotal,
        IColumn<ICplxContent> testedColumn)
    {
        var columnTotal = Convert.ToDecimal(testedColumn.TotalFor(component.Instance));
        Assert.AreEqual(expectedTotal, columnTotal, $"Incoherent column autocalculated sum");
    }
}

