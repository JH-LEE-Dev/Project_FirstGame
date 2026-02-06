using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArtifactData
{
    public ArtifactType type;

    [Space]
    [Header("Artifact Effects")]
    public List<ArtifactCommand> cardLogicSystemEffects_Prefab;
    public List<ArtifactCommand> cardDataControlSystemEffects_Prefab;
    public List<ArtifactCommand> cardStatusEffects_Prefab;
    public List<ArtifactCommand> cardSlotSystemEffects_Prefab;
    public List<ArtifactCommand> complexSystemEffects_Prefab;
    public List<ArtifactCommand> selectionSystemEffects_Prefab;
}