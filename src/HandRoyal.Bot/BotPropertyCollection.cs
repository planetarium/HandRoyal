using System.Diagnostics.CodeAnalysis;

namespace HandRoyal.Bot;

public sealed partial class BotPropertyCollection
{
    private readonly Dictionary<string, object?> _valueByType = [];

    internal BotPropertyCollection()
    {
    }

    public int Count => _valueByType.Count;

    public object this[Type type]
    {
        get
        {
            if (_valueByType.TryGetValue($"{type}", out var value))
            {
                return value ?? throw new KeyNotFoundException(
                    $"Property of type {type} is null.");
            }

            throw new KeyNotFoundException($"Property of type {type} not found.");
        }

        set
        {
            if (!type.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    $"Value type {value.GetType()} does not match key type {type}.", nameof(value));
            }

            _valueByType[$"{type}"] = value;
        }
    }

    public object? this[string key]
    {
        get => _valueByType[key];
        set => _valueByType[key] = value;
    }

    public void Add(object value)
    {
        _valueByType[$"{value.GetType()}"] = value;
    }

    public void Add<T>(object value)
    {
        _valueByType[$"{typeof(T)}"] = value;
    }

    public bool Contains(Type type) => _valueByType.ContainsKey($"{type}");

    public bool Contains(string key) => _valueByType.ContainsKey(key);

    public bool Remove(Type type) => _valueByType.Remove($"{type}");

    public bool Remove(string key) => _valueByType.Remove(key);

    public bool TryGetValue<T>([MaybeNullWhen(false)] out T value)
    {
        if (_valueByType.TryGetValue($"{typeof(T)}", out var objValue) && objValue is T tValue)
        {
            value = tValue;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        if (_valueByType.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }

        value = default;
        return false;
    }

    public T GetValueOrDefault<T>()
    {
        if (_valueByType.TryGetValue($"{typeof(T)}", out var objValue) && objValue is T tValue)
        {
            return tValue;
        }

        return Activator.CreateInstance<T>();
    }

    public T GetValueOrDefault<T>(T defaultValue)
    {
        if (_valueByType.TryGetValue($"{typeof(T)}", out var objValue) && objValue is T tValue)
        {
            return tValue;
        }

        return defaultValue;
    }

    public void Clear() => _valueByType.Clear();
}
