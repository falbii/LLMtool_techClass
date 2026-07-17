using TechClassificationApp;
using Xunit;

namespace TechClass.Tests;

public sealed class CondensedVerifierTests
{
    [Fact]
    public void ExtractNumbers_HandlesUsAndEuropeanSeparators()
    {
        var numbers = CondensedVerifier.ExtractNumbers("Costs are 1,234.5 EUR and 2.345,6 EUR; efficiency is 0.65.");

        Assert.Contains(1234.5, numbers);
        Assert.Contains(2345.6, numbers);
        Assert.Contains(0.65, numbers);
    }

    [Fact]
    public void ExtractNumbers_PreservesSeparateWhitespaceDelimitedValues()
    {
        var numbers = CondensedVerifier.ExtractNumbers("range: 500 1400");

        Assert.Equal(new[] { 500d, 1400d }, numbers);
    }

    [Fact]
    public void Contains_UsesTightNumericTolerance()
    {
        var index = new List<double> { 0.65, 2035 };

        Assert.True(CondensedVerifier.Contains(index, 0.65));
        Assert.False(CondensedVerifier.Contains(index, 0.651));
    }

    [Fact]
    public void Verify_AcceptsPercentageEquivalentAndFlagsMissingValue()
    {
        var records = new[]
        {
            new TechnologyRecord
            {
                DatapaperTechId = "AEC_2035",
                Year = 2035,
                OverallEfficiency = 0.65,
                Lifetime = 25
            }
        };

        var report = CondensedVerifier.Verify(records, "In 2035, efficiency reaches 65 percent.");

        Assert.Equal(3, report.TotalValues);
        Assert.Equal(2, report.VerifiedValues);
        var finding = Assert.Single(report.Unverified);
        Assert.Equal("lifetime", finding.Field);
        Assert.Equal("AEC_2035", finding.TechId);
    }

    [Fact]
    public void Verify_ReportsOneHundredPercentWhenThereAreNoNumericFields()
    {
        var report = CondensedVerifier.Verify([new TechnologyRecord()], "No numeric data");

        Assert.Equal(0, report.TotalValues);
        Assert.Equal(100, report.VerifiedPercent);
        Assert.Empty(report.Unverified);
    }
}
