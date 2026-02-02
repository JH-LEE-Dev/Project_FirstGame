using UnityEngine;

public interface ICharacterData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();

    ICharacterStatProvider GetStatProvider();
}
