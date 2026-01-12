using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Flare")]
public class EffectCommand_Flare : CardEffectSystemCommand
{
    [SerializeField] private int drawAmount = 0;

    public override void Execute(ICardEffectCommandHandler cardEffectCommandHandler)
    {
        cardEffectCommandHandler.DrawAgain(drawAmount);
    }
}