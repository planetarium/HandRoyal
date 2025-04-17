using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HandRoyal.Pages;
using MudBlazor;

namespace HandRoyal.Bot.Pages;

internal sealed class BotPageViewModel : IPage
{
    private readonly IBotService _botService;

    public BotPageViewModel(IBotService botService)
    {
        _botService = botService;
        foreach (IBot bot in _botService.Bots)
        {
            Bots.Add(BotViewModel.GetOrCreate(bot));
        }

        _botService.Bots.CollectionChanged += Bots_CollectionChanged;
    }

    public bool IsEnabled => _botService.IsEnabled;

    public string Title => "Bot";

    public string Url => "/bot";

    public string Icon => Icons.Material.Filled.Android;

    public ObservableCollection<BotViewModel> Bots { get; } = [];

    public BotViewModel AddNew() => BotViewModel.GetOrCreate(_botService.AddNew());

    private void Bots_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (IBot bot in e.NewItems!)
                {
                    Bots.Add(BotViewModel.GetOrCreate(bot));
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (IBot bot in e.OldItems!)
                {
                    var viewModel = BotViewModel.GetOrCreate(bot);
                    Bots.Remove(viewModel);
                }

                break;
        }
    }
}
