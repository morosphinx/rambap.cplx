using rambap.cplx.Modules.Connectivity.Templates;

using static rambap.cplxtests.LibTests.AVCircular.ShellSize;
using static rambap.cplxtests.LibTests.AVCircular.ContactSize;
using static rambap.cplxtests.LibTests.AVCircular.Layout;

namespace rambap.cplxtests.LibTests;

/// <summary>
/// A fake, abstract avionic circular series of connector with removable contact <br/>
/// Loosely copied from the MIL-DTL-D38999 / EN3645 series, in order to see if the use case match. <br/>
/// Part numbering of this serie is a constructed significant - the PN specify the connector <br/>
/// The number of possible PN in this serie exceed 1000 + , therefore this lib must provide dynamic Parts <br/>
/// </summary>
public static class AVCircular
{
    public enum ShellStyle
    {
        SquareFlangeReceptable, // code 20
        JamNutReceptable, // code 24
        Plug // code 26
    }

    public enum ShellSize
    {
        A_09,
        B_11,
        C_13,
        D_15,
        E_17,
        F_19,
        G_21,
        H_23,
        J_25,
    }

    public enum Keying
    {
        N,
        A,
        B,
        C,
        D,
        E
    }

    public enum PlattingFinish
    {
        Nickel, // code F
        OliveDrabCadium, // code W
        BlackZincNickel, // code Z
    }

    public enum ContactType
    {
        P,
        N,
    }

    public enum ContactOrderCount
    {
        Standard, // Contact Type code is P / N
        LessContact, // Contact Type code is A / B
        NoContact, // Contact type code is P / N , and Optional delivery is L
    }

    public enum ContactSize
    {
        _26,
        _22D,
        _20,
        _16,
        _12,
        _10,
        // Multiple kind of pins exist for the size 8
        // TODO : TBD / Behavior to allow non standard substitutions
        _8_Power,
        _8_Coax,
        _8_Triax,
        _8_Quadrax,
        _4_Power,
    }

    public enum Layout
    {
        A35,
        B02,
        B04,
        B05,
        B35,
        B98,
        B99,
        C04,
        C08,
        C35,
        C98,
        D05,
        D15,
        D18,
        D19,
        D35,
        D97,
        E02,
        E06,
        E08,
        E26,
        E35,
        E99,
        F11,
        F18,
        F28,
        F32,
        F35,
        G11,
        G16,
        G35,
        G39,
        G41,
        G75,
        H21,
        H32,
        H35,
        H53,
        H55,
        J04,
        J07,
        J08,
        J11,
        J19,
        J20,
        J24,
        J29,
        J35,
        J37,
        J43,
        J46,
        J61,
        J90,
    }

    private static IEnumerable<ContactSize> ctc(int count, ContactSize size)
        => Enumerable.Range(0, count).Select(i => size);

