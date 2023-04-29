using System;
using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilders.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact]
    public void ShouldBeExplicitlyCastFromString()
    {
        var queryBuilder = (SqlQueryBuilder)"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", queryBuilder.GetQuery());
        Assert.Empty(queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeExplicitlyCastFromInterpolatedString()
    {
        var queryBuilder = (SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", queryBuilder.GetQuery());
        Assert.Empty(queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", queryBuilder.GetQuery());
        Assert.Empty(queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        // Building SQL query with parameters
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1",
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123 },
            queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeComposable()
    {
        // Composing SQL queries with parameters
        SqlQueryBuilder innerQueryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";
        SqlQueryBuilder outerQueryBuilder =
            $"SELECT * FROM ({innerQueryBuilder}) src WHERE IsValid = {true}";

        Assert.Equal(
            "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2",
            outerQueryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true },
            outerQueryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeComposableRecursively()
    {
        // Composing SQL queries with parameters
        SqlQueryBuilder innerQueryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";
        SqlQueryBuilder middleQueryBuilder =
            $"SELECT * FROM ({innerQueryBuilder}) src WHERE IsValid = {true}";
        SqlQueryBuilder outerQueryBuilder =
            $"SELECT * FROM ({middleQueryBuilder}) src WHERE Amount > {5}";

        Assert.Equal(
            "SELECT * FROM (" +
                "SELECT * FROM (" +
                    "SELECT * FROM Orders WHERE Id = @p1) src " +
                "WHERE IsValid = @p2) src " +
            "WHERE Amount > @p3",
            outerQueryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true, ["p3"] = 5 },
            outerQueryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldSupportConcatenation()
    {
        // Building SQL query with parameters using interpolated strings concatenation
        SqlQueryBuilder queryBuilder =
            $"SELECT * FROM Orders WHERE Id = {123}" + $" OR Id = {234}";
        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1 OR Id = @p2",
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = 234 },
            queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldSupportConcatenationWithAssignment()
    {
        // Building SQL query with parameters using interpolated strings concatenation
        SqlQueryBuilder queryBuilder =
            $"SELECT * FROM Orders WHERE Id = {123}";
        queryBuilder += $" OR Id = {234}";
        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1 OR Id = @p2",
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = 234 },
            queryBuilder.GetParameters());
    }

    [Theory]
    [InlineData(123)]
    [InlineData("abc")]
    public void ShouldReturnCorrectQueryAndParameters(object value)
    {
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {value}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();

        Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = value }, parameters);
    }

    [Fact]
    public void ShouldReturnParameterNamesWithSpecifiedSuffix()
    {
        // Building SQL query with specified parameter name prefix
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters("param");

        Assert.Equal("SELECT * FROM Orders WHERE Id = @param1", query);
        Assert.Equal(new Dictionary<string, object?> { ["param1"] = 123 }, parameters);
    }

    [Fact]
    public void ShouldReuseParametersWithTheSameValue()
    {
        SqlQueryBuilder queryBuilder =
            $"SELECT * FROM Orders WHERE Id = {123} AND IsValid = {true} AND Amount = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1 AND IsValid = @p2 AND Amount = @p1", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true }, parameters);
    }

    [Fact]
    public void ShouldReuseNullParametersWithTheSameValue()
    {
        SqlQueryBuilder queryBuilder =
            $"SELECT * FROM Orders"
            + $" WHERE Id = {123} AND IsValid = {null!} AND Amount = {null!}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1 AND IsValid = @p2 AND Amount = @p2", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = null }, parameters);
    }

    [Fact]
    public void ShouldInlineLiteralsCorrectly()
    {
        // Inline table name as literal string by specifying the "l" format
        var tableName = "Orders";
        SqlQueryBuilder queryBuilder = $"SELECT * FROM {tableName:l} WHERE Id = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();

        Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123 }, parameters);
    }

    [Fact]
    public void ShouldInlineLiteralsAsNestedQueriesCorrectly()
    {
        // Inline table name as literal string by converting it to SqlQueryBuilder
        var tableNameQueryBuilder = (SqlQueryBuilder)"Orders";
        SqlQueryBuilder queryBuilder = $"SELECT * FROM {tableNameQueryBuilder} WHERE Id = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();

        Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123 }, parameters);
    }

    [Fact]
    public void ShouldBeCreatedFromSingleParameterCorrectly()
    {
        // Building SQL query with parameters provided as nested SqlQueryBuilder
        var param = SqlQueryBuilder.FromParameter(123);
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {param}";

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1",
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123 },
            queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldSupportMetadata()
    {
        // Building SQL query with metadata
        var queryBuilder = ((SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}")
            .AddMetadata("DbName", "Db1");

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1",
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123 },
            queryBuilder.GetParameters());
        Assert.Equal(
            new Dictionary<string, object?> { ["DbName"] = "Db1" },
            queryBuilder.Metadata);
    }

    [Fact]
    public void ShouldPropagateAndMergeMetadataOnQueryComposition()
    {
        var innerQueryBuilder =
            ((SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}")
                .AddMetadata(
                    new Dictionary<string, object?>
                    {
                        ["a"] = 1,
                        ["b"] = 2,
                        ["c"] = null,
                    });
        var outerQueryBuilder =
            ((SqlQueryBuilder)$"SELECT * FROM ({innerQueryBuilder}) src WHERE IsValid = {true}")
                .AddMetadata("a", 1)
                .AddMetadata("c", null);
        Assert.Equal(
            "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2",
            outerQueryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true },
            outerQueryBuilder.GetParameters());
        Assert.Equal(
            new Dictionary<string, object?> { ["a"] = 1, ["b"] = 2, ["c"] = null },
            outerQueryBuilder.Metadata);
    }

    [Fact]
    public void ShouldThrowInvalidOperationExceptionOnInconsistentMetadataOnQueryComposition()
    {
        var innerQueryBuilder =
            ((SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}")
            .AddMetadata(
                new Dictionary<string, object?>
                {
                    ["a"] = 1,
                    ["b"] = 2,
                    ["c"] = null,
                });
        Assert.Throws<InvalidOperationException>(() =>
        {
            ((SqlQueryBuilder)$"SELECT * FROM ({innerQueryBuilder}) src WHERE IsValid = {true}")
                .AddMetadata("a", 1)
                .AddMetadata("c", 3);
        });
    }

    [Fact]
    public void ShouldThrowArgumentNullException_WhenConvertedFromNullString()
    {
        Assert.Throws<ArgumentNullException>(() => (SqlQueryBuilder)(string?)null);
    }

    [Fact]
    public void Equals_ShouldCompareByValues()
    {
        SqlQueryBuilder queryBuilder1 =
            $"SELECT * FROM (" +
                $"{(SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}"}) src " +
            $"WHERE IsValid = {true}";
        SqlQueryBuilder queryBuilder2 =
            $"SELECT * FROM (" +
            $"{(SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}"}) src " +
            $"WHERE IsValid = {true}";
        Assert.Equal(queryBuilder1, queryBuilder2);
    }

    [Fact]
    public void EqualityOperator_ShouldCompareByValues()
    {
        SqlQueryBuilder queryBuilder1 =
            $"SELECT * FROM (" +
            $"{(SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}"}) src " +
            $"WHERE IsValid = {true}";
        SqlQueryBuilder queryBuilder2 =
            $"SELECT * FROM (" +
            $"{(SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}"}) src " +
            $"WHERE IsValid = {true}";
        Assert.True(queryBuilder1 == queryBuilder2);
    }
}
