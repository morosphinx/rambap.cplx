using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.PartInterfaces;

public interface IPartMechanical
{
    // TODO : Raise warning on analyser if implemented without implementing the interface

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Slot"></param>
    public void Assembly_MechanicalSlots(SlottingBuilder Do);
        // Override with Slotting behavior


    public class SlottingBuilder
    {
        internal HashSet<MechanicalReceptacle> InstructedReceptacles = new();

        // By using a method with an helper, we force the user to go through this method,
        // Can store the data wherever for futre refenrece, ensure user does not touche it
        // outside of Assembly_MechanicalSlots() method,
        // and that it is called in a valdi ocntext (after component & property init init)
        public void In(MechanicalReceptacle receptacle, Part module, int position)
        {
            receptacle.SlottedParts.Add(new MechanicalReceptacle.SlottingInstruction(module, position));
            InstructedReceptacles.Add(receptacle);
        }

        // Prevent usage from outside
        internal SlottingBuilder() { }
    }
}
