using System.Text.RegularExpressions;
using JobScout.Domain.Enums;

namespace JobScout.Infrastructure.Matching;

public static class LocationMatcher
{
    private static readonly Dictionary<string, string[]> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Kuala Lumpur"] = ["Kuala Lumpur", "KL"],
        ["Selangor"] = ["Selangor", "Petaling Jaya", "Shah Alam", "Cyberjaya", "Subang Jaya", "Subang", "Klang", "Kajang", "Puchong"],
        ["Penang"] = ["Penang", "Pulau Pinang", "George Town", "Bayan Lepas"],
        ["Johor"] = ["Johor Bahru", "Johor", "Iskandar"],
        ["Putrajaya"] = ["Putrajaya"],
        ["Negeri Sembilan"] = ["Negeri Sembilan", "Seremban"],
        ["Perak"] = ["Perak", "Ipoh"],
        ["Malacca"] = ["Malacca", "Melaka"],
        ["Kedah"] = ["Kedah", "Alor Setar", "Sungai Petani"],
        ["Pahang"] = ["Pahang", "Kuantan"],
        ["Kelantan"] = ["Kelantan", "Kota Bharu"],
        ["Terengganu"] = ["Terengganu", "Kuala Terengganu"],
        ["Perlis"] = ["Perlis", "Kangar"],
        ["Sabah"] = ["Sabah", "Kota Kinabalu"],
        ["Sarawak"] = ["Sarawak", "Kuching"],
        ["Labuan"] = ["Labuan"]
    };

    public static IReadOnlyList<string> Split(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return [];
        }

        return location
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(token => token.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static bool Matches(string jobLocation, string? requested, WorkArrangement? workArrangement)
    {
        _ = workArrangement;
        var selected = SelectedStates(requested);
        if (selected.Count == 0)
        {
            return true;
        }

        var jobState = ResolveState(jobLocation);
        return jobState is not null && selected.Contains(jobState);
    }

    public static double Score(string jobLocation, string? requested, WorkArrangement? workArrangement)
    {
        _ = workArrangement;
        var selected = SelectedStates(requested);
        if (selected.Count == 0)
        {
            return 0.5;
        }

        var jobState = ResolveState(jobLocation);
        return jobState is not null && selected.Contains(jobState) ? 1.0 : 0.2;
    }

    public static string? ResolveState(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        foreach (var state in Aliases.Keys)
        {
            if (state.Equals(text.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return state;
            }
        }

        foreach (var (state, alias) in Aliases
            .SelectMany(pair => pair.Value.Select(alias => (State: pair.Key, Alias: alias)))
            .OrderByDescending(item => item.Alias.Length))
        {
            if (ContainsPlace(text, alias))
            {
                return state;
            }
        }

        return null;
    }

    private static HashSet<string> SelectedStates(string? requested)
    {
        var selected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in Split(requested))
        {
            var state = ResolveState(token);
            if (state is not null)
            {
                selected.Add(state);
            }
        }

        return selected;
    }

    private static bool ContainsPlace(string haystack, string needle)
    {
        if (needle.Length <= 3)
        {
            return Regex.IsMatch(haystack, $@"\b{Regex.Escape(needle)}\b", RegexOptions.IgnoreCase);
        }

        return haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }
}
