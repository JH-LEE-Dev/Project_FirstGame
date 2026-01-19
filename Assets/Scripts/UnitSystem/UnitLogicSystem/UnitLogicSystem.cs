using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using CardEffectSystemSignal;
using GameControlSignals;
using CardSystemSignals;
using UnitLogicSystemSignals;
using WaveSystemSignals;
using UnitSpawnSystemSignals;

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour,IUnitLogicSystemActions, IUnitLogicCommandHandler
{
    //외부 의존성
    private SignalHub signalHub;

    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth playerUnit;
    private List<Enemy> enemyUnits;


    private List<CardEffectStatusCommand> cardEffectCommands = new List<CardEffectStatusCommand>(10);

    public void Initialize(SignalHub _signalHub)
    {
        signalHub = _signalHub;

        SubscribeEvents();
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

    public void EnemyCreated(List<Enemy> _enemies)
    {
        enemyUnits = _enemies;

        BindEvent_Enemy();

        signalHub.Publish(new EnemySpawnedEvent());
    }

    public void ActivatePlayerAndCharacter(GameStartedEvent gameStartedEvent)
    {
        signalHub.Publish(new PlayerSpawnedEvent(playerUnit));
        signalHub.Publish(new CharacterSpawnedEvent(characterUnit));

        characterUnit.gameObject.SetActive(true);
        playerUnit.gameObject.SetActive(true);
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<CardEffectStatusCommandDispatchEvent>(InsertCommand);
        signalHub.Subscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.Subscribe<CardUsingTurnFinishedEvent>(CardUsingTurnFinishedEvent);
        signalHub.Subscribe<CardDrawStartEvent>(CardDrawed);
        signalHub.Subscribe<StartMoveEvent>(StartEnemyMove);
        signalHub.Subscribe<GameStartedEvent>(ActivatePlayerAndCharacter);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<CardEffectStatusCommandDispatchEvent>(InsertCommand);
        signalHub.UnSubscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.UnSubscribe<CardUsingTurnFinishedEvent>(CardUsingTurnFinishedEvent);
        signalHub.UnSubscribe<CardDrawStartEvent>(CardDrawed);
        signalHub.UnSubscribe<StartMoveEvent>(StartEnemyMove);
        signalHub.UnSubscribe<GameStartedEvent>(ActivatePlayerAndCharacter);
    }

    public void Release()
    {
        ReleaseEvent_Character();
        ReleaseEvent_Enemy();
        ReleaseEvent_Player();
        UnSubscribeEvents();
    }

    private void BindEvent_Player()
    {
        playerUnit.TakeDamageEvent -= PlayerTakeDamage;
        playerUnit.TakeDamageEvent += PlayerTakeDamage;
    }

    private void ReleaseEvent_Player()
    {
        playerUnit.TakeDamageEvent -= PlayerTakeDamage;
        playerUnit.TakeDamageEvent += PlayerTakeDamage;
    }

    private void BindEvent_Character()
    {
        characterUnit.PlayerAttackFinishedEvent -= PlayerTurnFinished;
        characterUnit.PlayerAttackFinishedEvent += PlayerTurnFinished;
    }

    private void ReleaseEvent_Character()
    {
        characterUnit.PlayerAttackFinishedEvent -= PlayerTurnFinished;
    }

    private void BindEvent_Enemy()
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].UnitIsDeadEvent -= EnemyIsDead;
            enemyUnits[i].UnitIsDeadEvent += EnemyIsDead;
        }
    }

    private void ReleaseEvent_Enemy()
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].UnitIsDeadEvent -= EnemyIsDead;
        }
    }

    private void EnemyIsDead(Unit deadUnit)
    {
        signalHub.Publish(new EnemyIsDeadEvent(deadUnit.transform.position));
    }

    private void StartEnemyMove(StartMoveEvent startMoveEvent)
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].OnMove();
        }
    }

    private void PlayerTurnFinished()
    {
        signalHub.Publish(new PlayerTurnFinishedEvent());
    }

    private void EnemyTurnStarted(EnemyTurnStartEvent enemyTurnStartEvent)
    {
        characterUnit.ResetbCanAction();
    }

    private void CardDrawed(CardDrawStartEvent cardDrawStartEvent)
    {
        characterUnit.PlayerTurnStarted();
    }

    private void CardUsingTurnFinishedEvent(CardUsingTurnFinishedEvent cardUsingTurnFinishedEvent)
    {
        characterUnit.SetbCanAction();
    }

    public void InsertCommand(CardEffectStatusCommandDispatchEvent cardEffectCommandEvent)
    {
        var cardEffectCommand = cardEffectCommandEvent.command;

        cardEffectCommands.Add(cardEffectCommand);

        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        for (int i = 0; i < cardEffectCommands.Count; ++i)
        {
            cardEffectCommands[i].Execute(this);
        }
    }

    public void ApplyShieldModifier(float bonusShield)
    {
        playerUnit.shieldEffectReceiver.ApplyShieldModifier(bonusShield);
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAttackModifier(bonusDamage);
    }

    public bool CanApplyBulletEffect()
    {
        return characterUnit.combatEffectReceiver.CanApplyBulletEffect();
    }

    public void PlayerTakeDamage(float damage)
    {
        signalHub.Publish(new PlayerTakeDamageEvent(damage));
    }
}
