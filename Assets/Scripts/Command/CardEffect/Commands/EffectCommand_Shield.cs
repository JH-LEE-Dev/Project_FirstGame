using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectStatusCommand
{
    [SerializeField] float bonusShield = 0f;

    public override void Execute(IUnitLogicCommandHandler unitLogicCommandHandler)
    {
        unitLogicCommandHandler.ApplyShieldModifier(bonusShield);
    }
}