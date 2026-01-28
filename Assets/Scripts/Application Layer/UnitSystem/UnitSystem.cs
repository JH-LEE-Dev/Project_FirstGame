using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using UnityEngine;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;
using CardSystemUISignal;

public class UnitSystem
{
    //외부 의존성
    private SignalHub signalHub;
    private UnitSpawner unitSpawner;
    private UnitLogicSystem unitLogicSystem;

    public void Initialize(SignalHub _signalHub,UnitSpawner _unitSpawner,UnitLogicSystem _unitLogicSystem)
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

        unitSpawner.CharacterCreatedEvent -= unitLogicSystem.CharacterCreated;
        unitSpawner.CharacterCreatedEvent += unitLogicSystem.CharacterCreated;

        unitSpawner.EnemyCreatedEvent -= unitLogicSystem.EnemyCreated;
        unitSpawner.EnemyCreatedEvent += unitLogicSystem.EnemyCreated;

        unitLogicSystem.EnemySpawnedEvent -= EnemySpawned;
        unitLogicSystem.EnemySpawnedEvent += EnemySpawned;

        unitLogicSystem.CharacterSpawendEvent -= CharacterSpawend;
        unitLogicSystem.CharacterSpawendEvent += CharacterSpawend;

        unitLogicSystem.PlayerSpawnedEvent -= PlayerSpawned;
        unitLogicSystem.PlayerSpawnedEvent += PlayerSpawned;

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

    }

    private void ReleaseEvents()
    {
        unitSpawner.PlayerCreatedEvent -= unitLogicSystem.PlayerCreated;

        unitSpawner.CharacterCreatedEvent -= unitLogicSystem.CharacterCreated;

        unitSpawner.EnemyCreatedEvent -= unitLogicSystem.EnemyCreated;

        unitLogicSystem.EnemySpawnedEvent -= EnemySpawned;

        unitLogicSystem.CharacterSpawendEvent -= CharacterSpawend;

        unitLogicSystem.PlayerSpawnedEvent -= PlayerSpawned;

        unitLogicSystem.EnemyIsDeadEvent -= EnemyIsDead;

        unitLogicSystem.PlayerTurnFinishedEvent -= PlayerTurnFinished;

        unitLogicSystem.PlayerTakeDamageEvent -= PlayerTakeDamage;

        unitLogicSystem.PlayerAttackedEvent -= PlayerAttacked;

        unitLogicSystem.PlayerGetShieldEvent -= PlayerGetShield;

        unitLogicSystem.PlayerGetHPEvent -= PlayerGetHP;

        unitLogicSystem.EnemyTakeDamageEvent -= EnemyTakeDamage;
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
        signalHub.Subscribe<CardDrawStartSignal>(unitLogicSystem.CardDrawed);
        signalHub.Subscribe<StartMoveSignal>(unitLogicSystem.StartEnemyMove);
        signalHub.Subscribe<GameStartedSignal>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.Subscribe<WaveStartSignal>(unitLogicSystem.ResetPlayer);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<SpawnWaveSignal>(unitSpawner.SpawnWave);
        signalHub.UnSubscribe<AllEnemyDeadSignal>(unitSpawner.ResetCurrentEnemies);
        signalHub.UnSubscribe<CardStatusEffectCommandDispatchSignal>(unitLogicSystem.ExecuteCommand);
        signalHub.UnSubscribe<EnemyTurnStartSignal>(unitLogicSystem.EnemyTurnStarted);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(unitLogicSystem.CardUsingFinished);
        signalHub.UnSubscribe<CardDrawStartSignal>(unitLogicSystem.CardDrawed);
        signalHub.UnSubscribe<StartMoveSignal>(unitLogicSystem.StartEnemyMove);
        signalHub.UnSubscribe<GameStartedSignal>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.UnSubscribe<WaveStartSignal>(unitLogicSystem.ResetPlayer);
    }

    private void EnemySpawned()
    {
        signalHub.Publish(new EnemySpawnedSignal());
    }

    private void CharacterSpawend(Character character)
    {
        signalHub.Publish(new CharacterSpawnedSignal(character));
    }

    private void PlayerSpawned(Earth player)
    {
        signalHub.Publish(new PlayerSpawnedSignal(player));
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

    private void EnemyTakeDamage(IEnemyData enemyData,float damage)
    {
        signalHub.Publish(new EnemyTakeDamageSignal(enemyData, damage));
    }

    public void Release()
    {
        ReleaseEvents();
        UnSubscribeEvents();
    }
}
