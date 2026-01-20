using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/CardPileDraw")]
public class ActionCommand_CardPileDraw : CardSystemActionCommand
{
    public override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.StartCardPileDraw();
    }
}
