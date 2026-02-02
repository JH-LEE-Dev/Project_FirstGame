using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/HandToGrave")]
public class ActionCommand_HandToGrave : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.HandToGrave();
    }
}
