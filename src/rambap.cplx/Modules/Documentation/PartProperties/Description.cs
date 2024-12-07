namespace rambap.cplx.PartProperties;

/// <summary>
/// Documentation. Text
/// </summary>
/// <param name="text"></param>
public class Description(string text)
{
    public static implicit operator Description(string text) => new Description(text);
    public string Text => text;
}

