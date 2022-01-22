using System.Runtime.CompilerServices;
using System.Text;

namespace SqlQueryBuilder;

/// <summary>
/// Builds SQL query with parameters from interpolated string.
/// </summary>
[InterpolatedStringHandler]
public sealed class SqlQueryBuilder
{
    private const char ParameterTag = '@';
    private const string DefaultParameterNamePrefix = "p";

    private List<Entry> Entries { get; } = new();

    /// <summary>
    /// Gets the text of SQL query with inlined parameters.
    /// </summary>
    /// <returns>The text of SQL query with inlined parameters.</returns>
    public string GetQuery() => GetQueryAndParameters().query;

    /// <summary>
    /// Gets the dictionary with names and values of SQL query parameters.
    /// </summary>
    /// <returns>The dictionary with names and values of SQL query parameters.</returns>
    public IReadOnlyDictionary<string, object?> GetParameters() => 
        GetQueryAndParameters().parameters;

    /// <summary>
    /// Gets the tuple of SQL query text and dictionary with parameter names and values.
    /// </summary>
    /// <param name="parameterNamePrefix">
    /// Overrides the prefix of generated parameter names.
    /// Default value is <c>"p"</c>.
    /// </param>
    /// <returns>
    /// The tuple of SQL query text and dictionary with parameter names and values.
    /// </returns>
    public (string query, IReadOnlyDictionary<string, object?> parameters) GetQueryAndParameters(
        string parameterNamePrefix = DefaultParameterNamePrefix)
    {
        var parameterIndex = 1;
        var queryBuilder = new StringBuilder();
        var parameters = new Dictionary<string, object?>();
        foreach (var entry in Entries)
        {
            switch (entry)
            {
                case StringEntry se:
                    queryBuilder.Append(se.String);
                    break;
                case ParameterEntry pe:
                    var parameterName = parameterNamePrefix + parameterIndex++;
                    queryBuilder.Append(ParameterTag).Append(parameterName);
                    parameters.Add(parameterName, pe.Value);
                    break;
            }
        }

        return (queryBuilder.ToString(), parameters);
    }

    private SqlQueryBuilder(string s)
    {
        AppendLiteral(s);
    }

    /// <summary>Creates a handler used to translate an interpolated string into a <see cref="string"/>.</summary>
    /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
    /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
    /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style", 
        "IDE0060:Remove unused parameter", 
        Justification = "Constructor with following parameter is required for InterpolatedStringHandler")]
    public SqlQueryBuilder(int literalLength, int formattedCount)
    {
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendLiteral(string value)
    {
        Entries.Add(new StringEntry(value));
    }

    /// <summary>Writes the instance of <see cref="SqlQueryBuilder"/> to the handler.</summary>
    /// <param name="value">The the instance of <see cref="SqlQueryBuilder"/> to write.</param>
    public void AppendFormatted(SqlQueryBuilder value)
    {
        Entries.AddRange(value.Entries);
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendFormatted<T>(T value)
    {
        Entries.Add(new ParameterEntry(value));
    }

    /// <summary>
    /// Implicitly converts <see cref="string"/> into instance of <see cref="SqlQueryBuilder"/>.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The instance of instance of <see cref="SqlQueryBuilder"/>.</returns>
    public static implicit operator SqlQueryBuilder(string s) => new(s);

    private abstract record Entry;

    private sealed record StringEntry(string String) : Entry;

    private sealed record ParameterEntry(object? Value) : Entry;
}
