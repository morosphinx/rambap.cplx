# cplx Language Overview

cplx is an extensible C# library that model hardware Part and systems as code, and produce documentation for those systems.

## Workflow

Using cplx involves three main steps
1. Define, in C# code, Parts
    - Do so by inheriting the Part class and adding PartProperties fields to it.
	- Relation between Parts are modeled by implementing a PartInterface
2. Create Component Instances of the Part(s) you want to document
    - Do so by calling new PInstance(yourPart)
	- Under the hood, cplx use C# Reflection to analyse the Part classes and construct a Component tree.
3. Run document generation methods on the PInstance
    - Using either [TBD] Generator / Instructions.

Thoses steps are all configured inside the code, and thus can (and should) be reused, organised and source-controled as code files.

When the modeled design evolves, you can edit the Parts classes, and re-run the generation process to produces updated documents in a breeze. See the recommended [workflow](../tutorial/Workflow.md)


## Setting up

Basic setup is defined in the [Quickstart](../tutorial/Quickstart.md)

At a minimum, you need to have an executable C# projet :
1. That reference the [rambap.cplx.Core](https://www.nuget.org/packages/rambap.cplx.Core/) package
2. That contains cplx's _cplxconfig.cs file[^1]

[^1]:_cplxconfig.cs contains C# analyzer configuration and global #using directives needed by most Part classes for a smooth editing experience. _cplxconfig.cs is included in templates package [rambap.cplx.Templates](https://www.nuget.org/packages/rambap.cplx.Templates/). You will also need to disable warnings CS8618 CS0169 CS0649 from the C# project file.

## The Model at Code / Compile Time
### Parts

Parts are defined by C# classes inheriting from the Part class

``` Csharp
// This define a part of PN 'BENCH_001'
class BENCH_001 : Part {}
```

Parts may inherit from other parts.

``` Csharp
// This define a part of PN 'SPECIAL_02'
// SPECIAL_02 inherit all components and properties of BENCH_002
class SPECIAL_02 : BENCH_002 {}
```

Not all PN are valid C# identifier. The PN can also be defined through attribute :

``` Csharp
// This define a part of PN '156-4786A'
[PN("156-4786A")] // This will be the PN in generated documents
class Connector_156_4786A : Part {} // This will be the identifier in C# code
```

### Components

Components are defined by adding fields or properties to a part[^2]

``` Csharp
class BENCH_001 : Part {
	RACK_597 C01 ; // This defined a component of CN 'C01', an instance of the part PN 'RACK_597' 
	RACK_597 C02 ; // This defined a component of CN 'C02', an instance of the part PN 'RACK_597' 
}
```

[^2]:Can also be abbreviated
``` Csharp
class BENCH_001 : Part {
	RACK_597 C01, C02;
}
```

### Properties

Properties are defined by adding fields or properties to a part

``` Csharp
class BENCH_001 : Part {
	Cost PowerSupply = 5000 ; // A cplx property
	Cost Assembly = 8000 ; // A cplx property
	Mass_kg Mass = 80 ; // A cplx property
}
```

cplx Properties are defined in the rambap.cplx.PartProperties namespace. Use your IDE autocompletion to list its content.

Some properties may be declared public, or autoimplemented with a backing target (using "=>"). See how they are used in [ConceptModules](Language_PerConcept.md) for the meanings and use cases.

``` Csharp
class RACK_751 : Part {
	public ConnectablePort J01 ;
}

class BENCH_003 : Part{
	RACK_751 R_NOM;
	RACK_751 R_RED;
	// We expose some connectable ports, renamed
	public ConnectablePort J_NOMINAL => R01.J01 ;
	public ConnectablePort J_REDUNDANT => R02.J01;
}
```

You may want to name a property with a name that's not a valid C# identifier. Do so with the [Rename("")] attribute.

``` Csharp
class RACK_789 : Part {
	[Rename("12V PSU")] // "12V PSU" will be the property (ConnectablePort) name in generated documents
	public ConnectablePort _12V_PSU ; // "_12V_PSU" will be the identifier in C# code
}
```

## How are properties used

The way properties are used when instantiating the part is defined in ConceptModules. See the existing properties and their syntaxes in [ConceptModules](Language_PerConcept.md).

## The Model at Runtime

### Instantiated Components and Pinstances

### Pinstance properties

## The Export Methods
