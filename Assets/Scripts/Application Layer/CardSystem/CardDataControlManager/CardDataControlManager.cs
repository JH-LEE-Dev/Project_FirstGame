using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDataControlManager : MonoBehaviour, ICardDataControlSystemActionCommandHandler
{
    public CardSystemEventInvoker cardSystemEventInvoker;

    private CardSystemContextType cardSystemContext;

    public void Initialize()
    {
        cardSystemEventInvoker = new CardSystemEventInvoker();
    }

    public void UpgradeCards(ReadOnlySpan<CardDataInstance> cards)
    {
        for(int i = 0;i < cards.Length;++i)
        {
            cards[i].SetUpgrade(true);
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsUpgraded, cardSystemContext, cards);
    }

    public void ExecuteCommand(ICardSystemActionCommand actionCommand)
    {
        cardSystemContext = actionCommand.GetCardSystemContext();
        actionCommand.Execute(this);
    }

    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards,int valueModifier)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].GetCardData().usingType == UsingType.Nesting)
                cards[i].valueModifier *= valueModifier;
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsValueModified, cardSystemContext, cards);
    }
}
