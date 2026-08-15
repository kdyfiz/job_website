using JobScout.Domain.Enums;
using JobScout.Infrastructure.Matching;

namespace JobScout.UnitTests;

public class LocationMatcherTests
{
    [Fact]
    public void Selangor_matches_petaling_jaya()
    {
        Assert.True(LocationMatcher.Matches("Petaling Jaya, Malaysia", "Selangor", WorkArrangement.Hybrid));
    }

    [Fact]
    public void Kuala_lumpur_does_not_match_other_states()
    {
        Assert.True(LocationMatcher.Matches("Kuala Lumpur, Malaysia", "Kuala Lumpur", WorkArrangement.Hybrid));
        Assert.False(LocationMatcher.Matches("Ipoh, Malaysia", "Kuala Lumpur", WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("Kota Kinabalu, Malaysia", "Kuala Lumpur", WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("Penang, Malaysia", "Kuala Lumpur", WorkArrangement.Hybrid));
        Assert.False(LocationMatcher.Matches("Malaysia (Remote)", "Kuala Lumpur", WorkArrangement.OnSite));
        Assert.False(LocationMatcher.Matches("Malaysia", "Kuala Lumpur", WorkArrangement.OnSite));
    }

    [Fact]
    public void Multiple_states_match_any_selected()
    {
        Assert.True(LocationMatcher.Matches("Penang, Malaysia", "Selangor, Penang", WorkArrangement.OnSite));
        Assert.True(LocationMatcher.Matches("Petaling Jaya, Malaysia", "Selangor, Penang", WorkArrangement.Hybrid));
        Assert.False(LocationMatcher.Matches("Ipoh, Malaysia", "Selangor, Penang", WorkArrangement.OnSite));
    }

    [Fact]
    public void Remote_job_still_must_be_in_selected_state()
    {
        Assert.True(LocationMatcher.Matches("Kuala Lumpur, Malaysia", "Kuala Lumpur", WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("Ipoh, Malaysia", "Kuala Lumpur", WorkArrangement.Remote));
    }

    [Fact]
    public void Empty_or_malaysia_keeps_malaysian_jobs_only()
    {
        Assert.True(LocationMatcher.Matches("Kuching, Malaysia", null, WorkArrangement.OnSite));
        Assert.True(LocationMatcher.Matches("Ipoh, Malaysia", "", WorkArrangement.Remote));
        Assert.True(LocationMatcher.Matches("Malaysia (Remote)", "Malaysia", WorkArrangement.Remote));
        Assert.True(LocationMatcher.Matches("Malaysia (Remote)", "Kuala Lumpur", WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("Remote", null, WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("United States (Remote)", "", WorkArrangement.Remote));
        Assert.False(LocationMatcher.Matches("Berlin, Germany", "Malaysia", WorkArrangement.OnSite));
        Assert.False(LocationMatcher.Matches("Remote", "Kuala Lumpur", WorkArrangement.Remote));
    }

    [Fact]
    public void Kl_alias_does_not_match_klang()
    {
        Assert.Equal("Selangor", LocationMatcher.ResolveState("Klang, Malaysia"));
        Assert.Equal("Kuala Lumpur", LocationMatcher.ResolveState("KL"));
    }
}
