namespace rambap.cplx.UnitTests;

// This file is copied as an example inside of README.md at the repo root
// Its formatting is, on purpose, more compact than the coding standard

class MaBaieIndustrielle : Part
{
    // Components
    RackAcquisition R01;
    RackAcquisition R02;
    RackAcquisition R03;
    RackAcquisitioncomplique R04;

    Cost Oublis = 20000;
}

internal class RackAcquisition : Part
{
    Cost Achat = 3000;

    public Carte C01;
    Carte C02;
}

internal class RackAcquisitioncomplique : RackAcquisition
{
    Carte C03;
    Carte C04;
}

internal class Carte : Part
{
    Cost Achat = 1500;
    Cost Assemblage = 400;
}