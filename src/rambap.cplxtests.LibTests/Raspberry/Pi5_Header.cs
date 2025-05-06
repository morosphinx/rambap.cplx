using rambap.cplx.Modules.Connectivity;
using rambap.cplx.Modules.Connectivity.Templates;
using static System.Net.WebRequestMethods;

namespace rambap.cplxtests.LibTests.Raspberry;

public class PinHeader : Pin { }
[CplxHideContents]
public class _40PinHeader : Connector<PinHeader>
{
    public _40PinHeader() : base(40){}
}
public class Pi5_Header : _40PinHeader
{
    Link Documentation = "https://www.raspberrypi.com/documentation/computers/raspberry-pi.html#gpio";

    public Signal _3V3_power => SignalOf(2, 4);
    public Signal _5_power =>   SignalOf(1, 17);
    public Signal Ground =>     SignalOf(6, 9, 14, 20, 25, 30, 34, 39);
    public Signal GPIO_1 =>     SignalOf(28);
    public Signal GPIO_2 =>     SignalOf(3);
    public Signal GPIO_3 =>     SignalOf(5);
    public Signal GPIO_4 =>     SignalOf(7);
    public Signal GPIO_5 =>     SignalOf(31);
    public Signal GPIO_6 =>     SignalOf(29);
    public Signal GPIO_7 =>     SignalOf(26);
    public Signal GPIO_8 =>     SignalOf(24);
    public Signal GPIO_9 =>     SignalOf(21);
    public Signal GPIO_10 =>    SignalOf(19);
    public Signal GPIO_11 =>    SignalOf(23);
    public Signal GPIO_12 =>    SignalOf(32);
    public Signal GPIO_13 =>    SignalOf(33);
    public Signal GPIO_14 =>    SignalOf(8);
    public Signal GPIO_15 =>    SignalOf(10);
    public Signal GPIO_16 =>    SignalOf(36);
    public Signal GPIO_17 =>    SignalOf(11);
    public Signal GPIO_18 =>    SignalOf(12);
    public Signal GPIO_19 =>    SignalOf(35);
    public Signal GPIO_20 =>    SignalOf(38);
    public Signal GPIO_21 =>    SignalOf(40);
    public Signal GPIO_22 =>    SignalOf(15);
    public Signal GPIO_23 =>    SignalOf(16);
    public Signal GPIO_24 =>    SignalOf(18);
    public Signal GPIO_25 =>    SignalOf(22);
    public Signal GPIO_26 =>    SignalOf(37);
    public Signal GPIO_27 =>    SignalOf(13);
}
