using UnityEngine;

public interface IUnitSignalHubProvider 
{
    public ICharacterSignalHub characterSignalHub { get; }
    public IPlayerSignalHub playerSignalHub { get; }
}
