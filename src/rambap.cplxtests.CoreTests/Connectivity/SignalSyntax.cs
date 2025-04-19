using rambap.cplx;
using rambap.cplx.Modules.Connectivity;

namespace rambap.cplxtests.CoreTests.Connectivity;

[TestClass]
public class SingleSignalSyntax
{
    private void TestSingleSignalPartSyntax<P>()
        where P : Part, new()
        => TestSingleSignalPartSyntax(new P());
    private void TestSingleSignalPartSyntax(Part part)
    {
        var instance = part.Instantiate().Instance;
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
        var instance = part.Instantiate().Instance;
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
        Signal SignalName => this.SignalOf(PortA);

        WireablePort PortA;
    }

    public class C : Part
    {
        Signal SignalName;

        WireablePort PortA = new();

        public C()
        {
            SignalName = this.SignalOf(PortA);
        }
    }
    public class D : Part
    {
        Signal SignalName => this.SignalOf(PortA);

        WireablePort PortA;
    }
}

[TestClass]
public class WiredComposedSignalSyntax
{
    private void TestWiredComposedSignalSyntaxPartSyntax<P>()
        where P : Part, new()
        => TestWiredComposedSignalSyntaxPartSyntax(new P());
    private void TestWiredComposedSignalSyntaxPartSyntax(Part part)
    {
        var component = part.Instantiate();
        var connectivity = component.Instance.Connectivity();
        Assert.IsNotNull(connectivity);

        Assert.IsTrue(connectivity.Wirings.Count() == 1);
        var wiring = connectivity.Wirings.Single();
        var lcomponent = component.SubComponents.ElementAt(0).Instance;
        var rcomponent = component.SubComponents.ElementAt(1).Instance;

        Assert.IsNotNull(wiring.LeftPort.AssignedSignal);
        Assert.ReferenceEquals(wiring.LeftPort.AssignedSignal,lcomponent.Connectivity()!.Signals.Single());

        Assert.IsNotNull(wiring.RightPort.AssignedSignal);
        Assert.ReferenceEquals(wiring.RightPort.AssignedSignal, rcomponent.Connectivity()!.Signals.Single());

        Assert.AreNotEqual(wiring.LeftPort.AssignedSignal, wiring.RightPort.AssignedSignal);
    }


    [TestMethod] public void TestA1() => TestWiredComposedSignalSyntaxPartSyntax<WiredComposedSignalSyntaxExemples.A1>();
    [TestMethod] public void TestA2() => TestWiredComposedSignalSyntaxPartSyntax<WiredComposedSignalSyntaxExemples.A2>();
    [TestMethod] public void TestB1() => TestWiredComposedSignalSyntaxPartSyntax<WiredComposedSignalSyntaxExemples.B1>();
    [TestMethod] public void TestB2() => TestWiredComposedSignalSyntaxPartSyntax<WiredComposedSignalSyntaxExemples.B2>();
}

public class WiredComposedSignalSyntaxExemples
{
    public class ComponentA : Part, IPartSignalDefining
    {
        public Signal Signal;
        public WireablePort Port;
        public void Assembly_Signals(SignalBuilder Do)
            => Do.Assign(Signal, Port);
    }
    public class A1 : Part, IPartConnectable
    {
        ComponentA C1, C2;

        public void Assembly_Connections(ConnectionBuilder Do)
            => Do.Wire(C1.Port, C2.Port);
    }
    public class A2 : Part, IPartConnectable
    {
        ComponentA C1, C2;

        public void Assembly_Connections(ConnectionBuilder Do)
            => Do.Wire(C1.Signal, C2.Signal);
    }

    public class ComponentB : Part
    {
        // TODO : either
        // 1 - forbid this
        // 2 - find a way to ensure unicity of returned reference
        // 3 - find a way to work around it during part construction
        //        (UID using the port ?)
        //        (Keep an internal implementation stack, like the port expositions?)
        public Signal Signal => this.SignalOf(Port);
        public WireablePort Port;
    }
    public class B1 : Part, IPartConnectable
    {
        ComponentB C1, C2;

        public void Assembly_Connections(ConnectionBuilder Do)
            => Do.Wire(C1.Port, C2.Port);
    }
    public class B2 : Part, IPartConnectable
    {
        ComponentB C1, C2;

        public void Assembly_Connections(ConnectionBuilder Do)
            => Do.Wire(C1.Signal, C2.Signal); // This fail, unbacked property recreate the Signal Object
    }
}