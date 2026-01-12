using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitLogicSystem : MonoBehaviour, IUnitLogicSystemActions, IUnitLogicSystemProvider, IUnitLogicCommandHandler
{
    //의존성 DIP적용 검토하기.
    private Character characterUnit;
    private Earth earthUnit;
    private List<Enemy> enemyUnits;

    public IReadOnlyList<IEnemyData> enemyData => enemyUnits;

    public ICharacterData characterData => characterUnit;

    public IEarthData earthData => earthUnit;

    private List<CardEffectStatusCommand> cardEffectCommands = new List<CardEffectStatusCommand>(10);

    public void Initialize(Character _characterUnit, Earth _earthUnit, List<Enemy> _enemyUnits)
    {
        characterUnit = _characterUnit;
        enemyUnits = _enemyUnits;
        earthUnit = _earthUnit;
    }

    public void InsertCommand(CardEffectStatusCommand cardEffectCommand)
    {
        cardEffectCommands.Add(cardEffectCommand);

        ExecuteCommands();
    }

    private void ExecuteCommands()
    { 
        for(int i=0; i<cardEffectCommands.Count; ++i)
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
