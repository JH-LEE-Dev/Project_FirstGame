using UnityEngine;

public interface IGameModuleProvider
{
    ICardSystemEvents cardSystemEvents { get; }
    ICardSystemData cardSystemData { get; }
    ICardSystemActions cardSystemActions { get; }
    IUnitLogicSystemData unitLogicSystemData { get; }
    IUnitEventAccessor unitEventAccessor { get; }
    IUnitSpawnSystemEvent unitSpawnSystemEvent { get; }
}
