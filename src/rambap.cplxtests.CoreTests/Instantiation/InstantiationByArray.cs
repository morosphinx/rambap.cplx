using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.CoreTests.Instantiation;


[TestClass]
public class Enumerable_ListAutoField : InstantiationCheckPart
{
    public List<EmptyPart> MyList = [new EmptyPart(), new EmptyPart(),new EmptyPart()];
    public override IEnumerable<string> ExpectedCNs => ["MyList_00", "MyList_01", "MyList_02"];
}

[TestClass]
public class Enumerable_ListAutoProperty : InstantiationCheckPart
{
    public List<EmptyPart> MyList { get; } = [new EmptyPart(), new EmptyPart(), new EmptyPart()];
    public override IEnumerable<string> ExpectedCNs => ["MyList_00", "MyList_01", "MyList_02"];
}

[TestClass]
public class Enumerable_ListConstructedField : InstantiationCheckPart
{
    public List<EmptyPart> MyList ;
    public override IEnumerable<string> ExpectedCNs => ["Name1", "Name2"];
    public Enumerable_ListConstructedField()
    {
        MyList = [
            new EmptyPart() { CN = "Name1" },
            new EmptyPart() { CN = "Name2" },
            ];
    }
}

[TestClass]
public class Enumerable_ListConstructedProperty : InstantiationCheckPart
{
    public List<EmptyPart> MyList { get; }
    public override IEnumerable<string> ExpectedCNs => ["Name1", "Name2"];
    public Enumerable_ListConstructedProperty()
    {
        MyList = [
            new EmptyPart() { CN = "Name1" },
            new EmptyPart() { CN = "Name2" },
            ];
    }
}