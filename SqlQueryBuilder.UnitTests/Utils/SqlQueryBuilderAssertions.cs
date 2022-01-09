using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace SqlQueryBuilder.UnitTests.Utils;

internal sealed class SqlQueryBuilderAssertions
    : ReferenceTypeAssertions<SqlQueryBuilder, SqlQueryBuilderAssertions>
{
    public SqlQueryBuilderAssertions(SqlQueryBuilder instance) 
        : base(instance)
    {
    }

    public AndConstraint<SqlQueryBuilderAssertions> HaveQuery(
        string query, string because = "", params object[] becauseArgs)
    {
        Subject.GetQuery().Should().Be(query, because, becauseArgs);
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    public AndConstraint<SqlQueryBuilderAssertions> NotHaveParameters(
        string because = "", params object[] becauseArgs)
    {
        Subject.GetParameters()
            .Should().BeEquivalentTo(new Dictionary<string, object>(), because, becauseArgs);
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    public AndConstraint<SqlQueryBuilderAssertions> HaveParameters(
        IReadOnlyDictionary<string, object> parameters, 
        string because = "", 
        params object[] becauseArgs)
    {
        Subject.GetParameters()
            .Should().BeEquivalentTo(parameters, because, becauseArgs);
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    protected override string Identifier => nameof(SqlQueryBuilder);
}