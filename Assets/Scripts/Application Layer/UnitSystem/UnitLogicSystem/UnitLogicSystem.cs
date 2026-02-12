using UnityEngine;
using System.Collections.Generic;
using EffectSystemSignal;
using GameControlSignals;
using CardSystemSignals;
using WaveSystemSignals;
using System;
using CardSystemUISignal;

//이 클래스 책임이 커질 거 같으므로, 컴포넌트로 기능 분할할 것.

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour, IStatusEffectCommandHandler
{
    public event Action<IEnemyData> EnemyIsKilledEvent;
    public event Action EnemySpawnedEvent;
    public event Action<Character> CharacterSpawendEvent;
    public event Action<Earth> PlayerSpawnedEvent;
    public event Action<Vector2> EnemyIsDeadEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action<float> PlayerTakeDamageEvent;
    public event Action PlayerAttackedEvent;
    public event Action<float> PlayerGetShieldEvent;
    public event Action<float> PlayerGetHPEvent;
    public event Action<IEnemyData, float, bool> EnemyTakeDamageEvent;
    public event Action CharacterStatChangedEvent;
    public event Action PlayerIsDeadEvent;
    public event Action<ElementExplosionType> ElementExplosionOccuredEvent;

    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth playerUnit;
    private IReadOnlyList<Enemy> enemyUnits;
    private List<IEnemyHandler> enemyHandlers = new List<IEnemyHandler>(SYSTEM_VAR.maxEnemyCount);
    private ElementExplosionSystem elementExplosionSystem;

    public void Initialize(ElementExplosionSystem _elementExplosionSystem)
    {
        elementExplosionSystem = _elementExplosionSystem;

        BindEvents();
    }

    private void BindEvents()
    {
        elementExplosionSystem.ElementExplosionOccuredEvent -= ElementExplosionOccured;
        elementExplosionSystem.ElementExplosionOccuredEvent += ElementExplosionOccured;
    }

    private void ReleaseEvents()
    {
        elementExplosionSystem.ElementExplosionOccuredEvent -= ElementExplosionOccured;
    }

    public void CharacterCreated(Character _character)
    {
        characterUnit = _character;

        BindEvent_Character();
    }

    public void PlayerCreated(Earth _player)
    {
        playerUnit = _player;

        BindEvent_Player();
    }

    public void EnemyCreated(IReadOnlyList<Enemy> _enemies)
    {
        enemyUnits = _enemies;

        BindEvent_Enemy();

        for(int i = 0;i<enemyUnits.Count;++i)
        {
            enemyHandlers.Add(enemyUnits[i]);
        }

        EnemySpawnedEvent?.Invoke();
    }

    public void ActivatePlayerAndCharacter(GameStartedSignal gameStartedSignal)
    {
        PlayerSpawnedEvent?.Invoke(playerUnit);
        CharacterSpawendEvent?.Invoke(characterUnit);

        characterUnit.gameObject.SetActive(true);
        playerUnit.gameObject.SetActive(true);
    }

    public void Release()
    {
        ReleaseEvents();    
        ReleaseEvent_Character();
        ReleaseEvent_Enemy();
        ReleaseEvent_Player();
    }

    private void BindEvent_Player()
    {
        playerUnit.TakeDamageEvent -= PlayerTakeDamage;
        playerUnit.TakeDamageEvent += PlayerTakeDamage;

        playerUnit.PlayerDeadEvent -= PlayerIsDead;
        playerUnit.PlayerDeadEvent += PlayerIsDead;

        playerUnit.PlayerHitEvent -= PlayerHit;
        playerUnit.PlayerHitEvent += PlayerHit;
    }

    private void ReleaseEvent_Player()
    {
        playerUnit.TakeDamageEvent -= PlayerTakeDamage;

        playerUnit.PlayerDeadEvent -= PlayerIsDead;

        playerUnit.PlayerHitEvent -= PlayerHit;
    }

    private void BindEvent_Character()
    {
        characterUnit.PlayerAttackFinishedEvent -= PlayerTurnFinished;
        characterUnit.PlayerAttackFinishedEvent += PlayerTurnFinished;

        characterUnit.PlayerAttackEvent -= PlayerAttacked;
        characterUnit.PlayerAttackEvent += PlayerAttacked;

        characterUnit.CharacterStatChangedEvent -= CharacterStatChanged;
        characterUnit.CharacterStatChangedEvent += CharacterStatChanged;
    }

    private void ReleaseEvent_Character()
    {
        characterUnit.PlayerAttackFinishedEvent -= PlayerTurnFinished;

        characterUnit.PlayerAttackEvent -= PlayerAttacked;

        characterUnit.CharacterStatChangedEvent -= CharacterStatChanged;
    }

    private void BindEvent_Enemy()
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].UnitIsDeadEvent -= EnemyIsDead;
            enemyUnits[i].UnitIsDeadEvent += EnemyIsDead;

            enemyUnits[i].EnemyIsKilledEvent -= EnemyIsKilled;
            enemyUnits[i].EnemyIsKilledEvent += EnemyIsKilled;

            enemyUnits[i].EnemyTakeDamageEvent -= EnemyTakeDamage;
            enemyUnits[i].EnemyTakeDamageEvent += EnemyTakeDamage;

            enemyUnits[i].EnemyCollideEvent -= EnemyCollide;
            enemyUnits[i].EnemyCollideEvent += EnemyCollide;

            enemyUnits[i].EnemyHitEvent -= EnemyHit;
            enemyUnits[i].EnemyHitEvent += EnemyHit;
        }
    }

    private void ReleaseEvent_Enemy()
    {
        if (enemyUnits != null)
        {
            for (int i = 0; i < enemyUnits.Count; ++i)
            {
                enemyUnits[i].UnitIsDeadEvent -= EnemyIsDead;

                enemyUnits[i].EnemyIsKilledEvent -= EnemyIsKilled;

                enemyUnits[i].EnemyCollideEvent -= EnemyCollide;

                enemyUnits[i].EnemyHitEvent -= EnemyHit;
            }
        }
    }

    private void EnemyIsDead(Unit deadUnit)
    {
        EnemyIsDeadEvent?.Invoke(deadUnit.transform.position);
    }

    private void EnemyIsKilled(IEnemyData _enemyData, EnemyTypeData enemyTypeData)
    {
        playerUnit.EarnMoney(enemyTypeData.rewardWhenKilled);
        EnemyIsKilledEvent?.Invoke(_enemyData);
    }

    public void StartEnemyMove(StartMoveSignal startMoveSignal)
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].OnMove();
        }
    }

    private void PlayerTurnFinished()
    {
        PlayerTurnFinishedEvent?.Invoke();
        playerUnit.PlayerTurnEnd();
    }

    public void EnemyTurnStarted(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        characterUnit.ResetbCanAction();

        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].ResetState();
        }
    }

    public void CardUsePhaseStarted(CardUsePhaseStartedSignal cardUsePhaseStarted)
    {
        characterUnit.PlayerTurnStarted();
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        characterUnit.SetbCanAction();
    }

    public void ExecuteCommand(CardStatusEffectCommandDispatchSignal cardEffectCommandSignal)
    {
        var cardEffectCommand = cardEffectCommandSignal.command;

        if (cardEffectCommandSignal.bUndo == false)
            cardEffectCommand.Execute(this);
        else
            cardEffectCommand.Undo(this);
    }

    public void ApplyShieldModifier(float bonusShield)
    {
        playerUnit.statusEffectReceiver.ApplyShieldModifier(bonusShield);
        PlayerGetShieldEvent?.Invoke(bonusShield);
    }

    public void ApplyAdditionalAttackModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAdditionalAttackModifier(bonusDamage);
        CharacterStatChanged();
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAttackModifier(bonusDamage);
        CharacterStatChanged();
    }

    public void PlayerTakeDamage(float damage)
    {
        PlayerTakeDamageEvent?.Invoke(damage);
    }

    private void PlayerAttacked()
    {
        PlayerAttackedEvent?.Invoke();
    }

    public void ApplyRangeModifier(float bonusRange)
    {
        characterUnit.combatEffectReceiver.ApplyRangeModifier(bonusRange);
        CharacterStatChanged();
    }

    public void ApplyAttackCntModifier(int cnt)
    {
        characterUnit.combatEffectReceiver.ApplyAttackCntModifier(cnt);
        CharacterStatChanged();
    }

    public void HPDecrease(float amount)
    {
        playerUnit.TakeCollideDamage(amount, false,default);
    }

    public void ApplyCriticalChanceModifier(int chance)
    {
        characterUnit.combatEffectReceiver.ApplyCriticalChanceModifier(chance);
        CharacterStatChanged();
    }

    public void ApplyWeaknessModifier(int turnCnt)
    {
        characterUnit.combatEffectReceiver.ApplyWeaknessModifier(turnCnt);
        CharacterStatChanged();
    }

    public void HPIncrease(float amount)
    {
        playerUnit.statusEffectReceiver.IncreaseHP(amount);
        PlayerGetHPEvent?.Invoke(amount);
    }

    private void EnemyTakeDamage(IEnemyData enemyData, float damage, bool bCritical)
    {
        EnemyTakeDamageEvent?.Invoke(enemyData, damage, bCritical);
    }

    public void ResetPlayerShield()
    {
        playerUnit.ResetShield();
    }

    private void CharacterStatChanged()
    {
        CharacterStatChangedEvent?.Invoke();
    }

    public void SetCharacterCanAttackState(bool bCanAttack)
    {
        characterUnit.SetbCanAttack(bCanAttack);
    }

    public void ApplyAdditionalAttackValueModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAdditionalAttackValueModifier(bonusDamage);
        CharacterStatChanged();
    }

    public void ApplyTotalDamageValueModifier(float bonusValue)
    {
        characterUnit.combatEffectReceiver.ApplyTotalDamageValueModifier(bonusValue);
        CharacterStatChanged();
    }

    public void UndoAdditionalAttackValueModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.UndoAdditionalAttackValueModifier(bonusDamage);
        CharacterStatChanged();
    }

    private void PlayerIsDead()
    {
        PlayerIsDeadEvent?.Invoke();
    }

    public void PlayerMoneyUsed(int amount)
    {
        playerUnit.UseMoney(amount);
    }

    public void ApplyBulletElementType(BulletElementData effectElementData)
    {
        characterUnit.bulletEffectReceiver.ApplyBulletElementType(effectElementData);
    }

    public void SetBulletType(BulletType bulletType,bool bUpgraded)
    {
        characterUnit.bulletEffectReceiver.SetBulletType(bulletType, bUpgraded);
    }

    public void ResetBulletType()
    {
        characterUnit.bulletEffectReceiver.ResetBulletType();
    }

    public void UndoBulletElementApply(BulletElementData _effectElementData)
    {
        characterUnit.bulletEffectReceiver.UndoBulletElementApply(_effectElementData);
    }

    public void ApplyDebuffElementType(DebuffElementData _debuffElementData)
    {
        characterUnit.bulletEffectReceiver.ApplyDebuffElementType(_debuffElementData);
    }

    public void UndoDebuffElementApply(DebuffElementData _debuffElementData)
    {
        characterUnit.bulletEffectReceiver.UndoDebuffElementApply(_debuffElementData);
    }

    private void ElementExplosionOccured(ElementExplosionType _type)
    {
        ElementExplosionOccuredEvent?.Invoke(_type);
    }

    public IPlayerHandler GetPlayerHandler()
    {
        return playerUnit;
    }

    public IReadOnlyList<IEnemyHandler> GetEnemyHandlers()
    {
        return enemyHandlers;
    }

    public void EnemyTurnEnd()
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].EnemyTurnEnd();
        }
    }

    public void ApplyAdditionalAttackStat(AdditionalAttackStat _additionalAttackStat)
    {
        characterUnit.combatEffectReceiver.ApplyAdditionalAttackStat(_additionalAttackStat);
    }

    private void PlayerHit(IPlayerData _data, Vector2 pos, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _elements)
    {
        elementExplosionSystem.PlayerCollide(_data, _elements,pos);
    }

    private void EnemyHit(IEnemyData _data,IReadOnlyDictionary<BulletElementType, BulletElementData> _elements,Vector2 pos)
    {
        elementExplosionSystem.EnemyHit(_data, _elements, pos);
    }

    private void EnemyCollide(IEnemyData _data1,IEnemyData _data2, Vector2 pos)
    {
        elementExplosionSystem.EnemyCollide(_data1, _data2, pos);
    }
}
