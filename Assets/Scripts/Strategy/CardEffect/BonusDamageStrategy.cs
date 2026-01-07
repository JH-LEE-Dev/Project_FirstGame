using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/CardEffect/Bullet/BonusDamage")]
public class BonusDamageStrategy : CardEffectStrategy
{
    [SerializeField] float bonusDamage = 0f;

    public override void Execute()
    {
        Execute_Status();
        Execute_System();
    }

    protected override void Execute_Status()
    {
        unitLogicSystem.ApplyAttackModifier(bonusDamage);
    }

    protected override void Execute_System()
    {
        return;
    }
}