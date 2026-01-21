using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Flare")]
public class EffectCommand_Flare : CardEffectCommand
{
    [SerializeField] private int drawAmount = 0;

    public override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.DrawAgain(drawAmount + nestingCnt);

        ResetCommandData();
    }
}