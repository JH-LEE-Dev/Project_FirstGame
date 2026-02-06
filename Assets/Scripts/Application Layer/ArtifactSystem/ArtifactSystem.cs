using EffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using UnitSpawnSystemSignals;
using UnityEngine;

public class ArtifactSystem
{
    //외부 의존성
    private ArtifactManager artifactManager;
    private SignalHub signalHub;

    public void Initialize(ArtifactManager _artifactManager,SignalHub _signalHub)
    {
        artifactManager = _artifactManager;
        signalHub = _signalHub;

        SubscribeSignals();

        BindEvents();
    }
    

    public void Release()
    {
        UnsubscribeSignals();

        ReleaseEvents();
    }

    private void SubscribeSignals()
    {
        signalHub.Subscribe<CharacterCreatedSignal>(CharacterSpawned);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardUsePhaseStartedSignal>(CardUsePhaseStarted);
        signalHub.Subscribe<AfterCardUsePhaseStartedSignal>(AfterCardUsePhaseStarted);
    }

    private void UnsubscribeSignals()
    {
        signalHub.UnSubscribe<CharacterCreatedSignal>(CharacterSpawned);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardUsePhaseStartedSignal>(CardUsePhaseStarted);
        signalHub.UnSubscribe<AfterCardUsePhaseStartedSignal>(AfterCardUsePhaseStarted);
    }

    private void BindEvents()
    {
        artifactManager.AfrifactCommandDispatchEvent -= DispatchAftifactCommand;
        artifactManager.AfrifactCommandDispatchEvent += DispatchAftifactCommand;
    }

    private void ReleaseEvents()
    {
        artifactManager.AfrifactCommandDispatchEvent -= DispatchAftifactCommand;
    }

    private void CharacterSpawned(CharacterCreatedSignal characterCreatedSignal)
    {
        artifactManager.EquipDefaultArtifact(characterCreatedSignal.characterData);
    }

    private void PlayerTurnStarted(PlayerTurnStartSignal playerTurnStartSignal)
    {
        artifactManager.PlayerTurnStarted();
    }

    private void CardUsePhaseStarted(CardUsePhaseStartedSignal cardUsePhaseStarted)
    {
        artifactManager.CardUsePhaseStarted();
    }

    private void AfterCardUsePhaseStarted(AfterCardUsePhaseStartedSignal afterCardUsePhaseStarted)
    {
        artifactManager.AfterCardUsePhaseStarted();
    }

    private void DispatchAftifactCommand(ArtifactCommand _command,EffectApplyType _type, bool bUndo)
    {
        if(_type == EffectApplyType.StatusSystem)
        {
            signalHub.Publish(new CardStatusEffectCommandDispatchSignal(_command,bUndo));
        }
        else
        {
            signalHub.Publish(new ArtifactEffectCommandDispatchSignal(_command, bUndo, _type));
        }
    }
}
