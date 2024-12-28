using rambap.cplx.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public enum MechanicalSlotType { UserDefined, RackUnit };

public class MechanicalReceptacle : IPartProperty
{
    public MechanicalSlotType Type { get; init; }
    public int SlotAmount { get; init; }

    public static implicit operator MechanicalReceptacle((MechanicalSlotType type, int slotAmount) t)
        => new MechanicalReceptacle() { Type = t.type, SlotAmount = t.slotAmount };


    internal record SlottingInstruction(Part part, int Position);
    internal List<SlottingInstruction> SlottedParts { get; } = [];
}

public class MechanicalModule : IPartProperty
{
    public MechanicalSlotType Type { get; init; }
    public int SlotSize { get; init; }

    public static implicit operator MechanicalModule((MechanicalSlotType type, int slotSize) t)
        => new MechanicalModule() { Type = t.type, SlotSize = t.slotSize };
}