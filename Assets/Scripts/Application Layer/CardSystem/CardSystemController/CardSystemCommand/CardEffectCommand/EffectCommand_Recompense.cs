using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Recompense")]
public class EffectCommand_Recompense : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        int prevUsedBulletCard = complexSystemActionCommandHandler.GetPrevUsedBulletCardCnt();
        complexSystemActionCommandHandler.AdditionalDraw(prevUsedBulletCard);

        ResetCommandData();
    }
}
