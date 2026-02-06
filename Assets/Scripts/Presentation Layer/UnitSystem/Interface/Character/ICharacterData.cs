using UnityEngine;

public interface ICharacterData
{
    Transform GetTransform();
    float GetMaxHealth();
    float GetCurrentHealth();
    CharacterType characterType { get; }
    ICharacterStatProvider GetStatProvider();
}
