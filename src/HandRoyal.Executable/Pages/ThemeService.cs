using HandRoyal.Pages;
using MudBlazor;
using MudBlazor.Utilities;

namespace HandRoyal.Executable.Pages;

internal sealed class ThemeService : IThemeService
{
    private readonly Dictionary<string, string> _palette = [];
    private MudTheme _theme = new();
    private bool _isDarkMode = true;

    public event EventHandler? ThemeChanged;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                UpdatePalette(_theme);
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public MudTheme Theme
    {
        get => _theme;
        set
        {
            if (_theme != value)
            {
                _theme = value;
                UpdatePalette(_theme);
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    IReadOnlyDictionary<string, string> IThemeService.Palette => _palette;

    private void UpdatePalette(MudTheme theme)
    {
        var palette = _isDarkMode ? (Palette)theme.PaletteDark : theme.PaletteLight;
        var properties = palette.GetType().GetProperties();

        _palette.Clear();
        foreach (var property in properties)
        {
            var value = property.GetValue(palette);
            if (value is string strValue)
            {
                _palette[property.Name] = strValue;
            }
            else if (value is MudColor colorValue)
            {
                _palette[property.Name] = colorValue.ToString(MudColorOutputFormats.RGBA);
            }
            else if (value is double doubleValue)
            {
                _palette[property.Name] = doubleValue.ToString("R");
            }
            else
            {
                throw new NotSupportedException($"Unsupported type: {value?.GetType()}");
            }
        }
    }
}
