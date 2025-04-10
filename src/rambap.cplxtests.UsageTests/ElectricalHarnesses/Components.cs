using rambap.cplx.Modules.Connectivity.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.UsageTests.ElectricalHarnesses;

class TestLead_4mm : Part, IPartConnectable
{
    public WireablePort SolderPoint;
    public ConnectablePort TestLead4mmMale;

    Cost Buy = 10;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.StructuralConnection(TestLead4mmMale, SolderPoint);
    }
}

class SubD9Connector_M : Connector<Size24pin>
{
    public SubD9Connector_M() : base(9) { }
}
class Size24pin : Pin { }

class SubD_Backshell : Part { }