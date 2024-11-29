namespace rambap.cplx.UnitTests;

[PN("456_123_456")]
[PartDescription("Eclateur SubD9 vers fiches banane 4mm")]
public class Breakout9 : Part
{
    ConnecteurSubD9 J01;

    Fiche4mmNoir J11, J12, J13, J14, J15, J16, J17, J18, J19;

    Gaine10mm Gaine;
    Cost Fil = 50;

    RecurrentTask Assembly = 0.5;
    NonRecurrentTask Achats = 2.0;
    NonRecurrentTask PlanCablage = 2;
    NonRecurrentTask Relecture = 4;

    ASticker Sticker;

    Part Sticket2;
}

class ASticker : Part
{
    
}

[PN("408-209")]
[PartDescription("Gaine Tressée 10mm RS PRO, Noir en PET, 5m")]
class Gaine10mm : Part
{
    Link rs = "https://fr.rs-online.com/web/p/gaines-electriques/0408209?gb=s";

    Cost Buy = 8 ;
}


[PN("MHDPPK9-DB9S-K")]
[PartDescription(
@"Connecteur SubD9 a souder. Avec backshell incluse")]
class ConnecteurSubD9 : Part
{
    Link rs = "https://fr.rs-online.com/web/p/connecteurs-sub-d/7659555?gb=s";

    Cost Buy = 5;
}




[PN("934099100")]
[PartDescription("Fiche de Test male protégée 4mm")]
class Fiche4mmNoir : Part
{

    Link rs = "https://fr.rs-online.com/web/p/fiches-bananes/3468623?gb=s";
    Link mouser = "https://eu.mouser.com/ProductDetail/Altech/934099100?qs=qOhVvio4tz5d2Q%2FT5L2UjQ%3D%3D";

    Cost Buy = 5;

    Description Manufacturer = "Hirshman";

    NonRecurrentTask Nego = 1.0;
    RecurrentTask Assembly = 0.05;
}

