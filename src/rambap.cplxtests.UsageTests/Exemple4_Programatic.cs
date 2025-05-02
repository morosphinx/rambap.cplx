namespace rambap.cplxtests.UsageTests;

class Exemple4_Programatic : Part
{
    OctopusNthDegree BigOne = new(50);
    OctopusNthDegree SmallOne1 = new(10);
    OctopusNthDegree SmallOne2 = new(10);
    OctopusNthDegree SmallOne3 = new(10);
    OctopusNthDegree HumongousOne = new(1000);
}

class OctopusNthDegree : Part
{
    List<OctopusLeg> Legs = new ();
    public OctopusNthDegree(int legcount)
    {
        this.PN = $"OctopusNthDegree{legcount}";
        for(int i = 0; i < legcount; i++)
        {
            Legs.Add(new OctopusLeg() { CN = $"Leg{i}"});
        }
    }
}

class OctopusLeg : Part
{
    OctopusSucker S1, S2, S3, S4, S5, S6, S7, S8;
}

class OctopusSucker : Part
{
    Cost Buy = 5;
}
