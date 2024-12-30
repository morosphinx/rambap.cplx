using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.UnitTests.ExportValidity;

internal static class ColumnTester
{
    public static IEnumerable<(string name, IIterator<ComponentContent> iterator)> AllComponentIterators(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return ("ComponentIterator, Flat, No Branches", new ComponentIterator()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });
        yield return ("ComponentIterator, Recursive, No Branches", new ComponentIterator()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });
        yield return ("ComponentIterator, Flat, With Branches", new ComponentIterator()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
        yield return ("ComponentIterator, Recursive, With Branches", new ComponentIterator()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
    }

    public static IEnumerable<(string name, IIterator<ComponentContent> iterator)> AllPartTypeIterators(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
        yield return ("Part list, Flat, No Branches", new PartTypesIterator()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });

        yield return ("PartTypeIterator, Recursive, No Branches", new PartTypesIterator()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = false,
            PropertyIterator = propertyIterator
        });

        yield return ("PartTypeIterator, Flat, With Branches", new PartTypesIterator()
        {
            RecursionCondition = (c, l) => false,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });

        yield return ("PartTypeIterator, Recursive, With Branches", new PartTypesIterator()
        {
            RecursionCondition = (c, l) => true,
            WriteBranches = true,
            PropertyIterator = propertyIterator
        });
    }

    public static IEnumerable<(string name, IIterator<ComponentContent> iterator)> AllPartLocationIterators(Func<Pinstance, IEnumerable<object>> propertyIterator)
    {
            yield return ("PartLocationIterator, Flat", new PartLocationIterator()
        {
            RecursionCondition = (c, l) => false,
            PropertyIterator = propertyIterator
        });

        yield return ("PartLocationIterator, Recursive", new PartLocationIterator()
        {
            RecursionCondition = (c, l) => true,
            PropertyIterator = propertyIterator
        });
    }

    public static void TestComponentIteratorColumn_Decimal(
        Pinstance pinstance,
        IColumn<ComponentContent> column,
        Func<Pinstance, IEnumerable<object>> propertyIterator,
        decimal expectedTotal)
    {
        foreach (var t in AllComponentIterators(propertyIterator))
        {
            var res = t.iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total, $"Incoherent column sum for {t.name}");
        }
    }

    public static void TestPartTypeIteratorColumn_Decimal(
       Pinstance pinstance,
       IColumn<ComponentContent> column,
       Func<Pinstance, IEnumerable<object>> propertyIterator,
       decimal expectedTotal)
    {
        foreach (var t in AllPartTypeIterators(propertyIterator))
        {
            var res = t.iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total, $"Incoherent column sum for {t.name}");
        }
    }

    public static void TestPartTypeLocationColumn_Decimal(
        Pinstance pinstance,
        IColumn<ComponentContent> column,
        Func<Pinstance, IEnumerable<object>> propertyIterator,
        decimal expectedTotal)
    {
        foreach (var t in AllPartLocationIterators(propertyIterator))
        {
            var res = t.iterator.MakeContent(pinstance);
            var values = res.Select(column.CellFor);
            var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();
            Assert.AreEqual(expectedTotal, total, $"Incoherent column sum for {t.name}");
        }
    }
}

