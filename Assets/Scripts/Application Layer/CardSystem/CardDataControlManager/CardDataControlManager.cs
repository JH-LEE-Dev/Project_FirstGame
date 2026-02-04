using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDataControlManager : MonoBehaviour, ICardDataControlActionCommandHandler
{
    public CardSystemEventInvoker cardSystemEventInvoker;

    private CardSystemContextType cardSystemContext;

    public void Initialize()
    {
        cardSystemEventInvoker = new CardSystemEventInvoker();
    }

    public void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].GetCardData().bUpgradable == false)
                continue;

            if (bPermenant == false)
                cards[i].SetUpgrade(true);
            else
                cards[i].SetPermanentlyUpgrade(true);
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsUpgraded, cardSystemContext, cards);
    }

    public void ExecuteCommand(ICardSystemActionCommand actionCommand)
    {
        cardSystemContext = actionCommand.GetCardSystemContext();
        actionCommand.Execute(this);
    }

    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].GetCardData().usingType == UsingType.Nesting)
                cards[i].valueModifier *= valueModifier;
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsValueModified, cardSystemContext, cards);
    }

    public void SetCardSystemContext(CardSystemContextType cardSystemContextType)
    {
        cardSystemContext = cardSystemContextType;
    }

    public void RevertCardsUpgrade(ReadOnlySpan<CardDataInstance> cards, bool bPermenant)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (bPermenant == false)
                cards[i].SetUpgrade(false);
            else
                cards[i].SetPermanentlyUpgrade(false);
        }
    }
}
