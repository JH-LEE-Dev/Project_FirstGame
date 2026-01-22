using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/DrawAgain")]
public class EffectCommand_DrawAgain : CardEffectCommand<ICardSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.DrawAgain(drawAmount + nestingCnt);

        ResetCommandData();
    }
}
