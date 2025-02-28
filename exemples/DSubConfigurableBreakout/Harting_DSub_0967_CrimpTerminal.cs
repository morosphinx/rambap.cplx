namespace rambap.cplxexemple.DSubConfigurableBreakout;


public enum DIN41652_PinCounts
{
    _09,
    _15,
    _25,
    _37,
    _50,
}

public enum DIN41652_Genders
{
    Male,
    Female
}

/// <summary>
/// Parametric part for Harting D-Sub DIN 41 652 · CECC 75 301-802 
/// Crimp terminal
///     (AKA : Connector shell)
/// Catalog page 05/26
/// </summary>
internal class Harting_DSub_0967_CrimpTerminal : Part
{

    public Link HartingWebsiteLink => $"https://www.harting.com/en-GB/q?query={PN.Replace(" ", "%20")}";

    public static int ToPinCount(DIN41652_PinCounts p)
        => p switch
        {
            DIN41652_PinCounts._09 => 9,
            DIN41652_PinCounts._15 => 15,
            DIN41652_PinCounts._25 => 25,
            DIN41652_PinCounts._37 => 37,
            DIN41652_PinCounts._50 => 50,
            _ => throw new NotImplementedException()
        };

    public Harting_DSub_0967_CrimpTerminal(DIN41652_Genders gender, DIN41652_PinCounts pinCount)
    {
        string strPinCount = pinCount.ToString().Substring(1);
        string strGender = gender switch
        {
            DIN41652_Genders.Male => "56",
            DIN41652_Genders.Female => "47",
            _ => throw new NotImplementedException()
        };
        PN = $"09 67 0{pinCount} {strGender}01";
    }
}

