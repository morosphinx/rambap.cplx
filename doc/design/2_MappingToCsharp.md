## 

At code / compile time, the part tree

At runtime, an instance tree

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

The way properties are used when instantiating the part is defined in [DataConcepts](./3_DataConcepts.md)

## The Model at Runtime

### Pinstance

### Pinstance properties
