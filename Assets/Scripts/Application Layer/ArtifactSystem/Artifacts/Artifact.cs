using System.Collections.Generic;
using UnityEngine;

public class Artifact
{
    public ArtifactData artifactData {  get; private set; }

    private List<ArtifactCommand> cardLogicSystemEffects = new List<ArtifactCommand>(3);
    private List<ArtifactCommand> cardDataControlSystemEffects = new List<ArtifactCommand>(3);
    private List<ArtifactCommand> cardStatusEffects = new List<ArtifactCommand>(3);
    private List<ArtifactCommand> cardSlotSystemEffects = new List<ArtifactCommand>(3);
    private List<ArtifactCommand> complexSystemEffects = new List<ArtifactCommand>(3);
    private List<ArtifactCommand> selectionSystemEffects = new List<ArtifactCommand>(3);

    public List<ArtifactCommand> GetcardLogicSystemEffects() { return cardLogicSystemEffects; }
    public List<ArtifactCommand> GetcardDataControlSystemEffects() { return cardDataControlSystemEffects; }
    public List<ArtifactCommand> GetcardStatusEffects() { return cardStatusEffects; }
    public List<ArtifactCommand> GetcardSlotSystemEffects() { return cardSlotSystemEffects; }
    public List<ArtifactCommand> GetcomplexSystemEffects() { return complexSystemEffects; }
    public List<ArtifactCommand> GetselectionSystemEffects() { return selectionSystemEffects; }

    private bool bPermanentUpgrade = false;
    private bool bUpgrade = false;

    public void Initialize(ArtifactData _data)
    {
        artifactData = _data;

        ReadyArtifactEffects();
    }

    private void ReadyArtifactEffects()
    {
        for (int i = 0; i < artifactData.cardLogicSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.cardLogicSystemEffects_Prefab[i]);

            cardLogicSystemEffects.Add(commands);
        }

        for (int i = 0; i < artifactData.cardStatusEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.cardStatusEffects_Prefab[i]);

            cardStatusEffects.Add(commands);
        }

        for (int i = 0; i < artifactData.cardDataControlSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.cardDataControlSystemEffects_Prefab[i]);

            cardDataControlSystemEffects.Add(commands);
        }

        for (int i = 0; i < artifactData.cardSlotSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.cardSlotSystemEffects_Prefab[i]);

            cardSlotSystemEffects.Add(commands);
        }

        for (int i = 0; i < artifactData.complexSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.complexSystemEffects_Prefab[i]);

            complexSystemEffects.Add(commands);
        }

        for (int i = 0; i < artifactData.selectionSystemEffects_Prefab.Count; ++i)
        {
            var commands = UnityEngine.Object.Instantiate(artifactData.selectionSystemEffects_Prefab[i]);

            selectionSystemEffects.Add(commands);
        }
    }

    public void Reset()
    {

    }

    public bool IsUpgraded()
    {
        return bUpgrade || bPermanentUpgrade;
    }

    public void SetUpgrade(bool boolean)
    {
        bUpgrade = boolean;
    }

    public void SetPermanentlyUpgrade(bool boolean)
    {
        bPermanentUpgrade = boolean;
    }
}
