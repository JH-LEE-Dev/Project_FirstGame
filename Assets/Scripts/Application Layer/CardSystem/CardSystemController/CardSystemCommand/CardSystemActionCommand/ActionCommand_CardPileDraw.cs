using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/CardPileDraw")]
public class ActionCommand_CardPileDraw : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.StartCardPileDraw();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
