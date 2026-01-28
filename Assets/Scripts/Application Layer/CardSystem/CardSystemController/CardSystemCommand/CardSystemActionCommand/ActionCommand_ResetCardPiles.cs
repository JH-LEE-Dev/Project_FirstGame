using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/ResetCardPiles")]
public class ActionCommand_ResetCardPiles : CardSystemActionCommand<ICardSystemActionCommandHandler>
{

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.ResetCardPiles();
    }
}
