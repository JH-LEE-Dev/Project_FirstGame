using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/CardPileDraw")]
public class ActionCommand_CardPileDraw : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.StartCardPileDraw();
    }
}
