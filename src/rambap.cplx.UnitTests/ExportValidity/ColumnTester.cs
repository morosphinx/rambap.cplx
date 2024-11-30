using rambap.cplx.Export;
using rambap.cplx.Export.Columns;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.UnitTests.ExportValidity;

internal static class ColumnTester
{
    public static IEnumerable<ComponentTree> AllComponentTrees(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return new ComponentTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        };
        yield return new ComponentTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        };
        yield return new ComponentTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        };
        yield return new ComponentTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        };
    }

    public static IEnumerable<PartTree> AllPartTrees(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return new PartTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        };

        yield return new PartTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        };

        yield return new PartTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        };

        yield return new PartTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        };
    }

    public static void TestComponentTreeColumn_Decimal(
        Pinstance pinstance,
        IColumn<ComponentTreeItem> column,
        Func<Pinstance, IEnumerable<object>> propertyIterator,
        decimal expectedTotal)
    {
        foreach (var iterator in AllComponentTrees(propertyIterator))
        {
            var res = iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total);
        }
    }

    public static void TestPartTreeColumn_Decimal(
       Pinstance pinstance,
       IColumn<PartTreeItem> column,
       Func<Pinstance, IEnumerable<object>> propertyIterator,
       decimal expectedTotal)
    {
        foreach (var iterator in AllPartTrees(propertyIterator))
        {
            var res = iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total);
        }
    }
}

