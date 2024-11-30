## How to use
### 1 - Define a CPLX Part :
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

### 2 - Run calculations :
- With a Part, construct an [Instance](../src/rambap.cplx/Core/Pinstance)
``` Csharp
var p = new BENCH_002();
var i = new Pinstance(p);
```

### 3 - Export documentation :
- Define an [IGenerator](../src/rambap.cplx/Export/IGenerator) or use one at rambap.cplx.Export.Generators
- Run its Do() method

``` Csharp
var generator = new cplx.Export.Generators.ConfigureGenerator(Generators.Content.Costing, Generators.HierarchyMode.Flat);
generator.Do(i, "C:\\DestinationFolder");
```