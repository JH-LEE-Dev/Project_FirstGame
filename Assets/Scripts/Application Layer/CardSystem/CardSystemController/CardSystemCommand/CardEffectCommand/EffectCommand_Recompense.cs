using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Recompense")]
public class EffectCommand_Recompense : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var handPile = complexSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == SYSTEM_VAR.maxHandPileCount)
        {
            Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
            return;
        }

        if (bUpgraded == false)
        {
            IReadOnlyList<CardDataInstance> prevHandToGraveCards = complexSystemActionCommandHandler.GetPrevHandToGraveCards();

            int bulletCardCnt = 0;
            for (int i = 0; i < prevHandToGraveCards.Count; ++i)
            {
                if (prevHandToGraveCards[i].GetCardData().cardType == CardType.Bullet)
                    ++bulletCardCnt;
            }

            int newDrawAmount = bulletCardCnt;
            if (handPile.Count + newDrawAmount > SYSTEM_VAR.maxHandPileCount)
            {
                newDrawAmount = SYSTEM_VAR.maxHandPileCount - handPile.Count;
            }

            if (newDrawAmount < 0)
            {
                Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
                return;
            }

            complexSystemActionCommandHandler.AdditionalDraw(newDrawAmount, cardSystemContextType);
        }
        else
        {
            IReadOnlyList<CardDataInstance> prevHandToGraveCards = complexSystemActionCommandHandler.GetPrevHandToGraveCards();

            int newDrawAmount = prevHandToGraveCards.Count;
            if (handPile.Count + newDrawAmount > SYSTEM_VAR.maxHandPileCount)
            {
                newDrawAmount = SYSTEM_VAR.maxHandPileCount - handPile.Count;
            }

            if (newDrawAmount < 0)
            {
                Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
                return;
            }

            complexSystemActionCommandHandler.AdditionalDraw(newDrawAmount, cardSystemContextType);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}
