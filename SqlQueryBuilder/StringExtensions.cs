namespace SqlQueryBuilder;

/// <summary>
/// Provides set of extension methods on stings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Wraps <see cref="string"/> value into <see cref="FormattedLiteral"/>
    /// in order to indicate that it should be interpreted as inline literal value
    /// rather than as parameter.
    /// </summary>
    /// <param name="value">Value to be wrapped.</param>
    /// <returns>Wrapped value.</returns>
    public static FormattedLiteral AsLiteral(this string value) => new(value);
}