using System.Collections.Generic;
using UnityEngine;

public interface IUnitLogicSystemProvider
{
    public IReadOnlyList<IEnemyData> enemyData { get; }

    public ICharacterData characterData { get; }

    public IEarthData earthData { get; }
}
