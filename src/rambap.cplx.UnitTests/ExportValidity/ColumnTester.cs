using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.UnitTests.ExportValidity;

internal static class ColumnTester
{
    public static IEnumerable<(string name, ComponentContentTree iterator)> AllComponentTrees(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return ("Component Tree, Flat, No Branches", new ComponentContentTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });
        yield return ("Component Tree, Recursive, No Branches", new ComponentContentTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });
        yield return ("Component Tree, Flat, With Branches", new ComponentContentTree()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
        yield return ("Component Tree, Recursive, With Branches", new ComponentContentTree()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
    }

    public static IEnumerable<(string name, PartContentList iterator)> AllPartTrees(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return ("Part list, Flat, No Branches", new PartContentList()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });

        yield return ("Part list, Recursive, No Branches", new PartContentList()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });

        yield return ("Part list, Flat, With Branches", new PartContentList()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });

        yield return ("Part list, Recursive, With Branches", new PartContentList()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
    }

    public static void TestComponentTreeColumn_Decimal(
        Pinstance pinstance,
        IColumn<ComponentContent> column,
        Func<Pinstance, IEnumerable<object>> propertyIterator,
        decimal expectedTotal)
    {
        foreach (var t in AllComponentTrees(propertyIterator))
        {
            var res = t.iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total, $"Incoherent column sum for {t.name}");
        }
    }

    public static void TestPartTreeColumn_Decimal(
       Pinstance pinstance,
       IColumn<PartContent> column,
       Func<Pinstance, IEnumerable<object>> propertyIterator,
       decimal expectedTotal)
    {
        foreach (var t in AllPartTrees(propertyIterator))
        {
            var res = t.iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total, $"Incoherent column sum for {t.name}");
        }
    }
}

