using EffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using UnityEngine;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;
using CardSystemUISignal;
using System.Collections.Generic;
using ShopSystemUISignals;

public class UnitSystem
{
    //외부 의존성
    private SignalHub signalHub;
    private UnitSpawner unitSpawner;
    private UnitLogicSystem unitLogicSystem;

    public void Initialize(SignalHub _signalHub, UnitSpawner _unitSpawner, UnitLogicSystem _unitLogicSystem)
    {
        signalHub = _signalHub;
        unitSpawner = _unitSpawner;
        unitLogicSystem = _unitLogicSystem;

        BindEvents();
        SubscribeEvents();
    }

    private void BindEvents()
    {
        unitSpawner.PlayerCreatedEvent -= unitLogicSystem.PlayerCreated;
        unitSpawner.PlayerCreatedEvent += unitLogicSystem.PlayerCreated;

        unitSpawner.CharacterCreatedEvent -= CharacterCreated;
        unitSpawner.CharacterCreatedEvent += CharacterCreated;

        unitSpawner.EnemyCreatedEvent -= unitLogicSystem.EnemyCreated;
        unitSpawner.EnemyCreatedEvent += unitLogicSystem.EnemyCreated;

        unitLogicSystem.EnemyIsDeadEvent -= EnemyIsDead;
        unitLogicSystem.EnemyIsDeadEvent += EnemyIsDead;

        unitLogicSystem.PlayerTurnFinishedEvent -= PlayerTurnFinished;
        unitLogicSystem.PlayerTurnFinishedEvent += PlayerTurnFinished;

        unitLogicSystem.PlayerTakeDamageEvent -= PlayerTakeDamage;
        unitLogicSystem.PlayerTakeDamageEvent += PlayerTakeDamage;

        unitLogicSystem.PlayerAttackedEvent -= PlayerAttacked;
        unitLogicSystem.PlayerAttackedEvent += PlayerAttacked;

        unitLogicSystem.PlayerGetShieldEvent -= PlayerGetShield;
        unitLogicSystem.PlayerGetShieldEvent += PlayerGetShield;

        unitLogicSystem.PlayerGetHPEvent -= PlayerGetHP;
        unitLogicSystem.PlayerGetHPEvent += PlayerGetHP;

        unitLogicSystem.EnemyTakeDamageEvent -= EnemyTakeDamage;
        unitLogicSystem.EnemyTakeDamageEvent += EnemyTakeDamage;

        unitLogicSystem.EnemyIsKilledEvent -= EnemyIsKilled;
        unitLogicSystem.EnemyIsKilledEvent += EnemyIsKilled;

        unitLogicSystem.CharacterStatChangedEvent -= CharacterStatChanged;
        unitLogicSystem.CharacterStatChangedEvent += CharacterStatChanged;

        unitSpawner.AdditionalEnemySpawnedEvent -= AdditionalEnemySpawned;
        unitSpawner.AdditionalEnemySpawnedEvent += AdditionalEnemySpawned;

        unitLogicSystem.PlayerIsDeadEvent -= PlayerIsDead;
        unitLogicSystem.PlayerIsDeadEvent += PlayerIsDead;

        unitLogicSystem.CharacterReadyToAttackEvent -= CharacterReadyToAttack;
        unitLogicSystem.CharacterReadyToAttackEvent += CharacterReadyToAttack;
    }

    private void ReleaseEvents()
    {
        unitSpawner.PlayerCreatedEvent -= unitLogicSystem.PlayerCreated;

        unitSpawner.CharacterCreatedEvent -= CharacterCreated;

        unitSpawner.EnemyCreatedEvent -= unitLogicSystem.EnemyCreated;

        unitLogicSystem.EnemyIsDeadEvent -= EnemyIsDead;

        unitLogicSystem.PlayerTurnFinishedEvent -= PlayerTurnFinished;

        unitLogicSystem.PlayerTakeDamageEvent -= PlayerTakeDamage;

        unitLogicSystem.PlayerAttackedEvent -= PlayerAttacked;

        unitLogicSystem.PlayerGetShieldEvent -= PlayerGetShield;

        unitLogicSystem.PlayerGetHPEvent -= PlayerGetHP;

        unitLogicSystem.EnemyTakeDamageEvent -= EnemyTakeDamage;

        unitLogicSystem.EnemyIsKilledEvent -= EnemyIsKilled;

        unitLogicSystem.CharacterStatChangedEvent -= CharacterStatChanged;

        unitSpawner.AdditionalEnemySpawnedEvent -= AdditionalEnemySpawned;

        unitLogicSystem.PlayerIsDeadEvent -= PlayerIsDead;

        unitLogicSystem.CharacterReadyToAttackEvent -= CharacterReadyToAttack;
    }

