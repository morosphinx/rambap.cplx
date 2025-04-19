using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;
using static rambap.cplx.PartInterfaces.IPartMechanical;

namespace rambap.cplx.Modules.Racking;

public class InstanceMechanicalAssembly : IInstanceConceptProperty
{
    public required List<MechanicalReceptacle> InstructedReceptacles { get; init; }
}

internal class SlotConcept : IConcept<InstanceMechanicalAssembly>
{
    // TODO : cleanup variables names, too much receptacle
    public override InstanceMechanicalAssembly? Make(Pinstance instance, IEnumerable<Component> subcomponents, Part template)
    {
        List<(string name, MechanicalReceptacle receptacle)> receptacles = new();
        ScanObjectContentFor<MechanicalReceptacle>(template,
            (p, i) =>
            {
                receptacles.Add((i.Name, p));
            });

        bool isModule = false;
        int moduleSize = 0;
        ScanObjectContentFor<MechanicalModule>(template,
            (p, s) =>
            {
                isModule = true;
                moduleSize = p.SlotSize;
            });


        // Execute the Assembly/Slotting instructions

        // WAIT, I CAN GIVE THE SLOT INSTRUCTION, HERE, INFORMATION ABOUT THE INSTANCE BEGIN PROCESSED
        // USE HITS to fetch instance info on part ?
        // we guarantee that instance childs are good at this points
        // ... we also guarantee that 1 part = 1 instance
        // ... add part to instance relation ?

        var tbdslottinginfo = new SlottingBuilder();
        if (template is IPartMechanical a)
            a.Assembly_MechanicalSlots(tbdslottinginfo);

        //TBD: FAIL ?
        /* foreach(var r in receptacles)
         {
             foreach(var module in r.receptacle.SlottedParts)
             {
                 var component = LookForComponent(instance,module.part);
                 if (component == null) throw new InvalidOperationException("Module is not a component or subcomponent of the part");
             }
         }*/
        // TODO : What do we need for generating an image of the solltings ?
        // Information about the owner of the receptacle would be nice, but not required
        // Receptable type, size, and other info are
        // We need info about witch INSTANCE are slotting into this
        // We can assume that part in the instruction are part of the object tree, and
        // probably in a shallow level at that ... but not enforced by the system, for
        // exemple a part may be passed as a constructor of another, deeper part


        // If not module size is specified, have a way for the part to declare
        // itslf as a module if it is composed of module (ooooor ... another receptacle?)
        return new InstanceMechanicalAssembly() { InstructedReceptacles = tbdslottinginfo.InstructedReceptacles.ToList() };
        throw new NotImplementedException();
    }
}