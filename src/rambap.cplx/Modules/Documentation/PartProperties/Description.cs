#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Documentation. Text
/// </summary>
/// <param name="text"></param>
public class Description(string text)
{
    public static implicit operator Description(string text) => new Description(text);
    public string Text => text;
}

