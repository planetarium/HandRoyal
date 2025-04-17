using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace HandRoyal.Bot;

public sealed class BotCollection : IEnumerable<IBot>, INotifyCollectionChanged
{
    private readonly List<IBot> _bots = [];
    private readonly Dictionary<string, IBot> _botByName = [];

    internal BotCollection()
    {
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

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
        var index = _bots.Count;
        _botByName.Add(bot.Name, bot);
        _bots.Add(bot);
        bot.Disposed += Bot_Disposed;
        InvokeAddedEvent(bot, index);
    }

    private void Bot_Disposed(object? sender, EventArgs e)
    {
        if (sender is IBot bot)
        {
            var index = _bots.IndexOf(bot);
            _bots.RemoveAt(index);
            _botByName.Remove(bot.Name);
            InvokeRemovedEvent(bot, index);
        }
    }

    private void InvokeAddedEvent(IBot bot, int index)
    {
        var action = NotifyCollectionChangedAction.Add;
        var e = new NotifyCollectionChangedEventArgs(action, bot, index);
        CollectionChanged?.Invoke(this, e);
    }

    private void InvokeRemovedEvent(IBot bot, int index)
    {
        var action = NotifyCollectionChangedAction.Remove;
        var e = new NotifyCollectionChangedEventArgs(action, bot, index);
        CollectionChanged?.Invoke(this, e);
    }
}
