using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplxexemples.DSubConfigurableBreakout.Harting_DSub_0967_TurnedCrimpContacts;
using static rambap.cplxexemples.DSubConfigurableBreakout.Harting_DSub_0967_CrimpTerminal;


namespace rambap.cplxexemples.DSubConfigurableBreakout;

internal class Harting_DSub_0967_CrimpAssembly : Connector
{
    Harting_DSub_0967_CrimpTerminal Shell;

    public bool UseHighEndCrimpContact { get; init; } = false;
    public ContactWireGauge Gauge { get; } = ContactWireGauge._24_20;


    private static IEnumerable<(string, Pin)> MakePins(
        DIN41652_Genders gender,
        DIN41652_PinCounts pinArrangement,
        ContactWireGauge gauge,
        bool useHighEndCrimpContact)
    {
        var pinCount = ToPinCount(pinArrangement);
        foreach (var i in Enumerable.Range(1, pinCount))
        {
            var pinName = $"{i}";
            var pin = new Harting_DSub_0967_TurnedCrimpContacts(gender, useHighEndCrimpContact, gauge);
            yield return (pinName, pin);
        }
    }

    public Harting_DSub_0967_CrimpAssembly(
        DIN41652_Genders gender,
        DIN41652_PinCounts pinCount,
        ContactWireGauge gauge = ContactWireGauge._24_20)
        : base(MakePins(gender, pinCount, gauge, false))
    {
        PN = $"DSub{pinCount}_{gender}";
        Shell = new Harting_DSub_0967_CrimpTerminal(gender, pinCount);
    }
}
