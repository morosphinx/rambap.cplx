# cplx Core Concept Syntaxes

cplx definitions are modular, and managed by ConceptsModules.

Below are exemples for all Concepts defined in cplx.Core.

## Cost Concept

The cost Concept calculate costs & manage supplier prices :
``` Csharp
[PN("20860-229")]
class MultipacPRO4U : Part
{
	Offer Radiospare = new()
	{
		SKU = "RS:469-1149",
		Price = 120,
		Link = "https://fr.rs-online.com/web/p/chassis/4691149"
	}
}

class MyElectronicRack : Part
{
	MultipacPRO4U Chassis;
	Cost Machining = 400;
}
```

## Description Concept
The description Concept store textual information and documentation links :
``` Csharp
[PN("20860-229")]
[PartDescription("MultipacPRO 19\" Chassis Basic Kit, 4 U, Depth 340 mm")]
class MultipacPRO4U : Part
{
	Description Material = "Aluminum";
	Link SchroffWebsite = "https://www.nvent.com/en-us/schroff/products/enc20860-229"; 
}
```


## Manufacturer Concept

## Mass Concept

## Slot Concept

## Task Concept

## Conenctivity Concept

WiringPort NOT begin combine-able => implying the structural equivalent (connection port) they are assigned to is their path endpoint identity

