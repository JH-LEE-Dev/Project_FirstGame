using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/ResetCardPiles")]
public class ActionCommand_ResetCardPiles : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.ResetCardPiles();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
