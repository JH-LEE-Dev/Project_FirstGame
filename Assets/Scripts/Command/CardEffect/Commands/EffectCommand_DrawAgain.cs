using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/DrawAgain")]
public class EffectCommand_DrawAgain : CardEffectSystemCommand
{
    [SerializeField] private int drawAmount = 0;

    public override void Execute(ICardEffectCommandHandler cardEffectCommandHandler)
    {
        cardEffectCommandHandler.DrawAgain(drawAmount);
        cardEffectCommandHandler.AttackAgain();
    }
}
