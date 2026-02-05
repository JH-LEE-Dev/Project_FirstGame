using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArtifactData
{
    public ArtifactType type;

    [Space]
    [Header("Card Effects")]
    public List<CardEffectCommand> cardLogicSystemEffects_Prefab;
    public List<CardEffectCommand> cardDataControlSystemEffects_Prefab;
    public List<CardEffectCommand> cardStatusEffects_Prefab;
    public List<CardEffectCommand> cardSlotSystemEffects_Prefab;
    public List<CardEffectCommand> complexSystemEffects_Prefab;
    public List<CardEffectCommand> selectionSystemEffects_Prefab;
}