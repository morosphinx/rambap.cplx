# cplx Quickstart

## 1 - Prerequisites

TODO 
- IDE VSCode / VStudio
- Basic C#, Basic Object Oriented Code. Namespace, Private and public.
- Use Git

## 2 - Instalation

TODO
- Run the command

## 3 - Creating a new project, running it

TODO
- Run the command
- Run the nugget update
- Run the project
- Exploring the default output

## 4 - A first Part



### 4.1 - Define a CPLX Part :
- Declare a new class, and inherit the [Part class](../src/rambap.cplx/Core/Part.cs)
    - Add its Components
    - Add some [part properties](../src/rambap.cplx/PartProperties)
    - Use [attributes](../src/rambap.cplx/PartAttributes)
    - WIP [^1]

[^1]: Define relations by implementing some [interfaces](../src/rambap.cplx/PartInterfaces)

``` Csharp
[PN("5963_123")] // A part attribute
class BENCH_002 : Part {
	Cost Buy 9000 ; // A part property
    Part R01 ; // A component
}
```

Start typing 'Part' to access auto-completion of properties and attributes.

## 5 - Generating Documentation

### 5.2 - Run calculations :
- With a Part, construct an [Instance](../src/rambap.cplx/Core/Pinstance)
``` Csharp
var p = new BENCH_002();
var i = new Pinstance(p);
```

### 5.3 - Export documentation :
- Define an [IGenerator](../src/rambap.cplx/Export/IGenerator) or use one at rambap.cplx.Export.Generators
- Run its Do() method

``` Csharp
var generator = new cplx.Export.Generators.ConfigureGenerator(Generators.Content.Costing, Generators.HierarchyMode.Flat);
generator.Do(i, "C:\\DestinationFolder");
```