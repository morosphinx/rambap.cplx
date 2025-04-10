namespace rambap.cplxtests.UsageTests.ElectricalHarnesses;

using DocumentFormat.OpenXml.Packaging;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Modules.Connectivity.Templates;

class BoxAssembly1 : Part, IPartConnectable
{
    InternalHarness1 U01, U02;

    public ConnectablePort J11 => U01.J01;
    public ConnectablePort J12 => U01.J02;
    public ConnectablePort J21 => U02.J01;
    public ConnectablePort J22 => U02.J02;

    public void Assembly_Connections(ConnectionBuilder Do){}
}

class InternalHarness1 : Part, IPartConnectable, IPartAdditionalDocuments
{
    Description Usage = "Internal SubD To SubD to test lead harness, to be integrated in a rack";
        
    SubD9Connector_M C01;
    SubD_Backshell C11;

    SubD9Connector_M C02;
    SubD_Backshell C12; 

    TestLead_4mm C04;
    TestLead_4mm C05;

    public ConnectablePort J01 => C01;
    public ConnectablePort J02 => C02;
    public ConnectablePort PWR_P => C04.TestLead4mmMale;
    public ConnectablePort PWR_N => C05.TestLead4mmMale;

    [Rename("12V")]
    Signal _12V => this.SignalOf(C04.SolderPoint);
    Signal GND => this.SignalOf(C01.Pin(9));

    public void Additional_Documentation(DocumentationBuilder Do)
    {
        // TBD : specify this is cable, requiring Wiring Plan documentation, here ?
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // D38 to D38 connection
        Do.Wire(C01.Pin(1), C02.Pin(1));
        Do.Wire(C01.Pin(2), C02.Pin(2));
        Do.Wire(C01.Pin(3), C02.Pin(3));
        Do.Wire(C01.Pin(4), C02.Pin(4));
        Do.Wire(C01.Pin(5), C02.Pin(5));
        Do.Wire(C01.Pin(6), C02.Pin(6));
        Do.Wire(C01.Pin(7), C02.Pin(7));
        // Power
        Do.Wire(C01.Pin(8), C04.SolderPoint);
        Do.Wire(C01.Pin(9), C05.SolderPoint);
    }
}
