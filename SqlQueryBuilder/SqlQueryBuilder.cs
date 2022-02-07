using System.Runtime.CompilerServices;
using System.Text;
using SqlQueryBuilder.Utils;

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
        var parameterValueToNameMap = Entries
            .OfType<ParameterEntry>()
            .Select(entry => entry.Value)
            .Distinct()
            .Select((value, index) => (value, index))
            .ToDictionaryWithNullableKey(
                item => item.value,
                item => parameterNamePrefix + (item.index + 1));

        void AppendEntryToStringBuilder(Entry entry, StringBuilder stringBuilder)
        {
            switch (entry)
            {
                case ParameterEntry parameterEntry:
                    stringBuilder.Append(ParameterTag);
                    stringBuilder.Append(parameterValueToNameMap[parameterEntry.Value]);
                    break;
                case StringEntry stringEntry:
                    stringBuilder.Append(stringEntry.String);
                    break;
                default:
                    throw new InvalidOperationException("Impossible case");
            }
        }

        var queryBuilder = new StringBuilder();
        foreach (var entry in Entries)
        {
            AppendEntryToStringBuilder(entry, queryBuilder);
        }

        var parameters = parameterValueToNameMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
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

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendFormatted<T>(T? value)
    {
        Entries.Add(new ParameterEntry(value));
    }

    /// <summary>Writes the instance of <see cref="SqlQueryBuilder"/> to the handler.</summary>
    /// <param name="value">The the instance of <see cref="SqlQueryBuilder"/> to write.</param>
    public void AppendFormatted(SqlQueryBuilder? value)
    {
        if (value != default)
        {
            Entries.AddRange(value.Entries);
        }
        else
        {
            Entries.Add(new ParameterEntry(default));
        }
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
