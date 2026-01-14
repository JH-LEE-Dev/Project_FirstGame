using UnityEngine;

public interface IUISignalHubProvider
{
    public ICardUISignalHub cardUISignalHub { get; }
    public IGameplayUISignalHub gameplayUISignalHub { get; }
}
