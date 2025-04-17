using System.Diagnostics.CodeAnalysis;
using HandRoyal.Pages;
using Microsoft.AspNetCore.Components;
using XtermBlazor;

namespace HandRoyal.Bot.Pages;

public partial class LogMonitor : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TerminalOptions _options = new()
    {
        CursorStyle = CursorStyle.Block,
    };

    [SuppressMessage(
        "Major Code Smell",
        "S1144:Unused private types or members should be removed",
        Justification = "This is used by LogMonitor.razor")]
    private readonly HashSet<string> _addons =
    [
        "addon-fit",
    ];

    private bool _isDisposed;

    [AllowNull]
    private Xterm _terminal;

    [Parameter]
    public ILogSource? LogSource { get; set; }

    [Inject]
    [AllowNull]
    private IThemeService ThemeService { get; set; }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            ThemeService.ThemeChanged -= ThemeService_ThemeChanged;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        UpdateOptions();
        ThemeService.ThemeChanged += ThemeService_ThemeChanged;
    }

    private async void ThemeService_ThemeChanged(object? sender, EventArgs e)
    {
        UpdateOptions();
        await _terminal.SetOptions(_options);
    }

    private void UpdateOptions()
    {
        _options.Theme.Background = ThemeService.Palette["Surface"];
        _options.Theme.Foreground = ThemeService.Palette["TextPrimary"];
        _options.Theme.SelectionBackground
            = ThemeService.IsDarkMode ? "rgb(86, 153, 247)" : "rgb(187,214,251)";
    }

    private async Task OnFirstRender()
    {
        if (LogSource is { } logSource)
        {
            _ = StartAsync(logSource, _cancellationTokenSource.Token);
        }

        await _terminal.Addon("addon-fit").InvokeVoidAsync("fit");
    }

    private async Task StartAsync(ILogSource logSource, CancellationToken cancellationToken)
    {
        await foreach (var item in logSource.GetLogStream(cancellationToken))
        {
            var text = item.Replace("\n", "\r\n");
            await _terminal.WriteLine(text);
        }
    }
}
