#pragma warning disable MEN002 // Line is too long
using System.Collections;

namespace HandRoyal.Bot;

public sealed partial class BotPropertyCollection : IDictionary<object, object?>
{
    ICollection<object> IDictionary<object, object?>.Keys => _valueByType.Keys;

    ICollection<object?> IDictionary<object, object?>.Values => _valueByType.Values;

    bool ICollection<KeyValuePair<object, object?>>.IsReadOnly => false;

    object? IDictionary<object, object?>.this[object key]
    {
        get => _valueByType[key];
        set => _valueByType[key] = value;
    }

    void IDictionary<object, object?>.Add(object key, object? value)
        => _valueByType.Add(key, value);

    void ICollection<KeyValuePair<object, object?>>.Add(KeyValuePair<object, object?> item)
        => _valueByType.Add(item.Key, item.Value);

    bool ICollection<KeyValuePair<object, object?>>.Contains(KeyValuePair<object, object?> item)
    {
        if (_valueByType.TryGetValue(item.Key, out var value))
        {
            return EqualityComparer<object?>.Default.Equals(value, item.Value);
        }

        return false;
    }

    bool IDictionary<object, object?>.ContainsKey(object key) => _valueByType.ContainsKey(key);

    void ICollection<KeyValuePair<object, object?>>.CopyTo(
        KeyValuePair<object, object?>[] array, int arrayIndex)
    {
        if (arrayIndex < 0 || arrayIndex >= array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        foreach (var item in _valueByType)
        {
            array[arrayIndex++] = item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _valueByType.Values.GetEnumerator();

    IEnumerator<KeyValuePair<object, object?>> IEnumerable<KeyValuePair<object, object?>>.GetEnumerator()
    {
        foreach (var item in _valueByType)
        {
            yield return item;
        }
    }

    bool IDictionary<object, object?>.Remove(object key) => _valueByType.Remove(key);

    bool ICollection<KeyValuePair<object, object?>>.Remove(KeyValuePair<object, object?> item)
    {
        if (_valueByType.TryGetValue(item.Key, out var value)
            && EqualityComparer<object?>.Default.Equals(value, item.Value))
        {
            _valueByType.Remove(item.Key);
            return true;
        }

        return false;
    }

    bool IDictionary<object, object?>.TryGetValue(object key, out object? value)
    {
        if (_valueByType.TryGetValue(key, out var objValue))
        {
            value = objValue;
            return true;
        }

        value = null;
        return false;
    }
}
