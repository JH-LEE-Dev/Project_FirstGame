using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Recompense")]
public class EffectCommand_Recompense : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
        {
            int prevUsedBulletCardCnt = complexSystemActionCommandHandler.GetPrevUsedBulletCardCnt();
            complexSystemActionCommandHandler.AdditionalDraw(prevUsedBulletCardCnt);
        }

        if(upgradeNestingCnt != 0)
        {
            int prevUsedCardCnt = complexSystemActionCommandHandler.GetPrevUsedCardCnt();
            complexSystemActionCommandHandler.AdditionalDraw(prevUsedCardCnt);
        }

        ResetCommandData();
    }
}
