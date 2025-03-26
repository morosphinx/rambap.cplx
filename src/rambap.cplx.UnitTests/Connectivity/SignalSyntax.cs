using static rambap.cplx.UnitTests.Connectivity.ExtensionTest;

namespace rambap.cplx.UnitTests.Connectivity;

[TestClass]
public class SingleSignalSyntax
{
    private void TestSingleSignalPartSyntax<P>()
        where P : Part, new()
        => TestSingleSignalPartSyntax(new P());
    private void TestSingleSignalPartSyntax(Part part)
    {
        var instance = new Pinstance(part);
        var connectivity = instance.Connectivity();
        Assert.IsNotNull(connectivity);
        Assert.IsTrue(connectivity.Signals.Count() == 1);
        Assert.IsTrue(connectivity.Signals.Single().Label == "SignalName");
        Assert.IsTrue(connectivity.Signals.Single().Owner == instance);
    }


    [TestMethod] public void TestA() => TestSingleSignalPartSyntax<SingleSignalSyntaxExemples.A>();
    [TestMethod] public void TestB() => TestSingleSignalPartSyntax<SingleSignalSyntaxExemples.B>();
    [TestMethod] public void TestC() => TestSingleSignalPartSyntax<SingleSignalSyntaxExemples.C>();
    [TestMethod] public void TestD() => TestSingleSignalPartSyntax<SingleSignalSyntaxExemples.D>();
}

public class SingleSignalSyntaxExemples
{
    public class A : Part
    {
        public Signal SignalName;
    }

    public class B : Part
    {
        List<Signal> Signals = [new Signal() { Name = "SignalName"}];
    }

    public class C : Part
    {
        List<Signal> Signals = [];

        public C()
        {
            Signals.Add(new Signal() { Name = "SignalName" });
        }
    }

    public class D : Part
    {
        A ComponentA;
        Signal SignalName => ComponentA.SignalName;
    }
}


[TestClass]
public class WiredSignalSyntax
{
    private void TestWiredSignalPartSyntax<P>()
        where P : Part, new()
        => TestWiredSignalPartSyntax(new P());
    private void TestWiredSignalPartSyntax(Part part)
    {
        var instance = new Pinstance(part);
        var connectivity = instance.Connectivity();
        Assert.IsNotNull(connectivity);
        Assert.IsTrue(connectivity.Signals.Count() == 1);
        Assert.IsTrue(connectivity.Signals.Single().Label == "SignalName");
        Assert.IsTrue(connectivity.Signals.Single().Owner == instance);
        var signal = connectivity.Signals.Single(); 

        Assert.IsTrue(connectivity.Wireables.Count() == 1);
        Assert.IsNotNull(connectivity.Wireables.Single().AssignedSignal);
        Assert.IsTrue(connectivity.Wireables.Single().AssignedSignal == signal);
    }


    [TestMethod] public void TestA() => TestWiredSignalPartSyntax<WiredSignalSyntaxExemples.A>();
    [TestMethod] public void TestB() => TestWiredSignalPartSyntax<WiredSignalSyntaxExemples.B>();
    [TestMethod] public void TestC() => TestWiredSignalPartSyntax<WiredSignalSyntaxExemples.C>();
    [TestMethod] public void TestD() => TestWiredSignalPartSyntax<WiredSignalSyntaxExemples.D>();
}
public class WiredSignalSyntaxExemples
{
    public class A : Part, IPartSignalDefining
    {
        public Signal SignalName;
         
        public WireablePort PortA;

        public void Assembly_Signals(SignalBuilder Do)
        {
            Do.Assign(SignalName,PortA);
        }
    }
    public class B : Part
    {
        Signal SignalName => (Signal) PortA;

        WireablePort PortA;
    }

    public class C : Part
    {
        Signal SignalName;

        WireablePort PortA = new();

        public C()
        {
            SignalName = (Signal) PortA;
        }
    }
    public class D : Part
    {
        Signal SignalName => this.WireSignalAsExtensionMethod(PortA);

        WireablePort PortA;
    }
}

public static class ExtensionTest
{
    //TBD : a way to have information on declaring class at the call site ?
    public static Signal WireSignalAsExtensionMethod(this Part a, WireablePort port)
        => new ImplicitAssignedSignal() { AssignedPorts = [port] };
}