namespace SqlQueryWithParameters.UnitTests.Utils;

internal static class SqlQueryWithParametersExtensions
{
    public static SqlQueryWithParametersAssertions Should(this SqlQueryBuilder instance) => new(instance);
}