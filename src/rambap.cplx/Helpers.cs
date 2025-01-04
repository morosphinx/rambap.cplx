namespace rambap.cplx;

/// <summary> Extension method to help define parts </summary>
public static class Helpers
{
    /// <summary> Construct a new list of T. Call the class parameterless constructor
    public static List<T> List<T>(int amount)
        where T : class, new()
        => Enumerable.Range(0, amount).Select(i => new T()).ToList();

    /// <summary> Construct a new list of T. Call a parameterless function as constructor </summary>
    public static List<T> List<T>(int amount, Func<T> constructor)
        => Enumerable.Range(0, amount).Select(i => constructor()).ToList();

    /// <summary> Construct a new list of T. Call a function(index) as constructor</summary>
    public static List<T> List<T>(int amount, Func<int,T> constructor)
        => Enumerable.Range(0, amount).Select(i => constructor(i)).ToList();
}

