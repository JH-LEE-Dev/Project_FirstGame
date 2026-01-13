using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

//캐릭터를 상위 모듈에 노출할 때 인터페이스로 묶어서 노출할 것. 이때 CombatReceiver도 private으로 해서 
//캐릭터를 Facade로 사용할 것.
public class UnitLogicSystem : MonoBehaviour, IUnitLogicSystemActions, IUnitLogicSystemProvider, IUnitLogicCommandHandler
{
    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth earthUnit;
    private List<Enemy> enemyUnits;

    public IReadOnlyList<IEnemyData> enemyData => enemyUnits;

    public ICharacterData characterData => characterUnit;

    public IPlayerData playerData => earthUnit;


    private List<CardEffectStatusCommand> cardEffectCommands = new List<CardEffectStatusCommand>(10);

    public void Initialize(Character _characterUnit)
    {
        characterUnit = _characterUnit;
    }

    public void Initialize(Earth earth)
    {
        earthUnit = earth;
    }

    public void Initialize(List<Enemy> enemies)
    {
        enemyUnits = enemies;
    }

    public void InsertCommand(CardEffectStatusCommand cardEffectCommand)
    {
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
}
