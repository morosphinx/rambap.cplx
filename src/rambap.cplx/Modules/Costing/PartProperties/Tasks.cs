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
/// <param name="Duration_day">Information about the task kind</param>
public record NonRecurrentTask(decimal Duration_day, string Category = "")
{
    public static implicit operator NonRecurrentTask(decimal duration_day) => new (duration_day);
    public static implicit operator NonRecurrentTask(double duration_day) => new ((decimal)duration_day);
    public static implicit operator NonRecurrentTask(int duration_day) => new (duration_day);

    public static implicit operator NonRecurrentTask((decimal duration_day, TaskCategory taskCategory) tuple)
        => new (tuple.duration_day, tuple.taskCategory.ToString());
    public static implicit operator NonRecurrentTask((double duration_day, TaskCategory taskCategory) tuple)
        => new ((decimal)tuple.duration_day, tuple.taskCategory.ToString());
    public static implicit operator NonRecurrentTask((int duration_day, TaskCategory taskCategory) tuple)
        => new (tuple.duration_day, tuple.taskCategory.ToString());
}

/// <summary>
/// A workload that need to be executed for the production or supply of each unit. <br/>
/// </summary>
/// <param name="Duration_day">Duration, in day, of the task.</param>
/// <param name="Category">Information about the task kind</param>
public record RecurrentTask(decimal Duration_day, string Category = "")
{
    public static implicit operator RecurrentTask(decimal duration_day)=> new (duration_day);
    public static implicit operator RecurrentTask(double duration_day)=> new ((decimal) duration_day);
    public static implicit operator RecurrentTask(int duration_day) => new (duration_day);

    public static implicit operator RecurrentTask((decimal duration_day, TaskCategory taskCategory) tuple)
        => new (tuple.duration_day, tuple.taskCategory.ToString());
    public static implicit operator RecurrentTask((double duration_day, TaskCategory taskCategory) tuple)
        => new ((decimal)tuple.duration_day, tuple.taskCategory.ToString());
    public static implicit operator RecurrentTask((int duration_day, TaskCategory taskCategory) tuple)
        => new (tuple.duration_day, tuple.taskCategory.ToString());
}
