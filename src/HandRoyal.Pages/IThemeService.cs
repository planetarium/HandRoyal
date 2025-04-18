namespace HandRoyal.Pages;

public interface IThemeService
{
    event EventHandler? ThemeChanged;

    bool IsDarkMode { get; }

    IReadOnlyDictionary<string, string> Palette { get; }
}
