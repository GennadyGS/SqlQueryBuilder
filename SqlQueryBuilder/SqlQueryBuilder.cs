using System.Runtime.CompilerServices;
using System.Text;

namespace SqlQueryBuilder;
[InterpolatedStringHandler]

public sealed class SqlQueryBuilder
{
    private const string ParameterTag = "@";
    private const string ParameterPrefix = "p";

    private List<Entry> Entries { get; } = new();

    public string GetQuery() => GetQueryAndParameters().query;

    public IReadOnlyDictionary<string, object?> GetParameters() => 
        GetQueryAndParameters().parameters;

    public (string query, IReadOnlyDictionary<string, object?> parameters) GetQueryAndParameters()
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
                    var parameterName = ParameterPrefix + parameterIndex++;
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

    public SqlQueryBuilder(int literalLength, int formattedCount)
    {
    }

    public void AppendLiteral(string s)
    {
        Entries.Add(new StringEntry(s));
    }

    public void AppendFormatted(SqlQueryBuilder builder)
    {
        Entries.AddRange(builder.Entries);
    }

    public void AppendFormatted<T>(T t)
    {
        Entries.Add(new ParameterEntry(t));
    }

    public static implicit operator SqlQueryBuilder(string s) => new(s);

    private abstract record Entry;

    private record StringEntry(string String) : Entry;

    private record ParameterEntry(object? Value) : Entry;
}
