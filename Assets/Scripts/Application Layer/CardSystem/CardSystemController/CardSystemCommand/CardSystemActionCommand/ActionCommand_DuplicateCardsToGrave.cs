using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/DuplicateCardsToGrave")]
public class ActionCommand_DuplicateCardsToGrave : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
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
            cardSystemActionCommandHandler.CardsToGrave(writeBuffer.Slice(0, duplicatedCnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
