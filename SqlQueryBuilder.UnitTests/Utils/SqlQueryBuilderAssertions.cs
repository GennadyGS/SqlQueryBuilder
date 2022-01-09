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

    public AndConstraint<SqlQueryBuilderAssertions> BeQueryWithParameters(string query)
    {
        Subject.Query.Should().Be(query);
        return new AndConstraint<SqlQueryBuilderAssertions>(this);
    }

    protected override string Identifier => nameof(SqlQueryBuilder);
}