using Bencodex.Types;
using LastHandStanding.Actions;
using Libplanet.Action;
using Libplanet.Action.Loader;

namespace LastHandStanding;

public class ActionLoader : IActionLoader
{
    private readonly TypedActionLoader _actionLoader =
        TypedActionLoader.Create(typeof(ActionBase).Assembly, typeof(ActionBase));

    public IAction LoadAction(long index, IValue value) => _actionLoader.LoadAction(index, value);
}
