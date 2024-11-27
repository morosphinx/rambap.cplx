using rambap.cplx.Core;

namespace rambap.cplx.Export.Columns;

public class LineNumberColumn<T> : IColumn<T>
{
    public string Title => "#";
    public ColumnTypeHint TypeHint => ColumnTypeHint.Numeric;

    int index = 1;
    public string CellFor(T item)
        => index++.ToString();

    public void Reset()
        => index = 1;

    public string TotalFor(Pinstance root)
        => "";
}

public class LineNumberColumnWithContinuation<T> : IColumn<T>
{
    public string Title => "#";
    public ColumnTypeHint TypeHint => ColumnTypeHint.Numeric;

    int index = 0;
    T? previousItem = default;

    public required Func<T?, T, bool> ContinuationCondition { private get; init; }

    public string CellFor(T item)
    {
        bool shouldIncrement = ContinuationCondition(previousItem, item);
        if (shouldIncrement) index += 1;
        previousItem = item;
        return index.ToString();
    }

    public void Reset()
    {
        index = 0;
        previousItem = default;
    }

    public string TotalFor(Pinstance root)
        => "";
}