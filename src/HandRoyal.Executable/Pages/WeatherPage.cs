using HandRoyal.Pages;

namespace HandRoyal.Executable.Pages;

internal sealed class WeatherPage : IPage
{
    public string Title => "Weather";

    public string Url => "/weather";

    public string Icon => "oi oi-list-rich";
}
