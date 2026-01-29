using System.Collections.Generic;

public interface IUnitSpawnSystemData
{
    ICharacterData characterData { get; }
    IPlayerData playerData { get; }
    List<IEnemyData> enemiesData { get; }
}
