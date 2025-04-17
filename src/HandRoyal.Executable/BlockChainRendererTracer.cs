using Libplanet.Node.Services;

namespace HandRoyal.Executable;

internal sealed class BlockChainRendererTracer(
    IRendererService rendererService, ILogger<BlockChainRendererTracer> logger)
    : IHostedService
{
    private IDisposable? _observer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _observer = rendererService.RenderBlockEnd.Subscribe(
            info => logger.LogInformation(
                "-Pattern2- #{Height} Block end: {Hash}",
                info.NewTip.Index,
                info.NewTip.Hash));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _observer?.Dispose();
        _observer = null;
        return Task.CompletedTask;
    }
}
