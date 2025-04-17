using System.Diagnostics.CodeAnalysis;
using BlazorMonaco.Editor;
using HandRoyal.Executable.Data;
using HandRoyal.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HandRoyal.Executable.Pages;

public partial class Schema : IDisposable
{
    [AllowNull]
    private string _schema;
    [AllowNull]
    private StandaloneCodeEditor _editor;

    [AllowNull]
    [Inject]
    private SettingsSchemaService SchemaService { get; set; }

    [AllowNull]
    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [AllowNull]
    [Inject]
    private IThemeService ThemeService { get; set; }

    void IDisposable.Dispose()
    {
        ThemeService.ThemeChanged -= ThemeService_ThemeChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        _schema = await SchemaService.GetSchemaAsync(default);
        ThemeService.ThemeChanged += ThemeService_ThemeChanged;
    }

    private string GetTheme() => ThemeService.IsDarkMode ? "vs-dark" : "vs-light";

    private void ThemeService_ThemeChanged(object? sender, EventArgs e) => UpdateEditorTheme();

    private void UpdateEditorTheme()
    {
        _editor.UpdateOptions(new EditorUpdateOptions { Theme = GetTheme() });
    }

    private async Task CopyToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", _schema);
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(
        StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = "json",
            Value = _schema ?? string.Empty,
            ReadOnly = true,
            ScrollBeyondLastLine = false,
            LineNumbers = "on",
            RenderWhitespace = "selection",
            WordWrap = "on",
            Theme = GetTheme(),
        };
    }
}
