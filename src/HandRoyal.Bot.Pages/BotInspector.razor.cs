using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BlazorMonaco.Editor;
using HandRoyal.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HandRoyal.Bot.Pages;

public partial class BotInspector : ComponentBase, IDisposable
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
    };

    private bool _isDisposed;
    [AllowNull]
    private StandaloneCodeEditor _inspector;
    [AllowNull]
    private MudTabs _tabs;

    [Parameter]
    public BotViewModel? Bot { get; set; }

    [AllowNull]
    [Inject]
    private IThemeService ThemeService { get; set; }

    void IDisposable.Dispose()
    {
        if (!_isDisposed)
        {
            ThemeService.ThemeChanged -= ThemeService_ThemeChanged;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ThemeService.ThemeChanged += ThemeService_ThemeChanged;
    }

    private string Inspect()
    {
        if (Bot is { } bot)
        {
            return JsonSerializer.Serialize(bot.Properties, _options);
        }

        return string.Empty;
    }

    private string GetTheme() => ThemeService.IsDarkMode ? "vs-dark" : "vs-light";

    private void ThemeService_ThemeChanged(object? sender, EventArgs e) => UpdateEditorTheme();

    private void UpdateEditorTheme()
    {
        _inspector.UpdateOptions(new EditorUpdateOptions { Theme = GetTheme() });
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(
        StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = "json",
            Value = Inspect(),
            ReadOnly = true,
            ScrollBeyondLastLine = false,
            LineNumbers = "on",
            RenderWhitespace = "selection",
            WordWrap = "on",
            Theme = GetTheme(),
        };
    }
}
