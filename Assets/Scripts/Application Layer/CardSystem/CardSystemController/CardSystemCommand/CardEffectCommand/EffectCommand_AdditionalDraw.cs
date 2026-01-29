using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AdditionalDraw")]
public class EffectCommand_AdditionalDraw : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;
    [SerializeField] private int upgradedDrawAmount = 0;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            cardLogicSystemActionCommandHandler.DrawAgain(drawAmount * nestingCnt*valueModifier);

        if(upgradeNestingCnt != 0)
            cardLogicSystemActionCommandHandler.DrawAgain(upgradedDrawAmount * upgradeNestingCnt*valueModifier);

        ResetCommandData();
    }
}