using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/DrawAgain")]
public class EffectCommand_DrawAgain : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;
    [SerializeField] private int upgradedDrawAmount = 0;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            cardSystemActionCommandHandler.DrawAgain(drawAmount * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
            cardSystemActionCommandHandler.DrawAgain(upgradedDrawAmount * upgradeNestingCnt * valueModifier);

        ResetCommandData();
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
