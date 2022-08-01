using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SqlQueryBuilder.Utils;

internal sealed class DictionaryWithNullableKey<TKey, TValue>
    : IReadOnlyDictionary<TKey?, TValue>
    where TKey : notnull
{
    private readonly IReadOnlyDictionary<TKey, TValue> _inner;
    private readonly bool _hasDefaultKey;
    private readonly TValue _valueForDefaultKey = default!;
    private readonly IEqualityComparer<TKey?> _equalityComparer;

    public DictionaryWithNullableKey(
        IReadOnlyCollection<KeyValuePair<TKey?, TValue>> keyValuePairs,
        IEqualityComparer<TKey?>? equalityComparer = null)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<TKey?>.Default;

        var defaultKeyValuePairs = keyValuePairs
            .Where(kvp => _equalityComparer.Equals(kvp.Key, default))
            .ToList();
        if (defaultKeyValuePairs.Count > 1)
        {
            throw new ArgumentException("Default key is duplicated.", nameof(keyValuePairs));
        }

        if (defaultKeyValuePairs.Any())
        {
            _hasDefaultKey = true;
            _valueForDefaultKey = defaultKeyValuePairs.Single().Value;
        }

        _inner = keyValuePairs
            .Where(kvp => !_equalityComparer.Equals(kvp.Key, default))
            .Select(kvp => KeyValuePair.Create(kvp.Key!, kvp.Value))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public int Count =>
        _hasDefaultKey
            ? _inner.Count + 1
            : _inner.Count;

    public IEnumerable<TKey?> Keys =>
        _hasDefaultKey
            ? _inner.Keys.Prepend((TKey?)default)
            : _inner.Keys;

    public IEnumerable<TValue> Values =>
        _hasDefaultKey
            ? _inner.Values.Prepend(_valueForDefaultKey)
            : _inner.Values;

    public TValue this[TKey? key] =>
        TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException($"Key {key} is not found in dictionary");

    [SuppressMessage(
        "Minor Code Smell",
        "S1905:Redundant casts should not be used",
        Justification = "Bug in analyzer: cast is required")]
    public IEnumerator<KeyValuePair<TKey?, TValue>> GetEnumerator()
    {
        var innerNullable = _inner.Select(kvp => KeyValuePair.Create((TKey?)kvp.Key, kvp.Value));
        var keyValuePairs = _hasDefaultKey
            ? innerNullable.Prepend(KeyValuePair.Create((TKey?)default, _valueForDefaultKey))
            : innerNullable;
        return keyValuePairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(TKey? key) =>
        _equalityComparer.Equals(key, default)
            ? _hasDefaultKey
            : _inner.ContainsKey(key!);

    public bool TryGetValue(TKey? key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_equalityComparer.Equals(key, default))
        {
            value = _valueForDefaultKey;
            return _hasDefaultKey;
        }

        return _inner.TryGetValue(key!, out value);
    }
}
