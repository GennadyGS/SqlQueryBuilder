namespace SqlQueryBuilder.UnitTests.Utils;

internal static class SqlQueryWithParametersExtensions
{
    public static SqlQueryBuilderAssertions Should(this SqlQueryBuilder instance) => new(instance);
}