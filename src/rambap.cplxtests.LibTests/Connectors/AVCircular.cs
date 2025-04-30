using rambap.cplx.Modules.Connectivity.Templates;

using static rambap.cplxtests.LibTests.Connectors.AVCircular.ShellSize;
using static rambap.cplxtests.LibTests.Connectors.AVCircular.ContactSize;
using static rambap.cplxtests.LibTests.Connectors.AVCircular.Layout;
using static rambap.cplxtests.LibTests.Connectors.AVCircular.InsertNumberingScheme;

namespace rambap.cplxtests.LibTests.Connectors;

/// <summary>
/// A fake, abstract avionic circular series of connector with removable contact <br/>
/// Loosely copied from the MIL-DTL-D38999 series III / EN3645 series, in order to see if the use case match. <br/>
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

    public enum Keying // Polarization
    {
        N,
        A,
        B,
        C,
        D,
        E
    }

    public enum PlattingFinish // Class
    {
        Nickel, // code F
        OliveDrabCadium, // code W
        BlackZincNickel, // code Z
    }

    public enum ContactType // Contact Style
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
        // _8_Quadrax,
        // _4_Power,
    }

    public enum Layout
    {
        A35,
        A98,
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
        // J20,
        J24,
        J29,
        J35,
        J37,
        J43,
        J46,
        J61,
        J90,
    }

    public enum InsertNumberingScheme
    {
        // I made up those names. I do not know if there's a standard name for them
        NUMER, // 1, 2, 3, etc ...
        // In All ALPHx numbering scheme, the following letters ar skipped :
        // Uppercases : 'I', 'O', 'Q'
        // Lowercases : 'i', 'l', 'o'
        ALPH1, // Uppercase A, B, C, ... then lowercase a, b, c, ... then double uppercase AA, BB, CC ..
        ALPH2, // Like ALPH1, in addition, skip the lowwercase 'j'
        BENM1, // Uppercase A, B, C, ... then numerical 1, 2, 3
    }

    private static IEnumerable<ContactSize> ctc(int count, ContactSize size)
        => Enumerable.Range(0, count).Select(i => size);

    private record InsertDefinition
    {
        public ShellSize ShellSize { get; init; }
        public Layout Layout { get; init; }
        public InsertNumberingScheme Numbering { get; init; }
        public required List<ContactSize> Contacts { get; init; }

        public static implicit operator InsertDefinition(
                (ShellSize shellSize,
                Layout layout,
                InsertNumberingScheme numbering,
                IEnumerable<ContactSize> contacts) tuple)
            => new InsertDefinition()
            {
                ShellSize = tuple.shellSize,
                Layout = tuple.layout,
                Numbering = tuple.numbering,
                Contacts = [.. tuple.contacts],
            };
    };


    // Order of pin matter, match the numbering pattern
    // Only present here are standard arrangements offered by Souriau
    private static List<InsertDefinition> InsertDefinitions { get; } =
        [
            (A_09, A35, NUMER, [.. ctc(6, _22D) ]),
            (A_09, A98, NUMER, [.. ctc(3, _20) ]),
            (B_11, B02, ALPH1, [.. ctc(2, _16) ]),
            (B_11, B04, ALPH1, [.. ctc(4, _20) ]),
            (B_11, B05, ALPH1, [.. ctc(5, _20) ]),
            (B_11, B35, NUMER, [.. ctc(13, _22D) ]),
            (B_11, B98, ALPH1, [.. ctc(6, _20) ]),
            (B_11, B99, ALPH1, [.. ctc(7, _20) ]),
            (C_13, C04, ALPH1, [.. ctc(4, _16) ]),
            (C_13, C08, ALPH1, [.. ctc(8, _20) ]),
            (C_13, C35, NUMER, [.. ctc(22, _22D) ]),
            (C_13, C98, ALPH1, [.. ctc(10, _20) ]),
            (D_15, D05, ALPH1, [.. ctc(5, _16) ]),
            (D_15, D15, ALPH1, [.. ctc(13, _20), .. ctc(1, _16), .. ctc(1, _20) ]),
            (D_15, D18, ALPH1, [.. ctc(18, _20) ]),
            (D_15, D19, ALPH1, [.. ctc(19, _20) ]),
            (D_15, D35, NUMER, [.. ctc(37, _22D) ]),
            (D_15, D97, ALPH1, [.. ctc(2, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(2, _16),]),
            (E_17, E02, NUMER, [.. ctc(38, _22D), .. ctc(1, _8_Triax)]),
            (E_17, E06, ALPH1, [.. ctc(6, _12) ]),
            (E_17, E08, ALPH1, [.. ctc(8, _16) ]),
            (E_17, E26, ALPH1, [.. ctc(26, _20) ]),
            (E_17, E35, NUMER, [.. ctc(55, _22D) ]),
            (E_17, E99, ALPH1, [.. ctc(20, _20), .. ctc(1, _16), .. ctc(1, _20),.. ctc(1, _16),  ]),
            (F_19, F11, ALPH1, [.. ctc(11, _16)  ]),
            (F_19, F18, ALPH1, [.. ctc(1, _22D), .. ctc(1, _8_Triax), .. ctc(3, _22D), .. ctc(1, _8_Triax), .. ctc(3, _22D),
                .. ctc(1, _8_Triax), .. ctc(3, _22D), .. ctc(1, _8_Triax), .. ctc(4, _22D) ]), // Total 14#22D & 4#Triax
            (F_19, F28, ALPH1, [.. ctc(22, _20), .. ctc(1, _16), .. ctc(4, _20), .. ctc(1, _16) ]),
            (F_19, F32, ALPH1, [.. ctc(32, _20) ]),
            (F_19, F35, NUMER, [.. ctc(66, _22D) ]),
            (G_21, G11, ALPH1, [.. ctc(11, _12) ]),
            (G_21, G16, ALPH1, [.. ctc(16, _16) ]),
            (G_21, G35, NUMER, [.. ctc(79, _22D) ]),
            (G_21, G39, ALPH1, [.. ctc(34, _20), .. ctc(1, _16), .. ctc(3, _20), .. ctc(1, _16)  ]),
            (G_21, G41, ALPH1, [.. ctc(41, _20) ]),
            (G_21, G75, ALPH1, [.. ctc(4, _8_Triax) ]),
            (H_23, H21, ALPH1, [.. ctc(21, _16)  ]),
            (H_23, H32, ALPH1, [.. ctc(32, _20) ]),
            (H_23, H35, NUMER, [.. ctc(100, _22D) ]),
            (H_23, H53, ALPH2, [.. ctc(53, _20) ]),
            (H_23, H55, ALPH1, [.. ctc(55, _20) ]),
            (J_25, J04, ALPH2, [.. ctc(43, _20), .. ctc(3, _16), .. ctc(2, _20), .. ctc(3, _16),
                .. ctc(2, _20), .. ctc(1, _16), .. ctc(1, _20), .. ctc(1, _16)]), // Total 48#20 & 8#16
            (J_25, J07, NUMER, [.. ctc(24, _20), .. ctc(1, _8_Triax), .. ctc(49, _20), .. ctc(1, _8_Triax), .. ctc(24, _20)]), // Total 97#20 & 2#Triax
            (J_25, J08, ALPH1, [.. ctc(8, _8_Triax) ]),
            (J_25, J11, ALPH1, [.. ctc(9, _10), .. ctc(2, _20) ]),
            (J_25, J19, ALPH1, [.. ctc(19, _12) ]),
            // (J_25, J20, BENM1, [ ]), // J20 will wait. Only occurence of #12 Coax listed here
                                 // On top of it, #12 Coax contact PN are different depending on the socket
            (J_25, J24, ALPH1, [.. ctc(2, _12), .. ctc(1, _16),.. ctc(2, _12), .. ctc(5, _16), .. ctc(2, _12), .. ctc(1, _16),
                .. ctc(4, _12), .. ctc(3, _16), .. ctc(2, _12), .. ctc(2, _16)]), // Total 12#16 & 12#12
            (J_25, J29, ALPH1, [.. ctc(29, _12) ]),
            (J_25, J35, NUMER, [.. ctc(128, _22D)  ]),
            (J_25, J37, ALPH2, [.. ctc(37, _16) ]),
            (J_25, J43, ALPH2, [.. ctc(23, _20), .. ctc(20, _16) ]),
            (J_25, J46, ALPH2, [.. ctc(40, _20), .. ctc(1, _16), .. ctc(1, _8_Coax), .. ctc(2, _16), .. ctc(1, _8_Coax), .. ctc(1, _16)]),
            (J_25, J61, ALPH2, [.. ctc(61, _20) ]),
            (J_25, J90, ALPH2, [.. ctc(40, _20), .. ctc(1, _16), .. ctc(1, _8_Triax), .. ctc(2, _16), .. ctc(1, _8_Triax), .. ctc(1, _16)]),
        ];

    private static InsertDefinition InsertDefinitionFor(Layout layout)
        => InsertDefinitions.First(d => d.Layout == layout);

    public static ShellSize LayoutToShellSize(Layout layout)
        => InsertDefinitionFor(layout).ShellSize;

    public record ConnectorConfiguration
    {
        public ShellStyle ShellStyle { get; init; }
        public Layout ContactLayout { get; init; }
        public ShellSize ShellSize => LayoutToShellSize(ContactLayout);
        public ContactType ContactType { get; init; }
        public ContactOrderCount ContactOrderCount { get; init; }
        public Keying Keying { get; init; }
        public PlattingFinish PlattingFinish { get; init; }
    }

    public static string D38999_PN(ConnectorConfiguration config) // Per Souriau
    {
        int specSheetCode = config.ShellStyle switch
        {
            ShellStyle.SquareFlangeReceptable => 20,
            ShellStyle.JamNutReceptable => 24,
            ShellStyle.Plug => 26,
            _ => throw new NotImplementedException(),
        };
        string platingCode = config.PlattingFinish switch
        {
            PlattingFinish.Nickel => "F",
            PlattingFinish.OliveDrabCadium => "W",
            PlattingFinish.BlackZincNickel => "Z",
            _ => throw new NotImplementedException(),
        };
        string layoutCode = $"{config.ContactLayout}";
        string contactTypeCode =
            config.ContactOrderCount switch
            {
                ContactOrderCount.Standard or ContactOrderCount.NoContact => config.ContactType switch
                {
                    ContactType.P => "P",
                    ContactType.N => "N",
                    _ => throw new NotImplementedException(),
                },
                ContactOrderCount.LessContact => config.ContactType switch
                {
                    ContactType.P => "A",
                    ContactType.N => "B",
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        string orientationCode = $"{config.Keying}";
        string optionalDeliveryCode = config.ContactOrderCount switch
        {
            ContactOrderCount.NoContact => "L",
            _ => ""
        };
        // Uses a dash after the specification instead of the slash '/' of the standard,
        // because cplx already uses '/' for CID construction 
        return $"D38999-{specSheetCode}{platingCode}{layoutCode}{contactTypeCode}{orientationCode}{optionalDeliveryCode}";
    }


    public static IEnumerable<string> GetInsertLabels(InsertNumberingScheme scheme, int count)
    {
        return scheme switch
        {
            NUMER => GetInsertLabels_NUMER(count),
            ALPH1 => GetInsertLabels_ALPHA(count, false),
            ALPH2 => GetInsertLabels_ALPHA(count, true),
            BENM1 => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    private static List<string> GetInsertLabels_NUMER(int count)
        => Enumerable.Range(1, count).Select(i => $"{i}").ToList();

    private static List<string> GetInsertLabels_ALPHA(int count, bool isAlpha2)
    {
        var alphabetcharsUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList();
        var alphabetcharsLowercase = alphabetcharsUppercase.Select(c => char.ToLower(c)).ToList();

        List<char> uppercaseSkips = ['I', 'O', 'Q'];
        var labelsUppercase = alphabetcharsUppercase.Where(c => !uppercaseSkips.Contains(c));

        List<char> lowercaseSkips =  ['i', 'l', 'o'];
        if (isAlpha2) lowercaseSkips.Add('j');
        var labelsLowercases = alphabetcharsLowercase.Where(c => !lowercaseSkips.Contains(c));

        IEnumerable<string> validLabelSequence =
            [
                .. labelsUppercase.Select(c => $"{c}"),
                .. labelsLowercases.Select(c => $"{c}"),
                .. labelsUppercase.Select(c => $"{c}{c}"), // Double uppercases
            ];

        var labels = validLabelSequence.Take(count).ToList();
        if (labels.Count != count)
            throw new InvalidOperationException($"Numbering scheme ALPH{(isAlpha2 ? 2 : 1)} cannot produce that many labels ({count})");
        return labels;
    }

    public class ContactSize_26 : Pin { }
    public class ContactSize_22D : Pin { }
    public class ContactSize_20 : Pin { }
    public class ContactSize_16 : Pin { }
    public class ContactSize_12 : Pin { }
    public class ContactSize_10 : Pin { }
    public class ContactSize_8_Power : Pin { }
    public class ContactSize_8_Coax : Pin { } // Wrong abstraction ? This has multiple conductors
    public class ContactSize_8_Triax : Pin { } // Wrong abstraction ? This has multiple conductors

    public static Pin GetPin(ContactSize size)
    {
        return size switch
        {
            _26 => new ContactSize_26(),
            _22D => new ContactSize_22D(),
            _20 => new ContactSize_20(),
            _16 => new ContactSize_16(),
            _12 => new ContactSize_12(),
            _10 => new ContactSize_10(),
            _8_Power => new ContactSize_8_Power(),
            _8_Coax => new ContactSize_8_Coax(),
            _8_Triax => new ContactSize_8_Triax(),
            _ => throw new NotImplementedException()
        };
    }


    private static List<(string name, Pin)> GetPins(Layout layout)
    {
        var insertDefinition = InsertDefinitionFor(layout);
        return GetPins(insertDefinition.Numbering, insertDefinition.Contacts);
    }
    private static List<(string name, Pin pin)> GetPins(
        InsertNumberingScheme numberingScheme, List<ContactSize> sizes)
    {
        var labels = GetInsertLabels(numberingScheme, sizes.Count);
        List<(string name, Pin pin)> pins = new(); 
        int i = 0;
        foreach(var l in labels)
        {
            var pinSize = sizes[i];
            pins.Add((l, GetPin(pinSize)));
            i++;
        }
        return pins;
    }


    public class DynamicAVCircular : Connector
    {
        public ConnectorConfiguration Config { get; }

        internal DynamicAVCircular(ConnectorConfiguration config) :
            base(GetPins(config.ContactLayout))
        {
            PN = D38999_PN(config);
            Config = config;
        }
    }

    public static Connector GetLibItem(ConnectorConfiguration Config)
        => new DynamicAVCircular(Config);
}
