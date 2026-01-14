using System;
using UnityEngine;

public interface IUnitSpawnSystemEvent 
{
    public event Action<IPlayerData> PlayerSpawnedEvent;
    public event Action<ICharacterData> CharacterSpawnedEvent;
    public event Action EnemySpawnedEvent;
}
