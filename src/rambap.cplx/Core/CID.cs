namespace rambap.cplx.Core;

/// <summary>
/// ComponentID (CID) are Path-like strings that reference a component of an instance, or a sub-component, and so on <br/>
/// </summary>
public static class CID
{
    /// <summary>
    /// Separator added between components CN
    /// </summary>
    public static string Separator => "/";

    /// <summary>
    /// Name given to the root component in the tree
    /// </summary>
    public static string ImplicitRoot => "*";

    /// <summary>
    /// Append a component CN to a CID path
    /// </summary>
    public static string Append(string CID, string CN) => CID + Separator + CN;

    /// <summary>
    /// Remove the <see cref="ImplicitRoot"/> From a CID, for display
    /// </summary>
    public static string RemoveImplicitRoot(string CID) => CID.Length > 2 ? CID.Substring(3) : CID;
}
