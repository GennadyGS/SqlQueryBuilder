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
        Subject.Query.Should().Be(query, because, becauseArgs);
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    public AndConstraint<SqlQueryBuilderAssertions> NotHaveParameters()
    {
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    protected override string Identifier => nameof(SqlQueryBuilder);
}