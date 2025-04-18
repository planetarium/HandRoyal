#pragma warning disable MEN002 // Line is too long
using System.Collections;

namespace HandRoyal.Bot;

public sealed partial class BotPropertyCollection : IDictionary<string, object?>
{
    ICollection<string> IDictionary<string, object?>.Keys => _valueByType.Keys;

    ICollection<object?> IDictionary<string, object?>.Values => _valueByType.Values;

    bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => false;

    object? IDictionary<string, object?>.this[string key]
    {
        get => _valueByType[key];
        set => _valueByType[key] = value;
    }

    void IDictionary<string, object?>.Add(string key, object? value)
        => _valueByType.Add(key, value);

    void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
        => _valueByType.Add(item.Key, item.Value);

    bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
    {
        if (_valueByType.TryGetValue(item.Key, out var value))
        {
            return EqualityComparer<object?>.Default.Equals(value, item.Value);
        }

        return false;
    }

    bool IDictionary<string, object?>.ContainsKey(string key) => _valueByType.ContainsKey(key);

    void ICollection<KeyValuePair<string, object?>>.CopyTo(
        KeyValuePair<string, object?>[] array, int arrayIndex)
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

    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
    {
        foreach (var item in _valueByType)
        {
            yield return item;
        }
    }

    bool IDictionary<string, object?>.Remove(string key) => _valueByType.Remove(key);

    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        if (_valueByType.TryGetValue(item.Key, out var value)
            && EqualityComparer<object?>.Default.Equals(value, item.Value))
        {
            _valueByType.Remove(item.Key);
            return true;
        }

        return false;
    }

    bool IDictionary<string, object?>.TryGetValue(string key, out object? value)
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
