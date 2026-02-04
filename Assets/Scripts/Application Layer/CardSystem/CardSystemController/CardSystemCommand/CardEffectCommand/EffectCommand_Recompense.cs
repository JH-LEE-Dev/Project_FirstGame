using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Recompense")]
public class EffectCommand_Recompense : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
        {
            IReadOnlyList<CardDataInstance> prevHandToGraveCards = complexSystemActionCommandHandler.GetPrevHandToGraveCards();

            int bulletCardCnt = 0;
            for(int i = 0;i<prevHandToGraveCards.Count;++i)
            {
                if (prevHandToGraveCards[i].GetCardData().cardType == CardType.Bullet)
                    ++bulletCardCnt;
            }

            complexSystemActionCommandHandler.AdditionalDraw(bulletCardCnt, cardSystemContextType);
        }

        if(upgradeNestingCnt != 0)
        {
            IReadOnlyList<CardDataInstance> prevHandToGraveCards = complexSystemActionCommandHandler.GetPrevHandToGraveCards();

            complexSystemActionCommandHandler.AdditionalDraw(prevHandToGraveCards.Count, cardSystemContextType);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}
