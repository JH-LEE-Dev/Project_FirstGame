using System;
using UnityEngine;

public interface IUnitSpawnSystemEvent 
{
    public event Action PlayerSpawnedEvent;
    public event Action EnemySpawnedEvent;
}
