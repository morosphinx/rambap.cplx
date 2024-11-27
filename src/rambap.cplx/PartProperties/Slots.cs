using rambap.cplx.Core;

namespace rambap.cplx.PartProperties;

public enum MechanicalSlotType { UserDefined, RackUnit };

public class MechanicalReceptacle : IPartProperty
{
    public MechanicalSlotType Type { get; init; }
    public int SlotAmount { get; init; }

    public static implicit operator MechanicalReceptacle((MechanicalSlotType type, int slotAmount) t)
        => new MechanicalReceptacle() { Type = t.type, SlotAmount = t.slotAmount };


    internal record SlottingInstruction(Part part, int Position);
    internal List<SlottingInstruction> SlottedParts { get; } = new();
}

public class MechanicalModule : IPartProperty
{
    public MechanicalSlotType Type { get; init; }
    public int SlotSize { get; init; }

    public static implicit operator MechanicalModule((MechanicalSlotType type, int slotSize) t)
        => new MechanicalModule() { Type = t.type, SlotSize = t.slotSize };
}