    // Order of pin matter, match the numbering pattern
    private static List<(ShellSize, Layout, IEnumerable<ContactSize>)> LayoutDefinitions { get; } =
        [
            (A_09, A35, [.. ctc(6, _22D) ]),
            (B_11, B02, [.. ctc(2, _16) ]),
            (B_11, B04, [.. ctc(4, _20) ]),
            (B_11, B05, [.. ctc(5, _20) ]),
            (B_11, B35, [.. ctc(13, _22D) ]),
            (B_11, B98, [.. ctc(6, _20) ]),
            (B_11, B99, [.. ctc(7, _20) ]),
            (C_13, C04, [.. ctc(4, _16) ]),
            (C_13, C08, [.. ctc(8, _20) ]),
            (C_13, C35, [.. ctc(22, _22D) ]),
            (C_13, C98, [.. ctc(10, _20) ]),
            (D_15, D05, [.. ctc(5, _16) ]),
            (D_15, D15, [.. ctc(13, _20), .. ctc(1, _16), .. ctc(1, _20) ]),
            (D_15, D18, [.. ctc(18, _20) ]),
            (D_15, D19, [.. ctc(19, _20) ]),
            (D_15, D35, [.. ctc(37, _22D) ]),
            (D_15, D97, [.. ctc(2, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(2, _16),]),
            (E_17, E02, [.. ctc(38, _22D), .. ctc(1, _8_Triax)]), // Triax pin has no numbering on the insert ?
            (E_17, E06, [.. ctc(6, _12) ]),
            (E_17, E08, [.. ctc(8, _16) ]),
            (E_17, E26, [.. ctc(26, _20) ]),
            (E_17, E35, [.. ctc(55, _22D) ]),
            (E_17, E99, [.. ctc(20, _20), .. ctc(1, _16), .. ctc(1, _20),.. ctc(1, _16),  ]),
            (F_19, F11, [.. ctc(11, _16)  ]),
            (F_19, F18, [.. ctc(14, _22D), .. ctc(4, _8_Triax) ]), // No numbering ?
            (F_19, F28, [.. ctc(22, _20), .. ctc(1, _16), .. ctc(4, _20), .. ctc(1, _16) ]),
            (F_19, F32, [.. ctc(32, _20) ]),
            (F_19, F35, [.. ctc(66, _22D) ]),
            (G_21, G11, [.. ctc(11, _12) ]),
            (G_21, G16, [.. ctc(16, _16) ]),
            (G_21, G35, [.. ctc(79, _22D) ]),
            (G_21, G39, [.. ctc(34, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(1, _16)  ]),
            (G_21, G41, [.. ctc(41, _20) ]),
            (G_21, G75, [.. ctc(4, _8_Triax) ]),
            (H_23, H21, [.. ctc(21, _16)  ]),
            (H_23, H32, [.. ctc(32, _20) ]),
            (H_23, H35, [.. ctc(100, _22D) ]),
            (H_23, H53, [.. ctc(53, _20) ]),
            (H_23, H55, [.. ctc(55, _20) ]),
            (J_25, J04, [.. ctc(43, _20), .. ctc(3, _16), .. ctc(2, _20), .. ctc(3, _16),
                .. ctc(2, _20), .. ctc(1, _16), .. ctc(1, _20), .. ctc(1, _16)]), // Total 48#20 & 8#16
            (J_25, J07, [.. ctc(24, _20), .. ctc(1, _8_Triax), .. ctc(49, _20), .. ctc(1, _8_Triax), .. ctc(24, _20)]), // Total 97#20 & 2#Triax
            (J_25, J08, [.. ctc(8, _8_Triax) ]),
            (J_25, J11, [.. ctc(9, _10), .. ctc(2, _20) ]),
            (J_25, J19, [.. ctc(19, _12) ]),
            // (J_25, J20, [ ]), // J_25 will wait. Only standard occurence of #12 Coax ?
            (J_25, J24, [.. ctc(2, _12), .. ctc(1, _16),.. ctc(2, _12), .. ctc(5, _16), .. ctc(2, _12), .. ctc(1, _16),
                .. ctc(4, _12), .. ctc(3, _16), .. ctc(2, _12), .. ctc(2, _16)]), // Total 12#16 & 12#12
            (J_25, J29, [.. ctc(29, _12) ]),
            (J_25, J35, [.. ctc(128, _22D)  ]),
            (J_25, J37, [.. ctc(37, _16) ]),
            (J_25, J43, [.. ctc(23, _20), .. ctc(20, _16) ]),
            (J_25, J46, [.. ctc(40, _20), .. ctc(1, _16), .. ctc(1, _8_Coax), .. ctc(2, _16), .. ctc(1, _8_Coax), .. ctc(1, _16)]),
            (J_25, J61, [.. ctc(61, _20) ]),
            (J_25, J90, [.. ctc(40, _20), .. ctc(1, _16), .. ctc(1, _8_Triax), .. ctc(2, _16), .. ctc(1, _8_Triax), .. ctc(1, _16)]),
        ];


    public record ConnectorConfiguration
    {
        ShellStyle ShellStyle { get; init; }
        ShellSize ShellSize { get; init; }
        ContactType ContactType { get; init; }
        ContactOrderCount ContactOrderCount { get; init; }
        Keying Keying { get; init; }
    }
}
