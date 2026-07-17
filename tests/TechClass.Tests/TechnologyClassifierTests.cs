using TechClassificationApp;
using Xunit;

namespace TechClass.Tests;

public sealed class TechnologyClassifierTests
{
    [Theory]
    [InlineData("[]", true)]
    [InlineData("[{\"name\":\"AEC\"}]", true)]
    [InlineData("{}", false)]
    [InlineData("[", false)]
    [InlineData("", false)]
    public void IsValidJsonArray_RecognizesOnlyCompleteArrays(string json, bool expected)
    {
        Assert.Equal(expected, TechnologyClassifier.IsValidJsonArray(json));
    }

    [Fact]
    public void ExtractJson_FindsArrayWrappedInModelText()
    {
        const string response = "Here are the records:\n```json\n[{\"tech_type\":\"AEC\"}]\n```";

        var json = TechnologyClassifier.ExtractJson(response);

        Assert.Equal("[{\"tech_type\":\"AEC\"}]", json);
    }

    [Fact]
    public void ExtractJson_SkipsIncompleteAttemptAndUsesRestartedArray()
    {
        const string response = "[{\"broken\": true}\n```json\n[{\"tech_type\":\"PEM\"}]";

        var json = TechnologyClassifier.ExtractJson(response);

        Assert.Equal("[{\"tech_type\":\"PEM\"}]", json);
    }

    [Fact]
    public void ExtractJson_ReturnsEmptyForInvalidResponse()
    {
        Assert.Equal(string.Empty, TechnologyClassifier.ExtractJson("No structured result"));
    }

    [Fact]
    public void ParseRowsFromJson_ConvertsScalarAndArrayValues()
    {
        const string json = "[{\"year\":2035,\"active\":true,\"carriers\":[\"H2\",\"O2\"],\"missing\":null}]";

        var rows = TechnologyClassifier.ParseRowsFromJson(json);

        var row = Assert.Single(rows);
        Assert.Equal("2035", row["year"]);
        Assert.Equal("true", row["active"]);
        Assert.Equal("H2, O2", row["carriers"]);
        Assert.False(row.ContainsKey("missing"));
    }

    [Fact]
    public void ParseRecord_NormalizesHeadersAndEuropeanNumbers()
    {
        var row = new Dictionary<string, string>
        {
            ["Tech Type"] = "Alkaline electrolysis",
            ["base_year"] = "2,035",
            ["Reference Unit Size"] = "1.500 MW",
            ["overall_efficiency"] = "0,650",
            ["capex"] = "1.234,5 EUR/kW",
            ["currency"] = "EUR"
        };

        var record = TechnologyClassifier.ParseRecord(row, out var errors);

        Assert.Empty(errors);
        Assert.Equal("Alkaline electrolysis", record.TechType);
        Assert.Equal(2035, record.Year);
        Assert.Equal(1500, record.ReferenceUnitSize);
        Assert.Equal(0.65, record.OverallEfficiency);
        Assert.Equal(1234.5m, record.Capex);
    }

    [Fact]
    public void ParseRecord_ReportsInvalidNumbersWithoutThrowing()
    {
        var row = new Dictionary<string, string>
        {
            ["year"] = "unknown",
            ["capex"] = "not available"
        };

        var record = TechnologyClassifier.ParseRecord(row, out var errors);

        Assert.Null(record.Year);
        Assert.Null(record.Capex);
        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, error => error.StartsWith("year:"));
        Assert.Contains(errors, error => error.StartsWith("capex:"));
    }

    [Theory]
    [InlineData("Electrolysis", "ELE")]
    [InlineData("Fischer-Tropsch synthesis", "FTS")]
    [InlineData("", "UNK")]
    public void ExtractAbbreviation_ProducesStableShortNames(string input, string expected)
    {
        Assert.Equal(expected, TechnologyClassifier.ExtractAbbreviation(input));
    }

    [Fact]
    public void GenerateTechId_UsesTechnologyFieldsAndAvoidsCollisions()
    {
        var record = new TechnologyRecord
        {
            MainInput = "Carbon dioxide",
            UnitOperation = "Fischer-Tropsch synthesis",
            ProcessType = "Conversion",
            MainOut = "Synthetic fuel"
        };
        var usedIds = new HashSet<string> { "CD_FTS_CON_SF" };

        var id = TechnologyClassifier.GenerateTechId(record, usedIds);

        Assert.Equal("CD_FTS_CON_SF_2", id);
    }

    [Fact]
    public void HasMeaningfulData_RequiresAtLeastTwoPopulatedFields()
    {
        Assert.False(TechnologyClassifier.HasMeaningfulData(new TechnologyRecord { Year = 2050 }));
        Assert.True(TechnologyClassifier.HasMeaningfulData(new TechnologyRecord
        {
            Year = 2050,
            MainSector = "Fuels"
        }));
    }

    [Fact]
    public void MergeByTechnologyAndYear_FillsMissingFieldsForSameDataPoint()
    {
        var rows = new[]
        {
            new TechnologyRecord { TechType = "AEC", Year = 2035, Description = "First" },
            new TechnologyRecord { TechType = "AEC", Year = 2035, MainSector = "Hydrogen" }
        };

        var merged = TechnologyClassifier.MergeByTechnologyAndYear(rows);

        var record = Assert.Single(merged);
        Assert.Equal("First", record.Description);
        Assert.Equal("Hydrogen", record.MainSector);
    }

    [Fact]
    public void MergeByTechnologyAndYear_DoesNotMergeRowsWithoutDataYear()
    {
        var rows = new[]
        {
            new TechnologyRecord { TechType = "AEC", Description = "First" },
            new TechnologyRecord { TechType = "AEC", Description = "Second" }
        };

        Assert.Equal(2, TechnologyClassifier.MergeByTechnologyAndYear(rows).Count);
    }
}
