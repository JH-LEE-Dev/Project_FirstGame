using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AttackAgain")]
public class EffectCommand_AttackAgain : CardEffectSystemCommand
{
    public override void Execute(ICardEffectCommandHandler cardEffectCommandHandler)
    {
        cardEffectCommandHandler.AttackAgain();
    }
}
