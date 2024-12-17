namespace rambap.cplxexemple.DSubConfigurableBreakout;

internal class Harting_DSub_CrimpAssembly : Part
{
    Harting_09_67_0xx_yy01 Shell;
    List<Harting_DSub_09_67_000_xx76> Contacts = new();

    public Connector MatingFace;
    public List<Connector> CrimpContacts = new();

    public bool UseHighEndCrimpContact { get; init; } = false;
    public Harting_DSub_09_67_000_xx76.WireGauges Gauge { get; init; } = Harting_DSub_09_67_000_xx76.WireGauges._24_20;

    public Harting_DSub_CrimpAssembly(DIN41652_Genders gender, DIN41652_PinCounts pinCount)
    {
        PN = $"DSub{pinCount}_{gender}";
        Shell = new Harting_09_67_0xx_yy01(gender, pinCount);

        Harting_DSub_09_67_000_xx76.ContactType contactType = gender switch
        {
            DIN41652_Genders.Male =>
                Harting_DSub_09_67_000_xx76.ContactType.Male,
            DIN41652_Genders.Female => UseHighEndCrimpContact
                ? Harting_DSub_09_67_000_xx76.ContactType.HighEndFemale
                : Harting_DSub_09_67_000_xx76.ContactType.Female,
            _ => throw new NotImplementedException(),
        };

        foreach (var i in Enumerable.Range(1, Harting_09_67_0xx_yy01.ToPintCount(pinCount)))
        {
            Contacts.Add(new Harting_DSub_09_67_000_xx76(contactType, Gauge));
            CrimpContacts.Add(new Connector());
        }

        MatingFace = new Connector();
    }
}
