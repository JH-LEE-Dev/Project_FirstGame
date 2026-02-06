using CardSystemSignals;
using CardSystemUISignal;
using GameControlSignals;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using UnityEngine;
using UnityEngine.UIElements;
using WaveSystemSignals;

public class GameplayUICoordinator
{
    public event Action<int> UnEquipBulletCardEvent;
    public event Action CancelCardPreviewEvent;
    public event Action<bool, int, Transform> CardUsedEvent;

    private UIView_HUD hudUISystem;
    private UIView_Unit_World unitWorldUISystem;
    private UIView_Unit_Canvas unitCanvasUISystem;
    private UIView_Gameplay gameplayUISystem;

    public void Initialize(UIView_HUD _hudUISystem, UIView_Unit_World _unitWorldUISystem,
        UIView_Gameplay _gameplayUISystem,UIView_Unit_Canvas _unitCanvasUISystem)
    {
        hudUISystem = _hudUISystem;
        unitWorldUISystem = _unitWorldUISystem;
        gameplayUISystem = _gameplayUISystem;
        unitCanvasUISystem = _unitCanvasUISystem;

        BindEvents();
    }

    private void BindEvents()
    {
        unitWorldUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;
        unitWorldUISystem.UnEquipBulletCardEvent += UnEquipBulletCard;

        unitWorldUISystem.CancelCardPreviewEvent -= CancelCardPreview;
        unitWorldUISystem.CancelCardPreviewEvent += CancelCardPreview;
    }

    private void ReleaseEvents()
    {
        unitWorldUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;

        unitWorldUISystem.CancelCardPreviewEvent -= CancelCardPreview;
    }

    public void Release()
    {
        ReleaseEvents();
    }

    public void PlayerTurnStarted()
    {
        hudUISystem.PlayerTurnStarted();
    }

    public void EnemyTurnStarted()
    {
        hudUISystem.EnemyTurnStarted();
    }

    public void PlayerAttacked()
    {
        unitWorldUISystem.UnEquipBulletCardForShoot();
        gameplayUISystem.PlayerAttackFinished();
    }

    public void CardUsingFinished()
    {
        gameplayUISystem.CardUsingFinished();
    }

    public void OnPlayerHit(float damage)
    {
        hudUISystem.OnPlayerHit(damage);
    }

    public void WaveStarted(int waveIdx)
    {
        hudUISystem.WaveStarted(waveIdx);
    }

    public void GameStarted()
    {
        hudUISystem.GameStarted();
    }

    public void WaveEnded()
    {
        hudUISystem.WaveEnded();
        unitCanvasUISystem.WaveEnded();
    }

    public void EnemyIsDead(Vector2 position)
    {
        hudUISystem.EnemyIsDead(position);
    }

    public void PlayerGetShield(float amount)
    {
        hudUISystem.PlayerGetShield(amount);
    }

    public void PlayerGetHP(float amount)
    {
        hudUISystem.PlayerGetHP(amount);
    }

    public void CardSlotCntChanged(int cnt)
    {
        unitWorldUISystem.SetBulletSocketCount(cnt);
    }

    public void EquipBulletCard(int slotIdx, ICardDataInstanceProvider equippedCard)
    {
        unitWorldUISystem.EquipBulletCard(slotIdx, equippedCard);
    }

    private void UnEquipBulletCard(int slotIdx)
    {
        UnEquipBulletCardEvent?.Invoke(slotIdx);
    }

    private void CancelCardPreview()
    {
        CancelCardPreviewEvent?.Invoke();
    }

    public void CardUsed(bool bVerified,int slotIdx)
    {
        Transform slotTransform = null;

        if (bVerified == true)
            slotTransform = unitWorldUISystem.GetSocketTransform(slotIdx);

        CardUsedEvent?.Invoke(bVerified, slotIdx, slotTransform);
    }

    public void EnemyTakeDamage(IEnemyData enemyData,float damage,bool bCritical)
    {
        unitWorldUISystem.EnemyTakeDamage(enemyData, damage,bCritical);
    }

    public void EnemyIsKilled(IEnemyData _enemyData)
    {
        hudUISystem.EnemyIsKilled(_enemyData.GetTransform().position);
    }

    public void PlayerEarnMoney(int amount)
    {
        hudUISystem.PlayerEarnMoney(amount);
    }

    public void WaveRewardReceived(int amount)
    {
        hudUISystem.WaveRewardRecieved(amount);
    }

    public void ResetPlayerShield()
    {
        hudUISystem.ResetPlayerShield();
    }

    public void CharacterStatChanged()
    {
        hudUISystem.CharacterStatChanged();
    }

    public void AdditionalEnemySpawned(IReadOnlyList<IEnemyData> enemyDatas)
    {
        unitCanvasUISystem.AdditionalEnemySpawned(enemyDatas);
    }

    public void PlayerIsDead()
    {

    }
}
