using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectStatusCommand
{
    [SerializeField] float bonusDamage = 0f;

    public override void Execute(IUnitLogicCommandHandler unitLogicCommandHandler)
    {
        unitLogicCommandHandler.ApplyAttackModifier(bonusDamage);
    }
}