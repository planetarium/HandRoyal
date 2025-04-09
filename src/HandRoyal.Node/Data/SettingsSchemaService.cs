using Libplanet.Node.Options.Schema;

namespace HandRoyal.Node.Data;

internal sealed class SettingsSchemaService
{
    private string? _schema;

    public async Task<string> GetSchemaAsync(CancellationToken cancellationToken)
    {
        if (_schema is null)
        {
            _schema = await OptionsSchemaBuilder.GetSchemaAsync(cancellationToken);
        }

        return _schema;
    }
}
