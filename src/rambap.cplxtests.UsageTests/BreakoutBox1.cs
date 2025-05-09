﻿namespace rambap.cplxtests.UsageTests;

[PN("456_123_456")]
[PartDescription("Eclateur SubD9 vers fiches banane 4mm")]
public class BreakoutBox1 : Part
{
    ConnecteurSubD9 J01;

    [ComponentDescription("Fiches 4mm Noires")]
    Fiche4mmNoir J11, J12, J13, J14, J15, J16, J17, J18, J19;

    BraidedCableSleeve10mm CableSleeve;
    Cost Wire = 50;

    RecurrentTask Assembly = 0.5;
    NonRecurrentTask Achats = (2.0, TaskCategory.Delivery);
    NonRecurrentTask PlanCablage = new(2,"Electronics");
    NonRecurrentTask Relecture = new(4,"Validation");

    BreakoutPCB PCB01;

    [ComponentDescription("L Identification")]
    ASticker Sticker;

    [ComponentDescription("R Identification")]
    ASticker SecondSticker;

    Part Sticket2;
}

[CommonName("PCB")]
class BreakoutPCB : Part
{
    Cost Components = 250.74;

    NonRecurrentTask MCU_Selection = (3, TaskCategory.Software);
    NonRecurrentTask MCU_Dev = (15, TaskCategory.Software);
}

class ASticker : Part
{
    NonRecurrentTask Design = 2.2;

}

[PN("408-209")]
[PartDescription("Gaine Tressée 10mm RS PRO, Noir en PET, 5m")]
class BraidedCableSleeve10mm : Part
{
    Link rs = "https://fr.rs-online.com/web/p/gaines-electriques/0408209?gb=s";

    Cost Buy = 8.2 ;
}


[PN("MHDPPK9-DB9S-K")]
[PartDescription(@"Connecteur SubD9 a souder. Avec backshell incluse")]
[CommonName("DSub9")]
class ConnecteurSubD9 : Part
{
    Link rs = "https://fr.rs-online.com/web/p/connecteurs-sub-d/7659555?gb=s";

    Cost Buy = 5.54;
}




[PN("934099100")]
[PartDescription("Fiche de Test male protégée 4mm")]
[CommonName("4mm Socket")]
class Fiche4mmNoir : Part
{

    Link rs = "https://fr.rs-online.com/web/p/fiches-bananes/3468623?gb=s";
    Link mouser = "https://eu.mouser.com/ProductDetail/Altech/934099100?qs=qOhVvio4tz5d2Q%2FT5L2UjQ%3D%3D";

    Cost Buy = 5;

    Description Manufacturer = "Hirshman";

    NonRecurrentTask Nego = 1.0;
    RecurrentTask Assembly = 0.05;

    SubComp A;
    SubComp B;
}

class SubComp : Part
{
    Cost Buy = 4;
    NonRecurrentTask test = 1;
}

