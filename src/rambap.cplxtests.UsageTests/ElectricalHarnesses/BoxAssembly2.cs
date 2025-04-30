using rambap.cplx.Modules.Connectivity;

using rambap.cplxtests.LibTests.Boxes;
using rambap.cplxtests.LibTests.Connectors;

namespace rambap.cplxtests.UsageTests.ElectricalHarnesses;
class BoxAssembly2 : Part, IPartConnectable
{
    InternalHarness2 U01, U02;

    public ConnectablePort J11 => U01.J01;
    public ConnectablePort J12 => U01.J02;
    public ConnectablePort J21 => U02.J01;
    public ConnectablePort J22 => U02.J02;

    public void Assembly_Connections(ConnectionBuilder Do){}
}

class InternalHarness2 : Part, IPartConnectable, IPartAdditionalDocuments
{
    Description Usage = "Internal SubD To SubD to test lead harness, to be integrated in a rack";
        
    DSub.DSub_1056 C01;
    DSub.Backshell_09 C11;

    DSub.DynamicDSub C02 = DSub.GetLibItem_Untyped(
        DSub.ContactCounts._09,
        DSub.ContactType.Male,
        false);
    DSub.Backshell_09 C12; 

    TestSocket2mm_Red C04;
    TestSocket2mm_Black C05;

    public ConnectablePort J01 => C01;
    public ConnectablePort J02 => C02;
    public ConnectablePort PWR_P => C04.Socket2mm;
    public ConnectablePort PWR_N => C05.Socket2mm;

    [Rename("12V")]
    Signal _12V => this.SignalOf(C04.WiringTab);
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
        Do.Wire(C01.Pin(8), C04.WiringTab);
        Do.Wire(C01.Pin(9), C05.WiringTab);
    }
}
