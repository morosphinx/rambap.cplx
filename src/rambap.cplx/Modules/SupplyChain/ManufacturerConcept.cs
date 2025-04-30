using rambap.cplx.Core;
using rambap.cplx.Modules.SupplyChain.WorldModel;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Modules.SupplyChain;

public class InstanceManufacturerInformation : IInstanceConceptProperty
{
    public Entity? Company { get; set; }
}

internal class ManufacturerConcept : IConcept<InstanceManufacturerInformation>
{
    public override InstanceManufacturerInformation? Make(Pinstance i, IEnumerable<Component> subcomponents, Part template)
    {
        // TODO : Handle case were multiples manufacturer are declared. Rigth now, multiples manufacturer override each other
        Manufacturer? manufacturer = null;
        ScanObjectContentFor<Manufacturer>(template,
            (t, i) => i.Name, // Call the implicit Manufacturer(string name) constructor
            (man, s) => manufacturer = man
        );

        return new InstanceManufacturerInformation
        {
            Company = manufacturer?.Company,
        };
    }
}

