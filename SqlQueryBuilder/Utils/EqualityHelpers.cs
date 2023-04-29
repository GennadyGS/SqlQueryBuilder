using System.Collections.Generic;
using System.Linq;

namespace WizNG.SqlGeneration.QueryBuilder.Utils;

internal static class EqualityHelpers
{
    public static bool ObjectsEqual(object? x, object? y) =>
        (x, y) switch
        {
            (null, null) => true,
            (null, _) => false,
            var (valueX, valueY) => valueX.Equals(valueY),
        };

    public static bool DictionariesEqual<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> dict1, IReadOnlyDictionary<TKey, TValue> dict2) =>
        dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
}
