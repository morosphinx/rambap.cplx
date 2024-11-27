namespace rambap.cplx.PartProperties;

/// <summary>
/// Common task categories <br/>
/// These are mostly for convenience, any string can be a category
/// </summary>
public enum TaskCategory
{
    // Commerce and supply
    Delivery,
    Quotation,
    Ordering,
    Negotiation,
    SupplierSearch,

    // Management 
    Meetings,
    ProjectManagement,

    // Quality
    Quality,
    Review,
    Validation,
    
    // System Architecture
    SystemArchitecture,
    TechnicalDocumentation,
    Retroengineering,

    // Technical Domains
    Electrical,
    Electronics,
    Mechancial,
    Software,
}

/// <summary>
/// A workload that need to be executed once, no matter the amount of produced unit. <br/>
/// </summary>
/// <param name="Duration_day">Duration, in day, of the task.</param>
public record NonRecurrentTask(decimal Duration_day, string Category = "")
{
    public static implicit operator NonRecurrentTask(decimal duration_day) => new NonRecurrentTask(duration_day);
    public static implicit operator NonRecurrentTask(double duration_day) => new NonRecurrentTask((decimal)duration_day);
    public static implicit operator NonRecurrentTask(int duration_day) => new NonRecurrentTask((decimal)duration_day);
    public NonRecurrentTask(decimal duration_day, TaskCategory taskCategory) : this(duration_day, taskCategory.ToString()) { }
    public NonRecurrentTask(double duration_day, TaskCategory taskCategory) : this((decimal)duration_day, taskCategory.ToString()) { }
}

/// <summary>
/// A workload that need to be executed for the production or supply of each unit. <br/>
/// </summary>
/// <param name="Duration_day">Duration, in day, of the task.</param>
public record RecurrentTask(decimal Duration_day, string Category = "")
{
    public static implicit operator RecurrentTask(decimal duration_day)=> new RecurrentTask(duration_day);
    public static implicit operator RecurrentTask(double duration_day)=> new RecurrentTask((decimal) duration_day);
    public static implicit operator RecurrentTask(int duration_day) => new RecurrentTask((decimal) duration_day);
    public RecurrentTask(decimal duration_day, TaskCategory taskCategory) : this(duration_day, taskCategory.ToString()) { }
    public RecurrentTask(double duration_day, TaskCategory taskCategory) : this((decimal)duration_day, taskCategory.ToString()) { }
}
