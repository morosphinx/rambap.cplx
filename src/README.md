# CPLX
## About
CPLX is a C# internal [domain specific language](https://en.wikipedia.org/wiki/Domain-specific_language) to define and document complex, hierarchical hardware systems.

CPLX definitions are source-controlable. CPLX calculates aggregate properties of systems, and generates documentation.

## An example
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

- Cost Breakdown

|CID      |PN              |Detail  |Cost    |
|---------|----------------|--------|-------:|
|/*       |ServerAssembly  |Cables  |500.00  |
|/*       |ServerAssembly  |Switches|1000.00 |
|CAB      |ServerCabinet36U|Buy     |2000.00 |
|R01      |RackTypeA       |Other   |4500.00 |
|R01      |RackTypeA       |Assembly|400.00  |
|R01/GPU01|RTX999          |Buy     |999.00  |
|R01/GPU02|RTX999          |Buy     |999.00  |
|R02      |RackTypeA       |Other   |4500.00 |
|R02      |RackTypeA       |Assembly|400.00  |
|R02/GPU01|RTX999          |Buy     |999.00  |
|R02/GPU02|RTX999          |Buy     |999.00  |
|R03      |RackTypeB       |Other   |3000.00 |
|R03/GPU  |RTX999          |Buy     |999.00  |
|---------|----------------|--------|-------:|
|TOTAL    |                |        |21295.00|

- Component Tree

```
CN          	PN              	Description              
 *          	ServerAssembly  	                         
 ├─ CAB     	ServerCabinet36U	                         
 ├─ R01     	RackTypeA       	Main workload server     
 │  ├─ GPU01	RTX999          	                         
 │  └─ GPU02	RTX999          	                         
 ├─ R02     	RackTypeA       	Redundant workload server
 │  ├─ GPU01	RTX999          	                         
 │  └─ GPU02	RTX999          	                         
 └─ R03     	RackTypeB       	Monitoring server        
 │  └─ GPU  	RTX999          	                         

```


- Bill of materials, to a depth of 1

|#|PN              |Component IDs|Description         |Count|Detail  |Unit Cost|Total Cost|
|-|----------------|-------------|--------------------|----:|--------|--------:|---------:|
|1|ServerAssembly  |/*           |                    |1    |Cables  |500.00   |500.00    |
|1|ServerAssembly  |/*           |                    |1    |Switches|1000.00  |1000.00   |
|2|ServerCabinet36U|CAB          |19' Rack cabinet 36U|1    |unit    |2000.00  |2000.00   |
|3|RackTypeA       |R01, R02     |                    |2    |unit    |6898.00  |13796.00  |
|4|RackTypeB       |R03          |                    |1    |unit    |3999.00  |3999.00   |

- Bill of materials, fully recursive

|#|PN              |Component IDs                                      |Description            |Count|Detail  |Unit Cost|Total Cost|
|-|----------------|---------------------------------------------------|-----------------------|----:|--------|--------:|---------:|
|1|ServerAssembly  |/*                                                 |                       |1    |Cables  |500.00   |500.00    |
|1|ServerAssembly  |/*                                                 |                       |1    |Switches|1000.00  |1000.00   |
|2|ServerCabinet36U|CAB                                                |19' Rack cabinet 36U   |1    |Buy     |2000.00  |2000.00   |
|3|RackTypeA       |R01, R02                                           |                       |2    |Other   |4500.00  |9000.00   |
|3|RackTypeA       |R01, R02                                           |                       |2    |Assembly|400.00   |800.00    |
|4|RTX999          |R01/GPU01, R01/GPU02, R02/GPU01, R02/GPU02, R03/GPU|9nth Gen GPU ; 9Go DRAM|5    |Buy     |999.00   |4995.00   |
|5|RackTypeB       |R03                                                |                       |1    |Other   |3000.00  |3000.00   |

Ouputs are fully configurable. 

CPLX is build with extensibility in mind, and can document more concepts and physical properties as the design get refined.

## Getting started
Prerequisites : Use either [Visual Studio](https://visualstudio.microsoft.com/) or [VisualStudio Code](https://code.visualstudio.com/) IDE with C# support installed. 

1 - Install the [rambap.cplx.Templates](TODO_NUGGET_LINK) template package. To do so in a console :
```
dotnet new install rambap.cplx.Templates
```
2 - Create a new project using the <i>cplx.Templates.Exe</i> template you just installed, either through your IDE or with :
```
dotnet new cplxExecutable --name MyProjectName
```
3 - Edit the MyPart.cs file

4 - Run the project