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

    private readonly List<Entry> _entries = new();
    private readonly Dictionary<string, object?> _metadata = new();

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata => _metadata;

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
        var parameterValueToNameMap = _entries
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
        foreach (var entry in _entries)
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
        _entries.Add(new StringEntry(value));
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendFormatted<T>(T? value)
    {
        _entries.Add(new ParameterEntry(value));
    }

    /// <summary>Writes the instance of <see cref="SqlQueryBuilder"/> to the handler.</summary>
    /// <param name="value">The the instance of <see cref="SqlQueryBuilder"/> to write.</param>
    public void AppendFormatted(SqlQueryBuilder? value)
    {
        if (value != default)
        {
            _entries.AddRange(value._entries);
            AddMetadata(value._metadata);
        }
        else
        {
            _entries.Add(new ParameterEntry(default));
        }
    }

    /// <summary>
    /// Implicitly converts <see cref="string"/> into instance of <see cref="SqlQueryBuilder"/>.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The instance of instance of <see cref="SqlQueryBuilder"/>.</returns>
    public static implicit operator SqlQueryBuilder(string s) => new(s);

    /// <summary>
    /// Adds metadata entry to query.
    /// </summary>
    /// <remarks>
    /// Subsequent calls with the same key and value are idempotent,
    /// while calls with the same key and different values cause <see cref="InvalidOperationException"/>.
    /// </remarks>
    /// <param name="key">Metadata key.</param>
    /// <param name="value">Metadata value.</param>
    /// <returns>Instance of <see cref="SqlQueryBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Metadata entry with key <paramref name="key"/>
    /// but value different from <paramref name="value"/> already exists.
    /// </exception>
    public SqlQueryBuilder AddMetadata(string key, object? value)
    {
        if (!_metadata.TryGetValue(key, out var existingValue))
        {
            _metadata.Add(key, value);
        }
        else
        {
            if (!ObjectsEqual(value, existingValue))
            {
                throw new InvalidOperationException(
                    $"Inconsistent value cannot be set to metadata key '{key}': '{value}' vs '{existingValue}'");
            }
        }

        return this;
    }

    /// <summary>
    /// Adds metadata entries to query.
    /// </summary>
    /// <remarks>
    /// Subsequent calls with the same key and value are idempotent,
    /// while calls with the same key and different values cause <see cref="InvalidOperationException"/>.
    /// </remarks>
    /// <param name="metadata">Dictionary containing multiple metadata entries.</param>
    /// <returns>Instance of <see cref="SqlQueryBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Metadata entry exists with key from <paramref name="metadata"/> but with different value.
    /// </exception>
    public SqlQueryBuilder AddMetadata(IEnumerable<KeyValuePair<string, object?>> metadata)
    {
        foreach (var (key, value) in metadata)
        {
            AddMetadata(key, value);
        }

        return this;
    }

    private static bool ObjectsEqual(object? x, object? y) =>
        (x, y) switch
        {
            (null, null) => true,
            (null, _) => false,
            var (xValue, yValue) => xValue.Equals(yValue),
        };

    private abstract record Entry;

    private sealed record StringEntry(string String) : Entry;

    private sealed record ParameterEntry(object? Value) : Entry;
}
