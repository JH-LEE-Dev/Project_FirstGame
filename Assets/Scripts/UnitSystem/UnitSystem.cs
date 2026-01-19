using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using UnityEngine;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;

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
    }

    private void SubscribeEvents()
    {
        //원래는 UnitSystem이 SpawnWave함수를 정의하여 unitSpawner로 Forwarding해야 함. (unitSpawner와 이벤트의 디커플링)
        //하지만 편의성을 위해서 임시적으로 함수를 다이렉트 연결.
        signalHub.Subscribe<SpawnWaveEvent>(unitSpawner.SpawnWave);
        signalHub.Subscribe<AllEnemyDeadEvent>(unitSpawner.ResetCurrentEnemies);
        signalHub.Subscribe<CardEffectStatusCommandDispatchEvent>(unitLogicSystem.InsertCommand);
        signalHub.Subscribe<EnemyTurnStartEvent>(unitLogicSystem.EnemyTurnStarted);
        signalHub.Subscribe<CardUsingTurnFinishedEvent>(unitLogicSystem.CardUsingTurnFinished);
        signalHub.Subscribe<CardDrawStartEvent>(unitLogicSystem.CardDrawed);
        signalHub.Subscribe<StartMoveEvent>(unitLogicSystem.StartEnemyMove);
        signalHub.Subscribe<GameStartedEvent>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.Subscribe<TryCardUseEvent>(TryCardUse);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<SpawnWaveEvent>(unitSpawner.SpawnWave);
        signalHub.UnSubscribe<AllEnemyDeadEvent>(unitSpawner.ResetCurrentEnemies);
        signalHub.UnSubscribe<CardEffectStatusCommandDispatchEvent>(unitLogicSystem.InsertCommand);
        signalHub.UnSubscribe<EnemyTurnStartEvent>(unitLogicSystem.EnemyTurnStarted);
        signalHub.UnSubscribe<CardUsingTurnFinishedEvent>(unitLogicSystem.CardUsingTurnFinished);
        signalHub.UnSubscribe<CardDrawStartEvent>(unitLogicSystem.CardDrawed);
        signalHub.UnSubscribe<StartMoveEvent>(unitLogicSystem.StartEnemyMove);
        signalHub.UnSubscribe<GameStartedEvent>(unitLogicSystem.ActivatePlayerAndCharacter);
        signalHub.UnSubscribe<TryCardUseEvent>(TryCardUse);
    }

    private void EnemySpawned()
    {
        signalHub.Publish(new EnemySpawnedEvent());
    }

    private void CharacterSpawend(Character character)
    {
        signalHub.Publish(new CharacterSpawnedEvent(character));
    }

    private void PlayerSpawned(Earth player)
    {
        signalHub.Publish(new PlayerSpawnedEvent(player));
    }

    private void EnemyIsDead(Vector2 position)
    {
        signalHub.Publish(new EnemyIsDeadEvent(position));
    }

    private void PlayerTurnFinished()
    {
        signalHub.Publish(new PlayerTurnFinishedEvent());
    }

    private void PlayerTakeDamage(float damage)
    {
        signalHub.Publish(new PlayerTakeDamageEvent(damage));
    }

    private void TryCardUse(TryCardUseEvent tryCardUseEvent)
    {
        unitLogicSystem.CanApplyBulletEffect(tryCardUseEvent.usedCard);
    }

    public void Release()
    {
        ReleaseEvents();
        UnSubscribeEvents();
    }
}
