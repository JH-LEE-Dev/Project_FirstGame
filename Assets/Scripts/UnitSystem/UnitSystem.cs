using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System.Collections.Generic;
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
        unitSpawner = _unitSpawner;
        unitLogicSystem = _unitLogicSystem;

        BindEvents();
        SubscribeEvents();
    }

    private void BindEvents()
    {
        unitSpawner.PlayerCreatedEvent -= PlayerCreated;
        unitSpawner.PlayerCreatedEvent += PlayerCreated;
        unitSpawner.CharacterCreatedEvent -= CharacterCreated;
        unitSpawner.CharacterCreatedEvent += CharacterCreated;
        unitSpawner.EnemyCreatedEvent -= EnemyCreated;
        unitSpawner.EnemyCreatedEvent += EnemyCreated;
    }

    private void SubscribeEvents()
    {

    }

    private void UnsubscribeEvents()
    {

    }

    private void ReleaseEvents()
    {
        unitSpawner.PlayerCreatedEvent -= PlayerCreated;
        unitSpawner.CharacterCreatedEvent -= CharacterCreated;
        unitSpawner.EnemyCreatedEvent -= EnemyCreated;
    }

    public void Release()
    {
        ReleaseEvents();
        UnsubscribeEvents();
    }

    private void PlayerCreated(Earth _earth)
    {
        unitLogicSystem.PlayerCreated(_earth);
    }

    private void CharacterCreated(Character _character)
    {
        unitLogicSystem.CharacterCreated(_character);
    }

    private void EnemyCreated(List<Enemy> _enemies)
    {
        unitLogicSystem.EnemyCreated(_enemies);
    }
}
