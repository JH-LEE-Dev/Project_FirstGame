using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using CardEffectSystemSignal;
using GameControlSignals;
using CardSystemSignals;
using UnitLogicSystemSignals;
using WaveSystemSignals;

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour, IUnitLogicSystemActions, IUnitLogicSystemData, IUnitLogicCommandHandler
{
    //외부 의존성
    private ISignalHub<IPulicSignal> signalHub;

    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth earthUnit;
    private List<Enemy> enemyUnits;

    public IReadOnlyList<IEnemyData> enemyData => enemyUnits;

    public ICharacterData characterData => characterUnit;

    public IPlayerData playerData => earthUnit;


    private List<CardEffectStatusCommand> cardEffectCommands = new List<CardEffectStatusCommand>(10);

    public void Initialize(ISignalHub<IPulicSignal> _signalHub)
    {
        signalHub = _signalHub;

        SubscribeEvents();
    }

    public void DependencyInjection_Character(Character _characterUnit)
    {
        characterUnit = _characterUnit;

        BindEvent_Character();
    }

    public void DependencyInjection_Earth(Earth earth)
    {
        earthUnit = earth;

        BindEvent_Player();
    }

    public void DependencyInjection_Enemy(List<Enemy> enemies)
    {
        enemyUnits = enemies;

        BindEvent_Enemy();
    }


    private void SubscribeEvents()
    {
        signalHub.Subscribe<CardEffectStatusCommandDispatchEvent>(InsertCommand);
        signalHub.Subscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.Subscribe<CardUsingTurnFinishedEvent>(CardUsingTurnFinishedEvent);
        signalHub.Subscribe<CardDrawStartEvent>(CardDrawed);
        signalHub.Subscribe<StartMoveEvent>(StartEnemyMove);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<CardEffectStatusCommandDispatchEvent>(InsertCommand);
        signalHub.UnSubscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.UnSubscribe<CardUsingTurnFinishedEvent>(CardUsingTurnFinishedEvent);
        signalHub.UnSubscribe<CardDrawStartEvent>(CardDrawed);
        signalHub.UnSubscribe<StartMoveEvent>(StartEnemyMove);
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
        earthUnit.TakeDamageEvent -= PlayerTakeDamage;
        earthUnit.TakeDamageEvent += PlayerTakeDamage;
    }

    private void ReleaseEvent_Player()
    {
        earthUnit.TakeDamageEvent -= PlayerTakeDamage;
        earthUnit.TakeDamageEvent += PlayerTakeDamage;
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
        earthUnit.shieldEffectReceiver.ApplyShieldModifier(bonusShield);
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
