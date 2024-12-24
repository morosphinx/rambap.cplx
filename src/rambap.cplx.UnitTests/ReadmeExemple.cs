namespace rambap.cplx.UnitTests;
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