using rambap.cplx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.CoreTests.Instantiation;

/// <summary>
/// Part used for Test, declaring its expected components after CplxInitialisation
/// </summary>
public abstract class InstantiationCheckPart : Part
{
    public abstract IEnumerable<string> ExpectedCNs { get; }
    public virtual IEnumerable<string> ForbiddenCNs => [];
    public virtual int ExpectedPartCount => ExpectedCNs.Count();

    private static void AssertIsCoherent(Component component)
    {
        // Assert no duplicate CN
        var subComponentCNs = component.SubComponents.Select(c => c.CN);
        Assert.AreEqual(subComponentCNs.Count(), subComponentCNs.Distinct().Count());
        // Assert all subcomponent have correct parent
        Assert.IsTrue(component.SubComponents.All(c => c.Parent == component));
    }
    private void AssertMatchExpectation(Component component)
    {
        // All Expected CN must be found
        foreach (var cn in ExpectedCNs)
            Assert.IsTrue(component.SubComponents.Any(c => c.CN == cn));
        // All Forbiddent CN must be missing
        foreach (var cn in ExpectedCNs)
            Assert.IsTrue(component.SubComponents.Any(c => c.CN == cn));
        // Check part count. Disabled if ExpectedPartCount < 0
        if (ExpectedPartCount >= 0)
            Assert.AreEqual(component.SubComponents.Count(), ExpectedPartCount);
    }

    public class EmptyPart : Part { }

    [TestMethod]
    public void TestSelfInstantiation()
    {
        var component = this.Instantiate();
        AssertIsCoherent(component);
        AssertMatchExpectation(component);
    }
}
