namespace JobScout.Domain.Models;

public sealed class JobMatch
{
    public required Job Job { get; init; }
    public int EstimatedMatchPercent { get; init; }
    public required MatchBreakdown Breakdown { get; init; }
}
