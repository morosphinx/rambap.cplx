### Parts & PN
cplx model **Parts**. Parts are plans to build or procure a thing. Defining a Part is a design activity.[^1]

[^1] : Parts are NOT references tracking real, physical objects, such as inventory items, [digital twins](https://en.wikipedia.org/wiki/Digital_twin), or anything with a [serial number ](https://en.wikipedia.org/wiki/Serial_number)

Parts are identified through an unique [**part number**](https://en.wikipedia.org/wiki/Part_number) (PN).

``` Csharp
// cplx define Parts as classes, using (by default) its classname as a PN
class BENCH_001 : Part
{
	// This define a part of PN 'R012_456'
}
```

### Components & CN
Part are composed of other parts. These are the part's **Components**. Each component is identified inside of the Part through a localy unique **Component Number** (CN). Each component is an **Instance** of the Part it reference : it carries information about the properties and usage of the part in the local context.

``` Csharp
// cplx define Components as any fields of a part that inherit from the Part class
// using (by default) the field name as the CN
class BENCH_001 : Part {
	RACK_597 C01 ; // Component CN 'C01', an instance of the part PN 'RACK_597' 
	RACK_597 C02 ; // Component CN 'C02', an instance of the part PN 'RACK_597' 
}
```

### Properties 
Parts have properties. Properties of the part and its components are used by [**Concepts**](2_DataConcepts) to calculate the overall properties of each part instance. These instances properties are then used to generate documentation.
``` Csharp
// cplx properties are defined in the cplx.Core.PartProperties namespace.
// Add them as field to each part
class BENCH_001 : Part {
	Cost PowerSupply = 5000 ;
	Cost Assembly = 8000 ;
	Mass_kg Mass = 80 ;
}
```

### Component Tree & CID
Composing multiple part together produces a **Component Tree**. Each component of the tree is identified through a path-like **Component Identifier** (CID), that aggregate the chain of CN required to reach it from the root part instance.
``` Csharp
// In the following example, from an instance of BENCH_001, the CID : '/C01/U02'
// Refer to the second CARD_8963 card of the first RACK_597 rack.
class BENCH_001 : Part {
	RACK_597 C01 ;
	RACK_597 C02 ;
}
class RACK_597 : Part {
	CARD_8963 U01 ;
	CARD_8963 U02 ;
}
class CARD_8963 : Part {
	Cost PcbPrint 200 ;
	Cost PCBComponents 600 ;
}
```