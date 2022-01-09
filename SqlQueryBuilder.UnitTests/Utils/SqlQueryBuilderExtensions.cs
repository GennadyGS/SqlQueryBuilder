namespace SqlQueryBuilder.UnitTests.Utils;

internal static class SqlQueryBuilderExtensions
{
    public static SqlQueryBuilderAssertions Should(this SqlQueryBuilder instance) => new(instance);
}