using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AdditionalDraw")]
public class EffectCommand_AdditionalDraw : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    [SerializeField] private int drawAmount = 0;
    [SerializeField] private int upgradedDrawAmount = 0;

    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        var handPile = cardLogicSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == SYSTEM_VAR.maxHandPileCount)
        {
            Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
            return;
        }

        if (bUpgraded == false)
        {
            int newDrawAmount = drawAmount * valueModifier;

            if (handPile.Count + newDrawAmount > SYSTEM_VAR.maxHandPileCount)
            {
                newDrawAmount = SYSTEM_VAR.maxHandPileCount - handPile.Count;
            }

            if (newDrawAmount < 0)
            {
                Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
                return;
            }

            cardLogicSystemActionCommandHandler.DrawAgain(newDrawAmount);
        }
        else
        {
            int newDrawAmount = upgradedDrawAmount * valueModifier;

            if (handPile.Count + newDrawAmount > SYSTEM_VAR.maxHandPileCount)
            {
                newDrawAmount = SYSTEM_VAR.maxHandPileCount - handPile.Count;
            }

            if (newDrawAmount < 0)
            {
                Debug.LogWarning("패에는 최대 12장의 카드가 존재할 수 있습니다. - 추가 드로우 실패");
                return;
            }

            cardLogicSystemActionCommandHandler.DrawAgain(newDrawAmount);
        }

        ResetCommandData();
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler handler)
    {

    }
}