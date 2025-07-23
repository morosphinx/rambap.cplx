using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplxtests.CoreTests.ExportValidity;

internal static class TestColumn_Support
{
    public static void TestDecimalColumn_SumCoherence<T>(
        Component component,
        IContentIterator<IContent> iterator,
        bool writeBranches,
        decimal expectedTotal,
        IColumn<IContent> testedColumn,
        Func<IPropertyContent<T>, string> propertyNaming,
        IEnumerable<IColumn<IContent>> debugDataColumns)
    {
        var res = iterator.MakeContent_AsFlat(component);
        var values = res.Select(testedColumn.CellFor);
        var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();

        // Write table in console for debug
        var debugTable = new TxtTableFile(component)
        {
            Table = new TableProducer<IContent>()
            {
                Columns =
                [
                    CommonColumns.LineNumber(),
                    IDColumns.ContentLocation(),
                    IDColumns.ComponentNumberPrettyTree(propertyNaming),
                    IDColumns.PartNumber(),
                    IDColumns.GroupCNs(),
                    CommonColumns.ComponentTotalCount(),
                    .. debugDataColumns,
                    testedColumn,
                ],
                Iterator = iterator,
                ContentTransform = cs
                    => cs.Where(c => (c.IsBranch && writeBranches) || c.IsLeaf) // Remove branch items if ! writeBranches
            },
            Formater = new FixedWidthTableFormater(),
        };
        debugTable.WriteToConsole();
        
        Assert.AreEqual(expectedTotal, total, $"Incoherent column sum");
    }

    public static void TestDecimalColumn_SelfTotal(
        Component component,
        decimal expectedTotal,
        IColumn<IContent> testedColumn)
    {
        var columnTotal = Convert.ToDecimal(testedColumn.TotalFor(component.Instance));
        Assert.AreEqual(expectedTotal, columnTotal, $"Incoherent column autocalculated sum");
    }
}

