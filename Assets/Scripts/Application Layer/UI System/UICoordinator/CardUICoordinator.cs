using CardSystemSignals;
using GameControlSignals;
using UICommandSystemSignals;
using UnitSpawnSystemSignals;
using UnityEngine;
using CardSystemUISignal;
using UnitLogicSystemSignals;

public class CardUICoordinator
{
    //외부 의존성
    SignalHub signalHub;


    private UIView_CardSystem cardUISystem;
    private UIView_Unit unitUISystem;

    public void Initialize(SignalHub _signalHub,UIView_CardSystem _cardUISystem,UIView_Unit _unitUISystem)
    {
        signalHub = _signalHub;
        cardUISystem = _cardUISystem;
        unitUISystem = _unitUISystem;

        BindEvent();

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedSignal>(CardDrawFinished);
        signalHub.Subscribe<CharacterSpawnedSignal>(CharacterSpawned);
        signalHub.Subscribe<CardSystem_JobDispatchSignal>(RecieveUIJob);
        signalHub.Subscribe<CardUsedSignal>(CardUsed);
        signalHub.Subscribe<PlayerAttackedSignal>(PlayerAttacked);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedSignal>(CardDrawFinished);
        signalHub.UnSubscribe<CharacterSpawnedSignal>(CharacterSpawned);
        signalHub.UnSubscribe<CardSystem_JobDispatchSignal>(RecieveUIJob);
        signalHub.UnSubscribe<CardUsedSignal>(CardUsed);
        signalHub.UnSubscribe<PlayerAttackedSignal>(PlayerAttacked);
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvent();
    }

    private void UICommandComplete(int idx)
    {
        signalHub.Publish(new  UICommandCompleteSignal(idx));
    }

    private void BindEvent()
    {
        cardUISystem.TryCardUseEvent -= TryCardUse;
        cardUISystem.TryCardUseEvent += TryCardUse;

        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
        cardUISystem.CardUsingFinishedEvent += CardUsingFinished;

        cardUISystem.UICommandCompleteEvent -= UICommandComplete;
        cardUISystem.UICommandCompleteEvent += UICommandComplete;

        cardUISystem.CardEquippedEvent -= unitUISystem.EquipBulletCard;
        cardUISystem.CardEquippedEvent += unitUISystem.EquipBulletCard;

        unitUISystem.UnEquipBulletCardEvent -= cardUISystem.UnEquipBulletCard;
        unitUISystem.UnEquipBulletCardEvent += cardUISystem.UnEquipBulletCard;

        unitUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;
        unitUISystem.UnEquipBulletCardEvent += UnEquipBulletCard;

        unitUISystem.CancelCardPreviewEvent -= cardUISystem.CancelPreview;
        unitUISystem.CancelCardPreviewEvent += cardUISystem.CancelPreview;
    }

    public void CharacterSpawned(CharacterSpawnedSignal characterSpawnedSignal)
    {
        unitUISystem.Initialize(characterSpawnedSignal.characterData);
    }

    private void ReleaseEvent()
    {
        cardUISystem.UICommandCompleteEvent -= UICommandComplete;

        cardUISystem.TryCardUseEvent -= TryCardUse;

        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;

        cardUISystem.CardEquippedEvent -= unitUISystem.EquipBulletCard;

        unitUISystem.UnEquipBulletCardEvent -= cardUISystem.UnEquipBulletCard;

        unitUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;

        unitUISystem.CancelCardPreviewEvent -= cardUISystem.CancelPreview;
    }

    public void TryCardUse(CardDataInstance usedCard)
    {
        signalHub.Publish(new TryCardUseSignal(usedCard));
    }

    public void CardDrawFinished(CardDrawFinishedSignal cardDrawFinishedSignal)
    {
        cardUISystem.CardDrawFinished();
    }

    public void CardUsingFinished()
    {
        signalHub.Publish(new CardUsingFinishedSignal());
    }

    public void CardUsed(CardUsedSignal cardUsedSignal)
    {
        Transform slotTransform = null;

        if(cardUsedSignal.bVerified == true)
            slotTransform = unitUISystem.GetSocketTransform(cardUsedSignal.slotIdx);

        cardUISystem.CardUsingApproved(cardUsedSignal.bVerified, cardUsedSignal.slotIdx, slotTransform);
    }

    public void RecieveUIJob(CardSystem_JobDispatchSignal cardSystem_JobDispatchSignal)
    {
        cardUISystem.RecieveUIJob(cardSystem_JobDispatchSignal.actionDataBatch);
    }

    public void EnemyTurnStarted(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        cardUISystem.EnemyTurnStarted();
    }

    public void PlayerTurnStarted(PlayerTurnStartSignal playerTurnStartSignal)
    {
        cardUISystem.PlayerTurnStarted();
    }

    public void UnEquipBulletCard(int idx)
    {
        signalHub.Publish(new DiscardBulletCardSignal(idx));
    }

    private void PlayerAttacked(PlayerAttackedSignal playerAttackedSignal)
    {
        unitUISystem.UnEquipBulletCardForShoot();
    }
}
