namespace rambap.cplx.Export.Iterators;

public record RecursionLocation()
{
    public required string CIN { get; init; }
    public required int Depth { get; init; }
    public required int ComponentIndex { get; init; }
    public required int ComponentCount { get; init; }
}


