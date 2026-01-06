using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.CoreTests.Instantiation;

[TestClass]
public class PublicNullField : InstantiationCheckPart
{
    public EmptyPart MyComponent;
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
}
[TestClass]
public class PrivateNullField : InstantiationCheckPart
{
    EmptyPart MyComponent;
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
}


[TestClass]
public class NullProperty_Get : InstantiationCheckPart
{
    public EmptyPart MyComponent { get; }
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
}

[TestClass]
public class PublicNullProperty_GetSet : InstantiationCheckPart
{
    public EmptyPart MyComponent { get; set; }
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
}

[TestClass]
public class PrivateNullProperty_GetSet : InstantiationCheckPart
{
    EmptyPart MyComponent { get; set; }
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
}


[TestClass]
public class AutoField : InstantiationCheckPart
{
    public EmptyPart MyComponent = new() { CN = "CN_01" };
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
}

[TestClass]
public class AutoProperty_Get : InstantiationCheckPart
{
    public EmptyPart MyComponent { get; } = new() { CN = "CN_01" };
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
}

[TestClass]
public class AutoProperty_GetSet : InstantiationCheckPart
{
    public EmptyPart MyComponent { get; set; } = new() { CN = "CN_01" };
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
}

[TestClass]
public class ConstructedField : InstantiationCheckPart
{
    public EmptyPart MyComponent ;
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
    public ConstructedField()
    {
        MyComponent = new() { CN = "CN_01" };
    }
}

[TestClass]
public class ConstructedProperty : InstantiationCheckPart
{
    public EmptyPart MyComponent { get; init; }
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
    public ConstructedProperty()
    {
        MyComponent = new() { CN = "CN_01"};
    }
}

[TestClass]
public class UnbackedProperty : InstantiationCheckPart
{
    public EmptyPart MyComponent => new() { CN = "CN_01" };
    public override IEnumerable<string> ExpectedCNs => ["CN_01"];
}

[TestClass]
public class AliasedProperty : InstantiationCheckPart
{
    EmptyPart MyComponent { get; set; }
    public EmptyPart Alias => MyComponent;
    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
    public override IEnumerable<string> ForbiddenCNs => [nameof(Alias)];
}

[TestClass]
public class IgnoredProperty : InstantiationCheckPart
{
    EmptyPart MyComponent { get; set; }

    [CplxIgnore]
    public EmptyPart IgnoredComponent { get; }

    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
    public override IEnumerable<string> ForbiddenCNs => [nameof(IgnoredComponent)];
}

[TestClass]
public class IgnoredField : InstantiationCheckPart
{
    public EmptyPart MyComponent;

    [CplxIgnore]
    public EmptyPart IgnoredComponent;

    public override IEnumerable<string> ExpectedCNs => [nameof(MyComponent)];
    public override IEnumerable<string> ForbiddenCNs => [nameof(IgnoredComponent)];
}