using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/DrawAgain")]
public class EffectCommand_DrawAgain : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;
    [SerializeField] private int upgradedDrawAmount = 0;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        if (bUpgraded == false)
            cardSystemActionCommandHandler.DrawAgain(drawAmount  * valueModifier);
        else
            cardSystemActionCommandHandler.DrawAgain(upgradedDrawAmount  * valueModifier);

        ResetCommandData();
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
