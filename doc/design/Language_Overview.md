# cplx Language Overview

cplx is an extensible C# library that model hardware Part and system as code, and produce documentation for those systems.

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

[^1]:_cplxconfig.cs contains C# analyzer configuration and global #using directives needed by most Part classes for a smooth editing experience. _cplxconfig.cs is included in templates package [rambap.cplx.Templates](https://www.nuget.org/packages/rambap.cplx.Templates/)

## The Model at Code / Compile Time
### Parts

Part are defined by C# classes inheriting from the Part class


``` Csharp
// This define a part of PN 'BENCH_001'
class BENCH_001 : Part {}
```


### Components

Components are defined by adding fields or properties to a part

``` Csharp
class BENCH_001 : Part {
	RACK_597 C01 ; // This defined a component of CN 'C01', an instance of the part PN 'RACK_597' 
	RACK_597 C02 ; // This defined a component of CN 'C02', an instance of the part PN 'RACK_597' 
}
```

TODO : Alternatives way to declare components :
- Additional components
- List

### Properties

Properties are defined by adding fields or properties to a part

``` Csharp
class BENCH_001 : Part {
	Cost PowerSupply = 5000 ; // A cplx roperty
	Cost Assembly = 8000 ; // A cplx roperty
	Mass_kg Mass = 80 ; // A cplx roperty
}
```

TODO : Alternatives way to declare properties :
- Additional properties
- List

cplx Properties are defined in the rambap.cplx.PartProperties namespace.

The way properties are used when instantiating the part is defined in [DataConcepts](./DataModel_TheConceptsModules.md)

## The Model at Runtime

### Pinstance

### Pinstance properties
