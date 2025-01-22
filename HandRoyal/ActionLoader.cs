using Bencodex.Types;
using HandRoyal.Actions;
using Libplanet.Action;
using Libplanet.Action.Loader;

namespace HandRoyal;

public class ActionLoader : IActionLoader
{
    private readonly TypedActionLoader _actionLoader =
        TypedActionLoader.Create(typeof(ActionBase).Assembly, typeof(ActionBase));

    public IAction LoadAction(long index, IValue value) => _actionLoader.LoadAction(index, value);
}
