using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/DuplicateCardsToHand")]
public class ActionCommand_DuplicateCardsToHand : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.DuplicateCardCardsToHand;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(cnt);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;


        var handPile = cardSystemActionCommandHandler.GetHandPile();

        if (handPile.Count + cnt > SYSTEM_VAR.maxHandPileCount)
            cnt = SYSTEM_VAR.maxHandPileCount - handPile.Count;

        if (cnt < 0)
        {
            Debug.LogWarning("패로 카드를 이동시키지 못했습니다. 패 총량 초과.");
            return;
        }


        int duplicatedCnt = 0;

        for (int i = 0; i < cnt; ++i)
        {
            if (cards[i] != null)
            {
                var newCard = cardSystemActionCommandHandler.CreateCard(cards[i].GetCardData().id);

                if (newCard == null)
                {
                    Debug.LogWarning("카드를 복제할 수 없습니다. 카드 총량은 50장입니다.");
                    break;
                }

                writeBuffer[duplicatedCnt] = newCard;
                writeBuffer[duplicatedCnt].SetUpgrade(cards[i].IsUpgraded());
                ++duplicatedCnt;
            }
        }

        if (duplicatedCnt != 0)
            cardSystemActionCommandHandler.CardsToHand(writeBuffer.Slice(0, duplicatedCnt));
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
