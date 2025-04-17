using HandRoyal.Pages;

namespace HandRoyal.Executable.Pages;

internal sealed class CounterPage : IPage
{
    public string Title => "Counter";

    public string Url => "/counter";

    public string Icon => "oi oi-plus";
}
