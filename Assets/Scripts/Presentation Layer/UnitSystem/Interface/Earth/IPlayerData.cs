using UnityEngine;

public interface IPlayerData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    float GetCurrentShield();

    float GetPrevHealth();

    float GetPrevShield();
    int GetPlayerCurrentMoney();
}
