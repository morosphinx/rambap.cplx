using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.UnitTests.ExportValidity;

internal static class ColumnTester
{
    public static ComponentIterator ComponentIterator_Flat_NoBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = false,
    };
    public static ComponentIterator ComponentIterator_Recursive_NoBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => true,
        WriteBranches = false,
    };
    public static ComponentIterator ComponentIterator_Flat_WithBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = true,
    };
    public static ComponentIterator ComponentIterator_Recursive_With_Branches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => true,
        WriteBranches = true,
    };


    public static PartTypesIterator<object> PartTypeIterator_Flat_NoBranches() => new PartTypesIterator<object>()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = false,
    };
    public static PartTypesIterator<object> PartTypeIterator_Recursive_NoBranches() => new PartTypesIterator<object>()
    {
        RecursionCondition = (c, l) => true,
        WriteBranches = false,
    };
    public static PartTypesIterator<object> PartTypeIterator_Flat_WithBranches() => new PartTypesIterator<object>()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = true,
    };
    public static PartTypesIterator<object> PartTypeIterator_Recursive_WithBranches() => new PartTypesIterator<object>()
    {
        RecursionCondition = (c, l) => true,
        WriteBranches = true,
    };


    public static ComponentIterator PartLocationIterator_Flat_NoBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => false,
        GroupPNsAtSameLocation = true,
        WriteBranches = false,
    };
    public static ComponentIterator PartLocationIterator_Recursive_NoBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => true,
        GroupPNsAtSameLocation = true,
        WriteBranches = false, // TODO : case write branch is true
    };

    public static ComponentIterator PartLocationIterator_Flat_WithBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => false,
        GroupPNsAtSameLocation = true,
        WriteBranches = true,
    };
    public static ComponentIterator PartLocationIterator_Recursive_WithBranches() => new ComponentIterator()
    {
        RecursionCondition = (c, l) => true,
        GroupPNsAtSameLocation = true,
        WriteBranches = true,
    };


    public static void TestDecimalColumn_SumCoherence(
        Pinstance pinstance,
        IIterator<IComponentContent> iterator,
        decimal expectedTotal,
        IColumn<IComponentContent> testedColumn,
        IEnumerable<IColumn<IComponentContent>> debugDataColumns)
    {
        var res = iterator.MakeContent(pinstance);
        var values = res.Select(testedColumn.CellFor);
        var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();

        // Write table in console for debug
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
        IColumn<IComponentContent> testedColumn)
    {
        var columnTotal = Convert.ToDecimal(testedColumn.TotalFor(pinstance));
        Assert.AreEqual(expectedTotal, columnTotal, $"Incoherent column autocalculated sum");
    }
}

