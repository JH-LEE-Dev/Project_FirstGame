using UnityEngine;

public interface IEnemyData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
}
