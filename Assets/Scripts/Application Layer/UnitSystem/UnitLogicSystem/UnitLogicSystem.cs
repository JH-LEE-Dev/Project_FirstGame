using UnityEngine;
using System.Collections.Generic;
using CardEffectSystemSignal;
using GameControlSignals;
using CardSystemSignals;
using WaveSystemSignals;
using System;
using CardSystemUISignal;

//이 클래스 책임이 커질 거 같으므로, 컴포넌트로 기능 분할할 것.

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour, ICardStatusEffectCommandHandler
{
    public event Action EnemySpawnedEvent;
    public event Action<Character> CharacterSpawendEvent;
    public event Action<Earth> PlayerSpawnedEvent;
    public event Action<Vector2> EnemyIsDeadEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action<float> PlayerTakeDamageEvent;
    public event Action PlayerAttackedEvent;

    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth playerUnit;
    private List<Enemy> enemyUnits;

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

    public void ActivatePlayerAndCharacter(GameStartedSignal gameStartedSignal)
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

        characterUnit.PlayerAttackEvent -= PlayerAttacked;
        characterUnit.PlayerAttackEvent += PlayerAttacked;
    }

    private void ReleaseEvent_Character()
    {
        characterUnit.PlayerAttackFinishedEvent -= PlayerTurnFinished;

        characterUnit.PlayerAttackEvent -= PlayerAttacked;
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
    }

    public void EnemyTurnStarted(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        characterUnit.ResetbCanAction();
    }

    public void CardDrawed(CardDrawStartSignal cardDrawStartSignal)
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

        cardEffectCommand.Execute(this);
    }

    public void ApplyShieldModifier(float bonusShield)
    {
        playerUnit.shieldEffectReceiver.ApplyShieldModifier(bonusShield);
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        characterUnit.combatEffectReceiver.ApplyAttackModifier(bonusDamage);
    }

    public void PlayerTakeDamage(float damage)
    {
        PlayerTakeDamageEvent?.Invoke(damage);
    }

    public void AttackAgain()
    {
        throw new NotImplementedException();
    }

    private void PlayerAttacked()
    {
        PlayerAttackedEvent?.Invoke();
    }
}
