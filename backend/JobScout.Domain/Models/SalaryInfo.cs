namespace JobScout.Domain.Models;

public sealed class SalaryInfo
{
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
    public string? Currency { get; init; }
    public string? Period { get; init; }
    public string? Display { get; init; }
}
