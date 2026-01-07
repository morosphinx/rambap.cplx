using rambap.cplx.Instantiation;

namespace rambap.cplxtests.CoreTests.Instantiation;

/// <summary>
/// Part used for Test, declaring its expected components after CplxInitialisation
/// </summary>
public abstract class InstantiationCheckPart : Part
{
    public abstract IEnumerable<string> ExpectedCNs { get; }
    public virtual IEnumerable<string> ForbiddenCNs => [];
    public virtual bool AllowUnexpected => false;

    private void ListComponent(Component component)
    {
        Console.WriteLine($"Expected : {string.Join(" ", ExpectedCNs)}");
        Console.WriteLine($"Forbidden : {string.Join(" ", ForbiddenCNs)}");
        Console.WriteLine();
        Console.WriteLine($"Found : {string.Join(" ", component.SubComponents.Select(c => c.CN))}");
    }

    private void AssertSubComponentCoherent(Component component)
    {
        // Assert all subcomponent have correct parent
        Assert.IsTrue(component.SubComponents.All(c => c.Parent == component));
    }

    private List<T> IntersectWithDuplicates<T>(IEnumerable<T> left, IEnumerable<T> right, out List<T> rightExclusion)
    {
        var temp = right.ToList();
        var intersection = left.Where(temp.Remove).ToList();
        rightExclusion = temp;
        return intersection;
    }
    private void AssertContentMatchExpectation(Component component)
    {
        var subcomponentCNs = component.SubComponents.Select(c => c.CN);
        var allExpectedFound = IntersectWithDuplicates(
            ExpectedCNs, subcomponentCNs, out var unexpected);
        var areAllExpectedFound = allExpectedFound.Count == ExpectedCNs.Count();
        var noUnexpected = unexpected.Count == 0;

        // All Expected CN must be found
        Assert.IsTrue(areAllExpectedFound);
        if(!AllowUnexpected) Assert.IsTrue(noUnexpected);

        // All Forbidden CN must be missing
        foreach (var cn in ExpectedCNs)
            Assert.IsTrue(component.SubComponents.Any(c => c.CN == cn));
    }

    public class EmptyPart : Part { }

    [TestMethod]
    public void TestSelfInstantiation()
    {
        var component = this.Instantiate();
        ListComponent(component);
        AssertSubComponentCoherent(component);
        AssertContentMatchExpectation(component);
    }
}
