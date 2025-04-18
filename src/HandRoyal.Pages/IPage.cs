namespace HandRoyal.Pages;

public interface IPage
{
    public bool IsEnabled => true;

    public string Title { get; }

    public string Url { get; }

    public string Icon { get; }
}
