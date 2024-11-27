namespace rambap.cplx.PartAttributes;

/// <summary>
/// Apply this atttribute to a <see cref="Part"> class to add additional descriptions to it<br/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PartDescriptionAttribute : Attribute
{
    public string Title { get; }
    public string Text { get; }

    public PartDescriptionAttribute(string text) : this("",text){}
    public PartDescriptionAttribute(string title, string text)
    {
        Title = title;
        Text = text;
    }
}