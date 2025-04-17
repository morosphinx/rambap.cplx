# CPLX
## About
CPLX is a C# internal [domain specific language](https://en.wikipedia.org/wiki/Domain-specific_language) to define and document complex, hierarchical hardware systems.

CPLX definitions are source-controlable. CPLX calculates aggregate properties of systems, and generates documentation.

## An exemple
### Inputs
By writing the following definition :
``` Csharp
class ServerAssembly : Part {
    // Components
    ServerCabinet36U CAB ;

    [ComponentDescription("Main workload server")]
    RackTypeA R01 ;

    [ComponentDescription("Redundant workload server")]
    RackTypeA R02 ;

    [ComponentDescription("Monitoring server")]
    RackTypeB R03 ;

    // Self Costs
    Cost Cables = 500 ;
    Cost Switches = 1000 ;
}

[PartDescription("19' Rack cabinet 36U")]
class ServerCabinet36U : Part {
    Cost Buy = 2000 ;
}

class RackTypeA : Part {
    RTX999 GPU01;
    RTX999 GPU02 ;
    Cost Other = 4500;
    Cost Assembly = 400;
}

class RackTypeB : Part {
    RTX999 GPU;
    Cost Other = 3000 ;
}

[PartDescription("9nth Gen GPU ; 9Go DRAM")]
class RTX999 : Part {
    Cost Buy = 999 ;
}
```


### Outputs
CPLX generate files assisting multiples aspects of the design process :

- Component Tree

```
CN          	PN              	Component description    	Part Description       
 *          	ServerAssembly  	ROOT COMPONENT           	                       
 ├─ CAB     	ServerCabinet36U	                         	19' Rack cabinet 36U   
 ├─ R01     	RackTypeA       	Main workload server     	                       
 │  ├─ GPU01	RTX999          	                         	9nth Gen GPU ; 9Go DRAM
 │  └─ GPU02	RTX999          	                         	9nth Gen GPU ; 9Go DRAM
 ├─ R02     	RackTypeA       	Redundant workload server	                       
 │  ├─ GPU01	RTX999          	                         	9nth Gen GPU ; 9Go DRAM
 │  └─ GPU02	RTX999          	                         	9nth Gen GPU ; 9Go DRAM
 └─ R03     	RackTypeB       	Monitoring server        	                       
    └─ GPU  	RTX999          	                         	9nth Gen GPU ; 9Go DRAM
```

- Bill of materials

| #|PN              |Part Description       |Cost Name|Unit Cost|Count|Total Cost|
|-:|----------------|-----------------------|---------|--------:|----:|---------:|
| 1|ServerAssembly  |                       |Cables   |   500.00|    1|    500.00|
| 1|ServerAssembly  |                       |Switches |  1000.00|    1|   1000.00|
| 2|ServerCabinet36U|19' Rack cabinet 36U   |Buy      |  2000.00|    1|   2000.00|
| 3|RackTypeA       |                       |Other    |  4500.00|    2|   9000.00|
| 3|RackTypeA       |                       |Assembly |   400.00|    2|    800.00|
| 4|RTX999          |9nth Gen GPU ; 9Go DRAM|Buy      |   999.00|    5|   4995.00|
| 5|RackTypeB       |                       |Other    |  3000.00|    1|   3000.00|
|-:|----------------|-----------------------|---------|--------:|----:|---------:|
|  |TOTAL           |                       |         |         |     |  21295.00|

- Cost Breakdown

|CN                     |SumCost        |CID      |PN              |Cost Name|Unit Cost|Count|Total Cost|
|-----------------------|---------------|---------|----------------|---------|--------:|----:|---------:|
| *                     | 21295.00      |/*       |ServerAssembly  |         |         |     |          |
| ├─ / Cables           | ├─ 500.00     |/*       |ServerAssembly  |Cables   |   500.00|    1|    500.00|
| ├─ / Switches         | ├─ 1000.00    |/*       |ServerAssembly  |Switches |  1000.00|    1|   1000.00|
| ├─ CAB / Buy          | ├─ 2000.00    |CAB      |ServerCabinet36U|Buy      |  2000.00|    1|   2000.00|
| ├─ 2x: RackTypeA      | ├─ 2x: 6898.00|R01      |RackTypeA       |         |         |     |          |
| │  ├─ / Other         | │  ├─ 4500.00 |R01      |RackTypeA       |Other    |  4500.00|    2|   9000.00|
| │  ├─ / Assembly      | │  ├─ 400.00  |R01      |RackTypeA       |Assembly |   400.00|    2|    800.00|
| │  └─ 2x: RTX999 / Buy| │  └─ 999.00  |R01/GPU01|RTX999          |Buy      |   999.00|    4|   3996.00|
| └─ R03                | └─ 3999.00    |R03      |RackTypeB       |         |         |     |          |
|    ├─ / Other         |    ├─ 3000.00 |R03      |RackTypeB       |Other    |  3000.00|    1|   3000.00|
|    └─ GPU / Buy       |    └─ 999.00  |R03/GPU  |RTX999          |Buy      |   999.00|    1|    999.00|
|-----------------------|---------------|---------|----------------|---------|--------:|----:|---------:|
|                       |               |TOTAL    |TOTAL           |         |         |     |  21295.00|

Ouputs are fully configurable. 

CPLX is build with extensibility in mind, and can document more concepts and physical properties as the design get refined.

## Getting started
Prerequisites : Use either [Visual Studio](https://visualstudio.microsoft.com/) or [VisualStudio Code](https://code.visualstudio.com/) IDE with C# support installed. 

1 - Install the [rambap.cplx.Templates](https://www.nuget.org/packages/rambap.cplx.Templates/) template package. To do so in a console :
```
dotnet new install rambap.cplx.Templates
```
2 - Create a new project using the <i>cplx.Templates.Exe</i> template you just installed, either through your IDE or with :
```
dotnet new cplxExecutable --name MyProjectName
```
3 - Edit the MyPart.cs file

4 - Run the project

## Documentation

[On github](https://github.com/morosphinx/rambap.cplx/tree/main/doc)
