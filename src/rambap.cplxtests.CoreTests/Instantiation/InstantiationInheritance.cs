
namespace rambap.cplxtests.CoreTests.Instantiation;

public abstract class BasePart : InstantiationCheckPart
{
    EmptyPart C_Base;
}

[TestClass]
public class DerivedDifferentName : BasePart
{
    EmptyPart C_Derived;

    public override IEnumerable<string> ExpectedCNs => ["C_Base", "C_Derived"];
}


[TestClass]
public class DerivedSameName : BasePart
{
    EmptyPart C_Base;

    public override IEnumerable<string> ExpectedCNs => ["C_Base", "C_Base"];
}