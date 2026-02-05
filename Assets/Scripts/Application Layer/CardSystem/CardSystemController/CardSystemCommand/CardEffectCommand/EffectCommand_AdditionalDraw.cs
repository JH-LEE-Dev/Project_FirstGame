using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AdditionalDraw")]
public class EffectCommand_AdditionalDraw : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;
    [SerializeField] private int upgradedDrawAmount = 0;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        if(bUpgraded == false)
            cardLogicSystemActionCommandHandler.DrawAgain(drawAmount  * valueModifier);
        else
            cardLogicSystemActionCommandHandler.DrawAgain(upgradedDrawAmount  * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler handler)
    {

    }
}