using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandRoyal.Pages;

namespace HandRoyal.Bot.Pages;

public sealed class BotViewModel : INotifyPropertyChanged, IDisposable, ILogSource
{
    private static readonly ConcurrentDictionary<IBot, BotViewModel> _viewModels = [];

    private readonly IBot _bot;
    private readonly Dictionary<string, object> _properties;
    private bool _isChecked;
    private bool _isRequsting;

    public BotViewModel(IBot bot)
    {
        _bot = bot;
        _properties = new Dictionary<string, object>
        {
            { nameof(IBot.Name), _bot.Name },
            { nameof(IBot.Address), _bot.Address.ToString() },
            { nameof(IBot.EndPoint), _bot.EndPoint.ToString() },
            { nameof(IBot.IsRunning), _bot.IsRunning },
            { nameof(IBot.Properties), _bot.Properties },
        };
        _bot.JobUpdated += (s, e) => JobUpdated?.Invoke(this, EventArgs.Empty);
        _bot.Disposed += (s, e) => _viewModels.TryRemove(_bot, out _);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? JobUpdated;

    public IBot Target => _bot;

    public IReadOnlyDictionary<string, object> Properties => _properties;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                NotifyStateChanged();
            }
        }
    }

    public string Name => _bot.Name;

    public string Type => _bot.GetType().Name;

    public string JobName => _bot.JobInfo.Name;

    public string JobState => _bot.JobInfo.State;

    public string JobStartTime => GetDateTime(_bot.JobInfo.StartTime);

    public string JobFinishTime => GetDateTime(_bot.JobInfo.FinishTime);

    public string JobDuration
        => DateTimeOffset.Now.Subtract(_bot.JobInfo.StartTime).ToString(@"hh\:mm\:ss");

    public bool IsRunning => _bot.IsRunning;

    public bool CanStart => !IsRunning && !_isRequsting;

    public bool CanStop => IsRunning && !_isRequsting;

    public static BotViewModel GetOrCreate(IBot bot)
        => _viewModels.GetOrAdd(bot, bot => new BotViewModel(bot));

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        using var scope = new RequestScope(this);
        await _bot.StartAsync(cancellationToken);
        NotifyStateChanged();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        using var scope = new RequestScope(this);
        await _bot.StopAsync(cancellationToken);
        NotifyStateChanged();
    }

    public void Dispose()
    {
        _bot.Dispose();
    }

    async IAsyncEnumerable<string> ILogSource.GetLogStream(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_bot is ILogSource logSource)
        {
            await foreach (var item in logSource.GetLogStream(cancellationToken))
            {
                yield return item;
            }
        }
    }

    private static string GetDateTime(DateTimeOffset dateTime)
        => dateTime == DateTimeOffset.MinValue ? string.Empty : dateTime.ToString("HH:mm:ss");

    private void NotifyStateChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private sealed class RequestScope : IDisposable
    {
        private readonly BotViewModel _viewModel;

        public RequestScope(BotViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel._isRequsting = true;
        }

        public void Dispose()
        {
            _viewModel._isRequsting = false;
        }
    }
}
