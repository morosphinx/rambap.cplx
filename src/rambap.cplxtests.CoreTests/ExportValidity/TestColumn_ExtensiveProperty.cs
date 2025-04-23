using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplxtests.CoreTests.ExportValidity.TestColumn_Support;

namespace rambap.cplxtests.CoreTests.ExportValidity;

/// <summary>
/// Test that a column representing an extensive property has a valid sum, no matter the iteration mode specified by the user
/// </summary>
/// <typeparam name="PART">An extensive part property</typeparam>
/// <typeparam name="ITER">Corresponding Pinstance property</typeparam>
public abstract class TestColumn_ExtensiveProperty<PART,ITER>
{
    /// <summary>
    /// Return properties from a Pinstance component <br/>
    /// Used when iterating the component trees
    /// </summary>
    protected abstract IEnumerable<ITER> PropertyIterator(Component component);

    /// <summary>
    /// Column whose sum will be checked
    /// </summary>
    protected abstract IColumn<ICplxContent> GetTestedColumn();

    /// <summary>
    /// For debug, return an indentifier name of the tested Pinstance property
    /// </summary>
    protected abstract string PropertyNaming(IPropertyContent<ITER> instanceProperty);

    /// <summary>
    /// For debug, other columns to write to the output
    /// </summary>
    protected abstract IEnumerable<IColumn<ICplxContent>> GetDebugColumns();

    /// <summary>
    /// For debug, other columns to write to the output
    /// </summary>
    private Part GetTestPart() => new DecimalPropertyPartExemple<PART>.Part_A();

    [TestMethod]
    private void Test_SelfTotal()
    {
        var part = GetTestPart();
        var component = part.Instantiate();
        TestDecimalColumn_SelfTotal(
            component,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            GetTestedColumn());
    }

    private void TestSumCoherence_ComponentIterator(bool recursive, bool writeBranches, bool groupPNsAtSameLocation = false)
        => TestSumCoherence_Iterator(
            new ComponentPropertyIterator<ITER>()
            {
                DocumentationPerimeter = new DocumentationPerimeter_WithInclusion() { InclusionCondition = c => recursive},
                WriteBranches = writeBranches,
                PropertyIterator = PropertyIterator,
                GroupPNsAtSameLocation = groupPNsAtSameLocation,
            });

    private void TestSumCoherence_PartTypeIterators(bool recursive, bool writeBranches)
        => TestSumCoherence_Iterator(
            new PartTypesIterator<ITER>()
            {
                DocumentationPerimeter = new DocumentationPerimeter_WithInclusion() { InclusionCondition = c => recursive },
                WriteBranches = writeBranches,
                PropertyIterator = PropertyIterator,
            });

    private void TestSumCoherence_Iterator(IIterator<ICplxContent> iterator)
    {
        var part = GetTestPart();
        var component = part.Instantiate();
        TestColumn_Support.TestDecimalColumn_SumCoherence<ITER>(
            component,
            iterator,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            GetTestedColumn(),
            PropertyNaming,
            GetDebugColumns());
    }

    [TestMethod]
    public void Test_ComponentIter_1() => TestSumCoherence_ComponentIterator(false, false);
    [TestMethod]    
    public void Test_ComponentIter_2() => TestSumCoherence_ComponentIterator(false, true);
    [TestMethod]    
    public void Test_ComponentIter_3() => TestSumCoherence_ComponentIterator(true, false);
    [TestMethod]    
    public void Test_ComponentIter_4() => TestSumCoherence_ComponentIterator(true, true);
    [TestMethod]    
    public void Test_PartIter_1() => TestSumCoherence_PartTypeIterators(false, false);
    [TestMethod]    
    public void Test_PartIter_2() => TestSumCoherence_PartTypeIterators(false, true);
    [TestMethod]    
    public void Test_PartIter_3() => TestSumCoherence_PartTypeIterators(true, false);
    [TestMethod]    
    public void Test_PartIter_4() => TestSumCoherence_PartTypeIterators(true, true);
    [TestMethod]    
    public void Test_ComponentIterGrouped_1() => TestSumCoherence_ComponentIterator(false, false, true);
    [TestMethod]    
    public void Test_ComponentIterGrouped_2() => TestSumCoherence_ComponentIterator(false, true, true);
    [TestMethod]    
    public void Test_ComponentIterGrouped_3() => TestSumCoherence_ComponentIterator(true, false, true);
    [TestMethod]    
    public void Test_ComponentIterGrouped_4() => TestSumCoherence_ComponentIterator(true, true, true);
}