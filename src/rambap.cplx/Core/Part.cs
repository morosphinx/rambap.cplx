using rambap.cplx.PartAttributes;

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
    /// Description of this par
    /// </summary>
    /// <remarks>
    /// Additional description can be added by the <see cref="PartDescriptionAttribute"/>
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
    /// Overide of the default CN of this part when used as a component
    /// </summary>
    public string? CN { init => CNOverride = value; }
    internal string? CNOverride { get; private set; } = null;

    /// <summary>
    /// List of component that can be completed by a part constructor to implement custom logic<br/>
    /// IF YOU USE THIS, YOU NEED TO DEFINE A UNIQUE PN USING THE PN PROPERTY
    /// </summary>
    protected List<Part> AdditionalComponents { get; } = new();

    /// <summary>
    /// If True, this part has been modified in his constructor, and should have a unique PN to reflect this
    /// </summary>
    internal bool RequirePNOverride
        => AdditionalComponents.Count() > 0;

    /// <summary>
    /// TODO : Usage ?
    /// TBD : Parent of this part in the part tree.
    /// Imply Part class instance unicity
    /// </summary>
    [CplxIgnore]
    internal Part? Parent  = null;


    /// <summary>
    /// You can override the Part() constructor to implement some custom logic. <br/>
    /// Otherwise keep a parameterless constructor.
    /// </summary>
    /// Part constructor is public to be able to use it as a convenient placeholder
    public Part()
    {
    }
}