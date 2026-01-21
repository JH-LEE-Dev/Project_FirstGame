using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/DrawAgain")]
public class EffectCommand_DrawAgain : CardEffectCommand
{
    [SerializeField] private int drawAmount = 0;

    public override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.DrawAgain(drawAmount + nestingCnt);

        ResetCommandData();
    }
}
