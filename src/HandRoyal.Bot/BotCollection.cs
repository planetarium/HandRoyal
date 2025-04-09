using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HandRoyal.Bot;

public sealed class BotCollection : IEnumerable<IBot>
{
    private readonly List<IBot> _bots = [];
    private readonly Dictionary<string, IBot> _botByName = [];

    internal BotCollection()
    {
    }

    public int Count => _bots.Count;

    public IBot this[string name] => _botByName[name];

    public IBot this[int index] => _bots[index];

    public bool Contains(IBot bot) => _bots.Contains(bot);

    public bool Contains(string name) => _botByName.ContainsKey(name);

    public bool TryGetBot(string name, [MaybeNullWhen(false)] out IBot bot)
        => _botByName.TryGetValue(name, out bot);

    IEnumerator<IBot> IEnumerable<IBot>.GetEnumerator() => _bots.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _bots.GetEnumerator();

    internal void Add(IBot bot)
    {
        _botByName.Add(bot.Name, bot);
        _bots.Add(bot);
    }
}
