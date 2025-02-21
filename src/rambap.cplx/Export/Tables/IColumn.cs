using rambap.cplx.Core;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.Tables;

public enum ColumnTypeHint
{
    /// <summary>
    /// Column display a string whose content must be printed as-is (eg : PN)
    /// </summary>
    StringExact,
    
    /// <summary>
    /// Column display a string whose content may be reformated (eg : textual comment) 
    /// </summary>
    StringFormatable, 

    /// <summary>
    /// Column display a numeric value
    /// </summary>
    Numeric,
}

/// <summary>
/// Define the content, and construction, of a table Column
/// </summary>
public interface IColumn
{
    /// <summary>
    /// Title of the column  in the CSV dile
    /// </summary>
    string Title { get; set; }

    ColumnTypeHint TypeHint { get; }

    /// <summary>
    /// Return text to be written in a line representing total
    /// </summary>
    string TotalFor(Pinstance root);

    /// <summary>
    /// Reset internal column data that may be carried from a line to another
    /// </summary>
    void Reset();
}

public static class IColumnExtensions
{
    /// <summary>
    /// Helper method to set a column title inside of [ ] enumarable column declaration
    /// </summary>
    public static T SetTitle<T>(this T e, string title)
        where T : IColumn
    {
        e.Title = title;
        return e;
    }
}


/// <summary>
/// Define the content, and construction, of a table Column
/// </summary>
public interface IColumn<T> : IColumn
{
    /// <summary>
    /// Return text to be written in each line's cell
    /// </summary>
    string CellFor(T item);
}

/// <summary>
/// Wrapper around <see cref="IColumn"/> to define a Column using a delegate
/// </summary>
public record DelegateColumn<T> : IColumn<T>
{
    public required string Title { get; set; }
    public required ColumnTypeHint TypeHint { get; init; }

    public required Func<T, string?> GetCellValue { get; init; }
    public string CellFor(T item) => GetCellValue(item) ?? "";

    public required Func<Pinstance, string?>? GetTotalValue { get; init; }
    public string TotalFor(Pinstance root) => GetTotalValue?.Invoke(root) ?? "";

    public void Reset() { } // DelegateColumn are Stateless

    [SetsRequiredMembers]
    public DelegateColumn(string title, ColumnTypeHint typeHint, Func<T, string?> getCellValue, Func<Pinstance, string?>? getTotalValue = null)
    {
        Title = title;
        TypeHint = typeHint;
        GetCellValue = getCellValue;
        GetTotalValue = getTotalValue;
    }
}

public record WrapperColumn<T> : IColumn<T>
{
    public required IColumn<T> WrappedColumn { get; init; }
    public string Title
    {
        get => WrappedColumn.Title;
        set => WrappedColumn.Title = value;
    }
    public ColumnTypeHint TypeHint => WrappedColumn.TypeHint;

    public string CellFor(T item)
        => WrappedColumn.CellFor(item);

    public void Reset()
        => WrappedColumn.Reset();

    public string TotalFor(Pinstance root)
        => WrappedColumn.TotalFor(root);
}