    private void SubscribeEvents()
    {
        //원래는 UnitSystem이 SpawnWave함수를 정의하여 unitSpawner로 Forwarding해야 함. (unitSpawner와 이벤트의 디커플링)
        //하지만 편의성을 위해서 임시적으로 함수를 다이렉트 연결.
        signalHub.Subscribe<SpawnWaveSignal>(unitSpawner.SpawnWave);
        signalHub.Subscribe<AllEnemyDeadSignal>(unitSpawner.ResetCurrentEnemies);
        signalHub.Subscribe<CardStatusEffectCommandDispatchSignal>(unitLogicSystem.ExecuteCommand);
        signalHub.Subscribe<EnemyTurnStartSignal>(unitLogicSystem.EnemyTurnStarted);
        signalHub.Subscribe<CardUsingFinishedSignal>(unitLogicSystem.CardUsingFinished);
        signalHub.Subscribe<CardUsePhaseStartedSignal>(unitLogicSystem.CardUsePhaseStarted);
        signalHub.Subscribe<StartMoveSignal>(unitLogicSystem.StartEnemyMove);
        signalHub.Subscribe<GameStartedSignal>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStart);
        signalHub.Subscribe<ShopTimeStartedSignal>(ShopTimeStarted);
        signalHub.Subscribe<ShopBillingSignal>(PlayerMoneyUsed);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<SpawnWaveSignal>(unitSpawner.SpawnWave);
        signalHub.UnSubscribe<AllEnemyDeadSignal>(unitSpawner.ResetCurrentEnemies);
        signalHub.UnSubscribe<CardStatusEffectCommandDispatchSignal>(unitLogicSystem.ExecuteCommand);
        signalHub.UnSubscribe<EnemyTurnStartSignal>(unitLogicSystem.EnemyTurnStarted);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(unitLogicSystem.CardUsingFinished);
        signalHub.UnSubscribe<CardUsePhaseStartedSignal>(unitLogicSystem.CardUsePhaseStarted);
        signalHub.UnSubscribe<StartMoveSignal>(unitLogicSystem.StartEnemyMove);
        signalHub.UnSubscribe<GameStartedSignal>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStart);
        signalHub.UnSubscribe<ShopTimeStartedSignal>(ShopTimeStarted);
        signalHub.UnSubscribe<ShopBillingSignal>(PlayerMoneyUsed);
    }

    private void EnemyIsDead(Vector2 position)
    {
        signalHub.Publish(new EnemyIsDeadSignal(position));
    }

    private void PlayerTurnFinished()
    {
        signalHub.Publish(new PlayerAttackFinishedSignal());
    }

    private void PlayerTakeDamage(float damage)
    {
        signalHub.Publish(new PlayerTakeDamageSignal(damage));
    }

    private void PlayerAttacked()
    {
        signalHub.Publish(new PlayerAttackedSignal());
    }

    private void PlayerGetShield(float amount)
    {
        signalHub.Publish(new PlayerGetShieldSignal(amount));
    }

    private void PlayerGetHP(float amount)
    {
        signalHub.Publish(new PlayerGetHPSignal(amount));
    }

    private void EnemyTakeDamage(IEnemyData enemyData, float damage, bool bCritical)
    {
        signalHub.Publish(new EnemyTakeDamageSignal(enemyData, damage, bCritical));
    }

    private void EnemyIsKilled(IEnemyData _enemyData)
    {
        signalHub.Publish(new EnemyIsKilledSignal(_enemyData));
        PlayerEarnMoney(_enemyData.enemyTypeData.rewardWhenKilled);
    }

    public void Release()
    {
        ReleaseEvents();
        UnSubscribeEvents();
    }

    public void PlayerTurnStart(PlayerTurnStartSignal playerTurnStartSignal)
    {
        unitLogicSystem.ResetPlayerShield();
        signalHub.Publish(new ResetPlayerShieldSignal());
    }

    private void CharacterStatChanged()
    {
        signalHub.Publish(new CharacterStatChangedSignal());
    }

    private void ShopTimeStarted(ShopTimeStartedSignal shopTimeStartedSignal)
    {
        unitLogicSystem.ResetPlayerShield();
        signalHub.Publish(new ResetPlayerShieldSignal());
    }

    private void AdditionalEnemySpawned(IReadOnlyList<IEnemyData> enemyDatas)
    {
        signalHub.Publish(new AdditionalEnemySpawnedSignal(enemyDatas));
    }

    private void CharacterCreated(Character _characterUnit)
    {
        unitLogicSystem.CharacterCreated(_characterUnit);
        signalHub.Publish(new CharacterCreatedSignal(_characterUnit));
    }

    private void PlayerIsDead()
    {
        unitSpawner.ReleaseAllEnemy();
        signalHub.Publish(new PlayerIsDeadSignal());
    }

    private void PlayerEarnMoney(int amount)
    {
        signalHub.Publish(new PlayerEarnMoneySignal(amount));
    }

    private void PlayerMoneyUsed(ShopBillingSignal shopBillingSignal)
    {
        unitLogicSystem.PlayerMoneyUsed(shopBillingSignal.usedMoney); 
    }

    private void CharacterReadyToAttack()
    {
        signalHub.Publish(new CharacterReadyToAttackSignal());
    }
}
