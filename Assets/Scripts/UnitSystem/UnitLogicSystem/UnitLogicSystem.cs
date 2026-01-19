using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using CardEffectSystemSignal;
using GameControlSignals;
using CardSystemSignals;
using UnitLogicSystemSignals;
using WaveSystemSignals;
using UnitSpawnSystemSignals;
using System;

//이 클래스 책임이 커질 거 같으므로, 컴포넌트로 기능 분할할 것.

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour, IUnitLogicCommandHandler
{
    public event Action EnemySpawnedEvent;
    public event Action<Character> CharacterSpawendEvent;
    public event Action<Earth> PlayerSpawnedEvent;
    public event Action<Vector2> EnemyIsDeadEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action<float> PlayerTakeDamageEvent;

    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth playerUnit;
    private List<Enemy> enemyUnits;

    private List<CardEffectStatusCommand> cardEffectCommands = new List<CardEffectStatusCommand>(10);

    public void Initialize()
    {
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

        EnemySpawnedEvent?.Invoke();
    }

    public void ActivatePlayerAndCharacter(GameStartedEvent gameStartedEvent)
    {
        PlayerSpawnedEvent?.Invoke(playerUnit);
        CharacterSpawendEvent?.Invoke(characterUnit);

        characterUnit.gameObject.SetActive(true);
        playerUnit.gameObject.SetActive(true);
    }

    public void Release()
    {
        ReleaseEvent_Character();
        ReleaseEvent_Enemy();
        ReleaseEvent_Player();
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
        EnemyIsDeadEvent?.Invoke(deadUnit.transform.position);
    }

    public void StartEnemyMove(StartMoveEvent startMoveEvent)
    {
        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            enemyUnits[i].OnMove();
        }
    }

    private void PlayerTurnFinished()
    {
        PlayerTurnFinishedEvent?.Invoke();
    }

    public void EnemyTurnStarted(EnemyTurnStartEvent enemyTurnStartEvent)
    {
        characterUnit.ResetbCanAction();
    }

    public void CardDrawed(CardDrawStartEvent cardDrawStartEvent)
    {
        characterUnit.PlayerTurnStarted();
    }

    public void CardUsingTurnFinished(CardUsingTurnFinishedEvent cardUsingTurnFinishedEvent)
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

    public void CanApplyBulletEffect(CardDataInstance usedCard)
    {
        characterUnit.combatEffectReceiver.CanApplyBulletEffect();
        //Signal Publish (CardUsedEvent)
    }

    public void PlayerTakeDamage(float damage)
    {
        PlayerTakeDamageEvent?.Invoke(damage);
    }
}
