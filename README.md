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
CN          	PN               	Component description    	Part description       
 *          	Server Assembly  	                         	                       
 ├─ CAB     	Server Cabinet36U	                         	19' Rack cabinet 36U   
 ├─ R01     	Rack Type A      	Main workload server     	                       
 │  ├─ GPU01	RTX999           	                         	9nth Gen GPU ; 9Go DRAM
 │  └─ GPU02	RTX999           	                         	9nth Gen GPU ; 9Go DRAM
 ├─ R02     	Rack Type A      	Redundant workload server	                       
 │  ├─ GPU01	RTX999           	                         	9nth Gen GPU ; 9Go DRAM
 │  └─ GPU02	RTX999           	                         	9nth Gen GPU ; 9Go DRAM
 └─ R03     	Rack Type B      	Monitoring server        	                       
    └─ GPU  	RTX999           	                         	9nth Gen GPU ; 9Go DRAM
```

- Bill of materials

|#|PN               |Description            |Cost Name|Unit Cost|Count|Total Cost|
|-|-----------------|-----------------------|---------|--------:|----:|---------:|
|1|Server Assembly  |                       |Cables   |   500.00|    1|    500.00|
|1|Server Assembly  |                       |Switches |  1000.00|    1|   1000.00|
|2|Server Cabinet36U|19' Rack cabinet 36U   |Buy      |  2000.00|    1|   2000.00|
|3|Rack Type A      |                       |Other    |  4500.00|    2|   9000.00|
|3|Rack Type A      |                       |Assembly |   400.00|    2|    800.00|
|4|RTX999           |9nth Gen GPU ; 9Go DRAM|Buy      |   999.00|    5|   4995.00|
|5|Rack Type B      |                       |Other    |  3000.00|    1|   3000.00|
|-|-----------------|-----------------------|---------|--------:|----:|---------:|
| |TOTAL            |                       |         |         |     |  21295.00|

- Cost Breakdown

|CN                 |CID      |PN               |Cost Name     |Unit Cost|Count|Total Cost|
|-------------------|---------|-----------------|--------------|--------:|----:|---------:|
| *                 |/*       |Server Assembly  |total per unit| 21295.00|    1|          |
| ├─ /              |/*       |Server Assembly  |Cables        |   500.00|    1|    500.00|
| ├─ /              |/*       |Server Assembly  |Switches      |  1000.00|    1|   1000.00|
| ├─ CAB            |CAB      |Server Cabinet36U|total per unit|  2000.00|    1|          |
| │  └─ /           |CAB      |Server Cabinet36U|Buy           |  2000.00|    1|   2000.00|
| ├─ 2x: Rack Type A|R01      |Rack Type A      |total per unit|  6898.00|    2|          |
| │  ├─ /           |R01      |Rack Type A      |Other         |  4500.00|    2|   9000.00|
| │  ├─ /           |R01      |Rack Type A      |Assembly      |   400.00|    2|    800.00|
| │  └─ 2x: RTX999  |R01/GPU01|RTX999           |total per unit|   999.00|    4|          |
| │     └─ /        |R01/GPU01|RTX999           |Buy           |   999.00|    4|   3996.00|
| └─ R03            |R03      |Rack Type B      |total per unit|  3999.00|    1|          |
|    ├─ /           |R03      |Rack Type B      |Other         |  3000.00|    1|   3000.00|
|    └─ GPU         |R03/GPU  |RTX999           |total per unit|   999.00|    1|          |
|       └─ /        |R03/GPU  |RTX999           |Buy           |   999.00|    1|    999.00|
|-------------------|---------|-----------------|--------------|--------:|----:|---------:|
|                   |TOTAL    |TOTAL            |              |         |     |  21295.00|

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