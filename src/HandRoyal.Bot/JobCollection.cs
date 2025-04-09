using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HandRoyal.Bot;

public sealed class JobCollection : IEnumerable<IJob>
{
    private readonly List<IJob> _states;
    private readonly Dictionary<string, IJob> _stateByName;
    private readonly Dictionary<Type, IJob> _stateByType;

    internal JobCollection(IJob[] states)
    {
        _states = new List<IJob>(states.Length);
        _stateByName = new Dictionary<string, IJob>(states.Length);
        _stateByType = new Dictionary<Type, IJob>(states.Length);
        foreach (var state in states)
        {
            _stateByName.Add(state.Name, state);
            _states.Add(state);
            _stateByType.Add(state.GetType(), state);
        }
    }

    public int Count => _states.Count;

    public IJob this[string name] => _stateByName[name];

    public IJob this[int index] => _states[index];

    public IJob this[Type type] => _stateByType[type];

    public bool Contains(IJob state) => _states.Contains(state);

    public bool Contains(string name) => _stateByName.ContainsKey(name);

    public bool Contains(Type type) => _stateByType.ContainsKey(type);

    public bool TryGetState(string name, [MaybeNullWhen(false)] out IJob state)
        => _stateByName.TryGetValue(name, out state);

    public bool TryGetState(Type type, [MaybeNullWhen(false)] out IJob state)
        => _stateByType.TryGetValue(type, out state);

    IEnumerator<IJob> IEnumerable<IJob>.GetEnumerator() => _states.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _states.GetEnumerator();
}
