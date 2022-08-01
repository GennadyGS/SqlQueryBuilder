namespace SqlQueryBuilder;

/// <summary>
/// Wrapper on top of <see cref="string"/> value indicating that it should be interpreted
/// as inlined literal string, rather than as parameter
/// </summary>
/// <param name="String">Wrapped <see cref="string"/> value.</param>
public record struct FormattedLiteral(string String);
