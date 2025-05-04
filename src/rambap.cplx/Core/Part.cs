using rambap.cplx.Attributes;

namespace rambap.cplx.Core;

/// <summary>
/// Definition of a system component
/// </summary>
/// <remarks>
/// To define a system, subtype this class, and compose other Parts or properties into it.<br/>
/// Parts are used to construct Components <see cref="Core.Pinstance">Instances</see>,
/// and doing so calculate global properties of the system<br/>
/// When composing a part into another :<br/>
/// - ComponentNumber (CN) is the field or property name<br/>
/// - Null Part that have a parameterless constructor don't need a value<br/>
/// - IEnumerable&lt;PartType&gt; Can be used
/// <code>
/// public class Wheel : Part {
///     Cost Price = 20;
/// }
/// public class BikeFrame : Part {
///     Cost Price = 150;
/// }
/// public class CoolFlag : Part {
///     Cost Price = 5;
/// }
/// public class Bike : Part {
///     Wheel FrontWheel ;
///     Wheel BackWheel ;
///     BikeFrame Frame ;
///     List&lt;CoolFlag&gt; Decorations = List(10, () => new CoolFlag());
///     
///     // Instance(Part) total calculated cost will be 20 + 20 + 150 + 5*10 = 240 €
/// }
/// </code>
/// </remarks>
public partial class Part
{
    /// <summary>
    /// Part Number. Uniquely identify this Part definition <br/>
    /// Two Parts with any distinct characteristic must have a different PN.
    /// </summary>
    /// <remarks>
    /// By default, PN is equal to the class name <br/>
    /// This can be overwritten by the <see cref="PNAttribute"/>
    /// </remarks>
    public string PN
    {
        init => PNOverride = value ;
        get => PNOverride ?? DefaultPNifNoOverride;
    }
    internal string? PNOverride { get; set; } = null;

    /// <summary>
    /// Get classname to make a default PN
    /// Many PN start with a number, but C# don't allow classname to.
    /// Set "_" in front of the classname as a workaround
    /// </summary>
    private string DefaultPNifNoOverride => this.GetType().Name.TrimStart('_');

    /// <summary>
    /// Description of the usage of this part, when considered as a component in another part
    /// </summary>
    /// <remarks>
    /// Additional description can be added by the <see cref="ComponentDescriptionAttribute"/>
    /// </remarks>
    public string ComponentDescription { get; init; } = "";

    /// <summary>
    /// Hardware revision of this part.
    /// </summary>
    /// <remarks>
    /// Can be overwritten by the <see cref="RevisionAttribute"/>
    /// </remarks>
    public string Revision { get; init; } = "";

    /// <summary>
    /// CommonName of this part
    /// </summary>
    /// <remarks>
    /// Can be overwritten by the <see cref="CommonNameAttribute"/>
    /// </remarks>
    public string CommonName { get; init; } = "";

    /// <summary>
    /// Overide of the default CN of this part when used as a component <br/>
    /// This take priority over <see cref="RenameAttribute"/>
    /// </summary>
    public string? CN { init => CNOverride = value; }
    internal string? CNOverride { get; private set; } = null;

    /// <summary>
    /// TODO : Usage ?
    /// TBD : Parent of this part in the part tree.
    /// Imply Part class instance unicity
    /// </summary>
    [CplxIgnore]
    internal Part? Parent  = null;

    /// <summary>
    /// Component that implement this part.
    /// Required during Instante Property construction by some concepts,
    /// Such as ones that declare relationship PartProperties, taking Part as Parameters
    /// </summary>
    [CplxIgnore]
    internal Component? ImplementingComponent { get; set; }

    /// <summary>
    /// Data dynamicaly filled by concepts for any purposes, before Pinstance construction<br/>
    /// </summary>
    [CplxIgnore]
    private List<object> ConceptInitialisationData { get; } = new();

    /// <summary>
    /// Access a typed item T of the part internal <see cref="ConceptInitialisationData"/>
    /// </summary>
    internal T GetContceptInitialisationData<T>()
        where T : new()
    {
        if(! ConceptInitialisationData.OfType<T>().Any())
            ConceptInitialisationData.Add(new T());
        return ConceptInitialisationData.OfType<T>().First();
    }

    /// <summary>
    /// You can override the Part() constructor to implement some custom logic. <br/>
    /// Otherwise keep a parameterless constructor.
    /// </summary>
    /// Part constructor is public to be able to use it as a convenient placeholder
    public Part()
    {
    }

    /// <summary>
    /// Run the cplx calculation on this Part <br/>
    /// This can only be done once per Part.
    /// </summary>
    /// <returns>A component instance, that can be inspected and used to generate documentation</returns>
    public Component Instantiate()
        => Instantiate(new AlternativesConfiguration());

    /// <summary>
    /// <inheritdoc cref="Part.Instantiate()"/> <br/>
    /// </summary>
    /// <param name="partConfiguration">Specify how <see cref="Alternatives{T}"/> parts should be chosen</param>
    /// <returns><inheritdoc cref="Part.Instantiate()"/></returns>
    public Component Instantiate(AlternativesConfiguration partConfiguration)
        => new Component(this, partConfiguration)
        {
            CN = "*",
            Comment = $"ROOT COMPONENT",
            IsPublic = true,
        };
}