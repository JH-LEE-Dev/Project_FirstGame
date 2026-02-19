using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArtifactManager : MonoBehaviour
{
    public event Action ArtifactAppliedEvent_AfterCardUsingPhase;

    [SerializeField] private ArtifactDataBase artifactDataBase;

    private Dictionary<int, ObjectPool<Artifact>> artifactPools = new Dictionary<int, ObjectPool<Artifact>>();

    private List<Artifact> equippedArtifacts = new List<Artifact>(SYSTEM_VAR.maxArtifactCount);

    public event Action<ArtifactCommand, EffectApplyType, bool> AfrifactCommandDispatchEvent;

    private List<ArtifactCommand> artifactEffect_BeforeTurn = new List<ArtifactCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<ArtifactCommand> artifactEffect_BeforeAttack = new List<ArtifactCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<ArtifactCommand> artifactEffect_AfterAttack = new List<ArtifactCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<ArtifactCommand> artifactEffect_BeforeCardUsingPhase = new List<ArtifactCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<ArtifactCommand> artifactEffect_AfterCardUsingPhase = new List<ArtifactCommand>(SYSTEM_VAR.maxDeckPileCount);

    //BeforeTurn,AfterAttack,BeforeAttack 등 아티팩트 효과 적용 타이밍을 CardSystemController보다 먼저 할지 뒤에 할지 통일해야 함.
    //현재 위 세 개는 순서 보장이 안됨.
    //beforeCardUsePhase는 CardSystemController가 먼저 실행하고, AfterCardUsePhase는 AfritfactManager가 먼저 실행함.
    public void Initialize()
    {
        ReadyArtifactPools();
    }

    private void ReadyArtifactPools()
    {
        for (int i = 0; i < artifactDataBase.artifactDatas.Count; ++i)
        {
            ArtifactData artifactData = artifactDataBase.GetArtifactData(i);

            ObjectPool<Artifact> pool = new ObjectPool<Artifact>(
                createFunc: () =>
                {
                    Artifact instance = new Artifact();
                    instance.Initialize(artifactData);
                    return instance;
                },
                actionOnGet: artifact =>
                {
                    artifact.Reset();
                },
                actionOnRelease: artifact =>
                {
                    artifact.Reset();
                },
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: SYSTEM_VAR.maxDeckPileCount,
                maxSize: SYSTEM_VAR.limitDeckPileCount
            );

            artifactPools.Add((int)artifactData.type, pool);
        }
    }

    public void Release()
    {

    }

    public void AddArtifact(ArtifactType artifactType)
    {
        ArtifactData artifactData = artifactDataBase.GetArtifactData((int)artifactType);
        if (artifactData == null)
            return;

        ObjectPool<Artifact> pool = artifactPools[(int)artifactData.type];

        Artifact artifact = pool.Get();

        equippedArtifacts.Add(artifact);
    }

    public void RemoveArtifact(Artifact _artifact)
    {
        equippedArtifacts.Remove(_artifact);
    }


    public void EquipDefaultArtifact(ICharacterData _characterData)
    {
        if (_characterData.characterType == CharacterType.Rumy)
        {
            AddArtifact(ArtifactType.RumysSatellite);
        }
    }

    public void PlayerTurnStarted()
    {
        OrganizeCardEffectCommand(GameSystemActionTimingType.BeforeTurn);

        DispatchCardEffect_BeforeTurn();
    }

    public void CardUsePhaseStarted()
    {
        OrganizeCardEffectCommand(GameSystemActionTimingType.BeforeCardUsingPhase);

        DispatchCardEffect_BeforeCardUsingPhase();
    }

    public void AfterCardUsePhaseStarted()
    {
        OrganizeCardEffectCommand(GameSystemActionTimingType.AfterCardUsingPhase);

        DispatchCardEffect_AfterCardUsingPhase();

        ArtifactAppliedEvent_AfterCardUsingPhase?.Invoke();
    }

    private void OrganizeCardEffectCommand(GameSystemActionTimingType _type)
    {
        for (int i = 0; i < equippedArtifacts.Count; ++i)
        {
            Artifact artifact = equippedArtifacts[i];

            //OCP 위반.
            List<ArtifactCommand> cardLogicSystemEffects = artifact.GetcardLogicSystemEffects();
            List<ArtifactCommand> cardDataControlSystemEffects = artifact.GetcardDataControlSystemEffects();
            List<ArtifactCommand> cardStatusEffects = artifact.GetcardStatusEffects();
            List<ArtifactCommand> cardSlotSystemEffects = artifact.GetcardSlotSystemEffects();
            List<ArtifactCommand> complexSystemEffects = artifact.GetcomplexSystemEffects();
            List<ArtifactCommand> selectionSystemEffects = artifact.GetselectionSystemEffects();

            for (int j = 0; j < cardStatusEffects.Count; ++j)
            {
                cardStatusEffects[j].InitializeCommand(artifact.IsUpgraded());

                GameSystemActionTimingType timing = cardStatusEffects[j].GetGameSystemActionTimingType();

                if (timing == _type)
                    InsertCommandToList(timing, cardStatusEffects[j]);
            }

            for (int j = 0; j < cardLogicSystemEffects.Count; ++j)
            {
                cardLogicSystemEffects[j].InitializeCommand(artifact.IsUpgraded());

                GameSystemActionTimingType timing = cardLogicSystemEffects[j].GetGameSystemActionTimingType();

                if (timing == _type)
                    InsertCommandToList(timing, cardLogicSystemEffects[j]);
            }

            for (int j = 0; j < cardSlotSystemEffects.Count; ++j)
            {
                cardSlotSystemEffects[j].InitializeCommand(artifact.IsUpgraded());

                GameSystemActionTimingType timing = cardSlotSystemEffects[j].GetGameSystemActionTimingType();

                if (timing == _type)
                    InsertCommandToList(timing, cardSlotSystemEffects[j]);
            }

            for (int j = 0; j < complexSystemEffects.Count; ++j)
            {
                complexSystemEffects[j].InitializeCommand(artifact.IsUpgraded());

                GameSystemActionTimingType timing = complexSystemEffects[j].GetGameSystemActionTimingType();

                if (timing == _type)
                    InsertCommandToList(timing, complexSystemEffects[j]);
            }

            for (int j = 0; j < selectionSystemEffects.Count; ++j)
            {
                selectionSystemEffects[j].InitializeCommand(artifact.IsUpgraded());

                GameSystemActionTimingType timing = selectionSystemEffects[j].GetGameSystemActionTimingType();

                if (timing == _type)
                    InsertCommandToList(timing, selectionSystemEffects[j]);
            }
        }
    }

    private void InsertCommandToList(GameSystemActionTimingType timingType, ArtifactCommand command)
    {
        if (timingType == GameSystemActionTimingType.BeforeAttack)
        {
            artifactEffect_BeforeAttack.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.AfterAttack)
        {
            artifactEffect_AfterAttack.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.BeforeTurn)
        {
            artifactEffect_BeforeTurn.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.BeforeCardUsingPhase)
        {
            artifactEffect_BeforeCardUsingPhase.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.AfterCardUsingPhase)
        {
            artifactEffect_AfterCardUsingPhase.Add(command);
        }
    }

    private void DispatchCardEffect_BeforeTurn()
    {
        //OCP 위반.
        for (int i = 0; i < artifactEffect_BeforeTurn.Count; ++i)
        {
            var command = artifactEffect_BeforeTurn[i];

            AfrifactCommandDispatchEvent?.Invoke(command, command.GetEffectApplyType(), false);
        }

        artifactEffect_BeforeTurn.Clear();
    }

    private void DispatchCardEffect_BeforeAttack()
    {
        for (int i = 0; i < artifactEffect_BeforeAttack.Count; ++i)
        {
            var command = artifactEffect_BeforeAttack[i];

            AfrifactCommandDispatchEvent?.Invoke(command, command.GetEffectApplyType(), false);
        }

        artifactEffect_BeforeAttack.Clear();
    }

    private void DispatchCardEffect_AfterAttack()
    {
        for (int i = 0; i < artifactEffect_AfterAttack.Count; ++i)
        {
            var command = artifactEffect_AfterAttack[i];

            AfrifactCommandDispatchEvent?.Invoke(command, command.GetEffectApplyType(), false);
        }

        artifactEffect_AfterAttack.Clear();
    }

    private void DispatchCardEffect_AfterCardUsingPhase()
    {
        for (int i = 0; i < artifactEffect_AfterCardUsingPhase.Count; ++i)
        {
            var command = artifactEffect_AfterCardUsingPhase[i];

            AfrifactCommandDispatchEvent?.Invoke(command, command.GetEffectApplyType(), false);
        }

        artifactEffect_AfterCardUsingPhase.Clear();
    }

    private void DispatchCardEffect_BeforeCardUsingPhase()
    {
        for (int i = 0; i < artifactEffect_BeforeCardUsingPhase.Count; ++i)
        {
            var command = artifactEffect_BeforeCardUsingPhase[i];

            AfrifactCommandDispatchEvent?.Invoke(command, command.GetEffectApplyType(), false);
        }

        artifactEffect_BeforeCardUsingPhase.Clear();
    }
}
