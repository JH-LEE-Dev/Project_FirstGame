using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDataControlManager : MonoBehaviour, ICardDataControlActionCommandHandler
{
    public CardSystemEventInvoker cardSystemEventInvoker;

    private GameSystemActionContextType cardSystemContext;

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

    public void ExecuteCommand(GameSystemCommand actionCommand, bool bUndo)
    {
        cardSystemContext = actionCommand.GetGameSystemContext();

        if (bUndo == false)
            actionCommand.Execute(this);
        else
            actionCommand.Undo(this);
    }

    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].GetCardData().usingType == UsingType.Nesting)
                cards[i].effectModifiers.Add(EffectModType.AllValueModifier,
                    new EffectModData(valueModifier, default, default));
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsValueModified, cardSystemContext, cards);
    }

    public void UndoValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].GetCardData().usingType == UsingType.Nesting)
            {
                if (cards[i].effectModifiers.ContainsKey(EffectModType.AllValueModifier))
                {
                    var data = cards[i].effectModifiers[EffectModType.AllValueModifier];
                    data.value /= valueModifier;
                    cards[i].effectModifiers[EffectModType.AllValueModifier] = data;

                    if (data.value == 1)
                    {
                        cards[i].effectModifiers.Remove(EffectModType.AllValueModifier);
                    }
                }
            }
        }

        cardSystemEventInvoker.Dispatch(CardDataControlSystemEventType.CardsValueModified, cardSystemContext, cards);
    }

    public void SetCardSystemContext(GameSystemActionContextType cardSystemContextType)
    {
        cardSystemContext = cardSystemContextType;
    }
}
