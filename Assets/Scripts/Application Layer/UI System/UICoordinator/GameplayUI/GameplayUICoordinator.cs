using CardSystemSignals;
using GameControlSignals;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;
using CardSystemUISignal;
using System;
using UnityEngine;

public class GameplayUICoordinator
{
    public event Action<int> UnEquipBulletCardEvent;
    public event Action CancelCardPreviewEvent;
    public event Action<bool, int, Transform> CardUsedEvent;

    private UIView_HUD hudUISystem;
    private UIView_Unit unitUISystem;
    private UIView_Gameplay gameplayUISystem;

    public void Initialize(UIView_HUD _hudUISystem, UIView_Unit _unitUISystem,UIView_Gameplay _gameplayUISystem)
    {
        hudUISystem = _hudUISystem;
        unitUISystem = _unitUISystem;
        gameplayUISystem = _gameplayUISystem;

        BindEvents();
    }

    private void BindEvents()
    {
        unitUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;
        unitUISystem.UnEquipBulletCardEvent += UnEquipBulletCard;

        unitUISystem.CancelCardPreviewEvent -= CancelCardPreview;
        unitUISystem.CancelCardPreviewEvent += CancelCardPreview;
    }

    private void ReleaseEvents()
    {
        unitUISystem.UnEquipBulletCardEvent -= UnEquipBulletCard;

        unitUISystem.CancelCardPreviewEvent -= CancelCardPreview;
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
        gameplayUISystem.EnemyTurnStarted();
    }

    public void PlayerAttacked()
    {
        unitUISystem.UnEquipBulletCardForShoot();
    }

    public void CardUseTimeStarted()
    {
        hudUISystem.CardUseTimeStarted();
    }

    public void PlayerSpawned(IPlayerData playerData)
    {
        hudUISystem.PlayerSpawned(playerData);
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
    public void CharacterSpawned(ICharacterData characterData)
    {
        unitUISystem.Initialize(characterData);
    }

    public void WaveEnded()
    {
        hudUISystem.WaveEnded();
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
        unitUISystem.SetBulletSocketCount(cnt);
    }

    public void EquipBulletCard(int slotIdx,CardDataInstance equippedCard)
    {
        unitUISystem.EquipBulletCard(slotIdx, equippedCard);
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
            slotTransform = unitUISystem.GetSocketTransform(slotIdx);

        CardUsedEvent?.Invoke(bVerified, slotIdx, slotTransform);
    }
}
