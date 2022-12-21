namespace CoreMeridian.Models;

public sealed record ItemAward
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Count { get; init; }
    public int Cost { get; init; }
    public int Stack { get; init; }
    public string Octet { get; init; }
    public int Proctype { get; init; }
    public int Mask { get; init; }
}