namespace rambap.cplx.Attributes;

/// <summary>
/// Apply this attribute to a property/field of type <see cref="Part"/> to add description to the component it represent. <br/>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class ComponentDescriptionAttribute : Attribute
{
    public string Title { get; }
    public string Text { get; }

    public ComponentDescriptionAttribute(string text) : this("", text) { }
    public ComponentDescriptionAttribute(string title, string text)
    {
        Title = title;
        Text = text;
    }
}