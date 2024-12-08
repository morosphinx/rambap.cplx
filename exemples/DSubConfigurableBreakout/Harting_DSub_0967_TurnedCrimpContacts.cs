namespace rambap.cplxexemple.DSubConfigurableBreakout;

/// <summary>
/// Parametric part for Harting D-Sub DIN 41 652 · CECC 75 301-802 
/// Turned crimp contacts
/// Catalog page 05/27
/// /// </summary>
internal class Harting_DSub_09_67_000_xx76 : Part
{
    public enum WireGauges
    {
        _22_18,
        _24_20,
        _26_22,
        _28_24,
    }

    public enum ContactType
    {
        Male,
        Female,
        HighEndFemale,
    }

    public Link HartingWebsiteLink;

    private static string GetHartinPNFor(ContactType type, WireGauges gauge)
        => gauge switch
        {
            WireGauges._22_18 => type switch
            {
                ContactType.Male            => "09 67 000 3576",
                ContactType.Female          => "09 67 000 3476",
                ContactType.HighEndFemale   => "09 67 000 3676",
                _ => throw new NotImplementedException(),
            },
            WireGauges._24_20 => type switch
            {
                ContactType.Male            => "09 67 000 8576",
                ContactType.Female          => "09 67 000 8476",
                ContactType.HighEndFemale   => "09 67 000 8676",
                _ => throw new NotImplementedException(),
            },
            WireGauges._26_22 => type switch
            {
                ContactType.Male            => "09 67 000 5576",
                ContactType.Female          => "09 67 000 5476",
                ContactType.HighEndFemale   => "09 67 000 5676",
                _ => throw new NotImplementedException(),
            },
            WireGauges._28_24 => type switch
            {
                ContactType.Male            => "09 67 000 7576",
                ContactType.Female          => "09 67 000 7476",
                ContactType.HighEndFemale   => "09 67 000 7676",
                _ => throw new NotImplementedException(),
            },
            _ => throw new NotImplementedException(),
        };

    public Harting_DSub_09_67_000_xx76(ContactType type, WireGauges gauges)
    {
        PN = GetHartinPNFor(type, gauges);
        HartingWebsiteLink = $"https://www.harting.com/en-GB/q?query={PN.Replace(" ", "%20")}";
    }
}
