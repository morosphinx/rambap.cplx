#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A mass, in kilogram
/// </summary>
/// <param name="mass_kg">mass, in kilogram</param>
public record Mass_kg(decimal mass_kg)
{
    public static implicit operator Mass_kg(decimal mass_kg)=> new (mass_kg);
    public static implicit operator Mass_kg(double mass_kg)=> new ((decimal) mass_kg);
    public static implicit operator Mass_kg(int mass_kg) => new (mass_kg);

    public static Mass_kg operator +(Mass_kg a, Mass_kg b) => a.mass_kg + b.mass_kg;
}

