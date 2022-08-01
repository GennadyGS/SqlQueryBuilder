namespace SqlQueryBuilder.Utils;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] items) =>
        items.Concat(source);

    public static IReadOnlyDictionary<TKey?, TValue>
        ToDictionaryWithNullableKey<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey?> keySelector,
        Func<TSource, TValue> valueSelector,
        IEqualityComparer<TKey?>? equalityComparer = null)
        where TKey : notnull
    {
        var keyValuePairs = source
            .Select(item => new KeyValuePair<TKey?, TValue>(keySelector(item), valueSelector(item)))
            .ToList();
        return new DictionaryWithNullableKey<TKey, TValue>(keyValuePairs, equalityComparer);
    }
}
