using System.Collections.Generic;
using UnityEngine;

public interface IUnitLogicSystemData
{
    public IReadOnlyList<IEnemyData> enemyData { get; }

    public ICharacterData characterData { get; }

    public IPlayerData playerData { get; }
}
