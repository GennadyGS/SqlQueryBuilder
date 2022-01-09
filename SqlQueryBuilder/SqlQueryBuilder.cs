using System.Runtime.CompilerServices;

namespace SqlQueryBuilder;
[InterpolatedStringHandler]

public sealed class SqlQueryBuilder
{
    public string Query { get; private set; }

    private SqlQueryBuilder(string s)
    {
        Query = s;
    }

    public SqlQueryBuilder(int literalLength, int formattedCount)
    {
        Query = string.Empty;
    }


    public void AppendLiteral(string s)
    {
        Query += s;
    }

    public void AppendFormatted(SqlQueryBuilder query)
    {
        Query += query.Query;
    }

    public void AppendFormatted<T>(T t)
    {
        throw new NotImplementedException();
    }

    public static implicit operator SqlQueryBuilder(string s) =>
        new(s);
}
