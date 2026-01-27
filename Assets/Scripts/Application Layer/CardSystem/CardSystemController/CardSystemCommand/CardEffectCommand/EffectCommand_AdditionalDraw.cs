using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AdditionalDraw")]
public class EffectCommand_AdditionalDraw : CardEffectCommand<ICardSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.DrawAgain(drawAmount *nestingCnt);

        ResetCommandData();
    }
}