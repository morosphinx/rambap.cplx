using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Concepts;

public class InstanceManufacturerInformation : IInstanceConceptProperty
{
    public Company? Company { get; set; }
}

internal class ManufacturerConcept : IConcept<InstanceManufacturerInformation>
{
    public override InstanceManufacturerInformation? Make(Pinstance i, Part template)
    {
        // TODO : Handle case were multiples manufacturer are declared. Rigth now, multiples manufacturer override each other
        Manufacturer? manufacturer = null;
        ScanObjectContentFor<Manufacturer>(template,
            (man, s) => manufacturer = man,
            (t, i) => i.Name); // Call the implicit Manufacturer(string name) constructor

        return new InstanceManufacturerInformation
        {
            Company = manufacturer?.Company,
        };
    }
}

