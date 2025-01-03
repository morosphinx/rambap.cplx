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


    public static PartTypesIterator PartTypeIterator_Flat_NoBranches() => new PartTypesIterator()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = false,
    };
    public static PartTypesIterator PartTypeIterator_Recursive_NoBranches() => new PartTypesIterator()
    {
        RecursionCondition = (c, l) => true,
        WriteBranches = false,
    };
    public static PartTypesIterator PartTypeIterator_Flat_WithBranches() => new PartTypesIterator()
    {
        RecursionCondition = (c, l) => false,
        WriteBranches = true,
    };
    public static PartTypesIterator PartTypeIterator_Recursive_WithBranches() => new PartTypesIterator()
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
        IIterator<ComponentContent> iterator,
        decimal expectedTotal,
        IColumn<ComponentContent> testedColumn,
        IEnumerable<IColumn<ComponentContent>> debugDataColumns)
    {
        var res = iterator.MakeContent(pinstance);
        var values = res.Select(testedColumn.CellFor);
        var total = values.Select(s => (s != "") ? Convert.ToDecimal(s) : 0M).Sum();

        // Write table in console for debug
        var debugTable = new TextTableFile(pinstance)
        {
            Table = new TableProducer<ComponentContent>()
            {
                Columns =
                [
                    CommonColumns.LineNumber(),
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
        IColumn<ComponentContent> testedColumn)
    {
        var columnTotal = Convert.ToDecimal(testedColumn.TotalFor(pinstance));
        Assert.AreEqual(expectedTotal, columnTotal, $"Incoherent column autocalculated sum");
    }

    public static void SetPropertyIterator(IIterator<ComponentContent> iterator,
        Func<Pinstance, IEnumerable<object>>? propertyIterator)
    {
        switch(iterator)
        {
            case ComponentIterator ci:
                ci.PropertyIterator = propertyIterator;
                break;
            case PartTypesIterator pi:
                pi.PropertyIterator = propertyIterator;
                break;
            default: throw new NotImplementedException();
        };
    }
}

