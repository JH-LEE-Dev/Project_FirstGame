using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectSystemCommand
{

    public override void Execute(ICardEffectCommandHandler cardEffectCommandHandler)
    {
        cardEffectCommandHandler.AttackAgain();
    }
}