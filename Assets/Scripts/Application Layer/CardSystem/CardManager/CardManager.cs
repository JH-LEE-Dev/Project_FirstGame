using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.GPUSort;

public class CardManager : MonoBehaviour, ICardSystemActionCommandHandler, ICardSystemData
{
    private CardSystemContextType cardSystemContext;
    public CardManagerEventInvoker cardManagerEventInvoker;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>(50);
    private List<CardDataInstance> permanentDeckPile = new List<CardDataInstance>(50);
    private List<CardDataInstance> handPile = new List<CardDataInstance>(20);
    private List<CardDataInstance> gravePile = new List<CardDataInstance>(50);
    private List<CardDataInstance> extinctionPile = new List<CardDataInstance>(50);

    IReadOnlyList<CardDataInstance> ICardSystemData.deckCards => deckPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.handCards => handPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.graveCards => gravePile;
    IReadOnlyList<CardDataInstance> ICardSystemData.extinctionCards => extinctionPile;

    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int cardPileDrawAmount = 5;
    [SerializeField] private int initialDeckCnt = 40;

    public void Initialize()
    {
        cardManagerEventInvoker = new CardManagerEventInvoker();
    }

    public void Release()
    {

    }

    public void Awake()
    {
        for (int i = 0; i < cardDataBase.cardData.Count; ++i)
        {
            CardData cardData = cardDataBase.GetCardData(i);

            ObjectPool<CardDataInstance> pool = new ObjectPool<CardDataInstance>(
                createFunc: () =>
                {
                    CardDataInstance instance = new CardDataInstance();
                    instance.Initialize(cardData);
                    return instance;
                },
                actionOnGet: card =>
                {
                    card.ResetState();
                },
                actionOnRelease: card =>
                {
                    card.ResetState();
                },
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 40,
                maxSize: 40
            );

            cardPools.Add(cardData.id, pool);
        }
    }

    public void Start()
    {
        CardData cardData = cardDataBase.GetCardData(0);
        if (cardData == null)
            return;

        ObjectPool<CardDataInstance> pool = cardPools[cardData.id];

        for (int i = 0; i < initialDeckCnt; ++i)
        {
            CardDataInstance card = pool.Get();
            permanentDeckPile.Add(card);
        }
    }

    private void OnDestroy()
    {

    }

    public IReadOnlyList<CardDataInstance> GetExtinctionPile()
    {
        return extinctionPile;
    }

    public IReadOnlyList<CardDataInstance> GetDeckPile()
    {
        return deckPile;
    }

    public IReadOnlyList<CardDataInstance> GetGravePile()
    {
        return gravePile;
    }

    public IReadOnlyList<CardDataInstance> GetHandPile()
    {
        return handPile;
    }

    public void CardPileDraw(int amount, bool bAdditional)
    {
        int restDrawCnt = 0;

        if (deckPile.Count < amount)
        {
            restDrawCnt = amount - deckPile.Count;
            amount = deckPile.Count;
        }

        var rentalBuffer = new RentalScope<CardDataInstance>(amount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < amount; ++i)
        {
            if (deckPile.Count == 0)
            {
                break;
            }

            var card = deckPile[deckPile.Count - 1];
            deckPile.RemoveAt(deckPile.Count - 1);

            handPile.Add(card);
            writeBuffer[i] = card;
        }

        if (rentalBuffer.Span.Length != 0)
        {
            if (bAdditional == false)
                cardManagerEventInvoker.Dispatch(CardSystemEventType.CardPileDrawEvent, cardSystemContext, writeBuffer.Slice(0, amount));
            else
                cardManagerEventInvoker.Dispatch(CardSystemEventType.CardAdditionalDrawEvent, cardSystemContext, writeBuffer.Slice(0, amount));
        }
        else
        {
            Debug.Log("덱에 더 이상 카드가 없어요");
        }

        if (deckPile.Count == 0 && gravePile.Count != 0 && restDrawCnt != 0)
        {
            GraveToDeck(restDrawCnt);
            CardPileDraw(restDrawCnt, false);
        }

        //절대 잊으면 안됨!
        rentalBuffer.Dispose();
    }

    public void StartCardPileDraw()
    {
        CardPileDraw(cardPileDrawAmount, false);
    }

    private void CardAdditionalPileDraw(int amount)
    {
        CardPileDraw(amount, true);
    }

    public void CardsRemoveFromHand(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
                handPile.Remove(cards[i]);
        }
    }

    public void ReleaseCard(CardDataInstance card)
    {
        gravePile.Remove(card);

        int id = card.GetCardData().id;
        cardPools[id].Release(card);
    }

    public void ClearAllCards()
    {
        foreach (var card in handPile)
            cardPools[card.GetCardData().id].Release(card);

        foreach (var card in gravePile)
            cardPools[card.GetCardData().id].Release(card);

        handPile.Clear();
        gravePile.Clear();
        deckPile.Clear();
    }

    public void HandToGrave()
    {
        for (int i = 0; i < handPile.Count; ++i)
        {
            var card = handPile[i];
            gravePile.Add(card);
        }

        handPile.Clear();
    }

    private void GraveToDeck(int amount)
    {
        if (gravePile.Count == 0)
        {
            Debug.Log("묘지에 더 이상 카드가 없습니다. 씌발");
            return;
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(amount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < amount; ++i)
        {
            var card = gravePile[i];
            writeBuffer[i] = card;
            deckPile.Add(card);
            gravePile.Remove(card);
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.GraveCardsToDeckEvent, cardSystemContext, writeBuffer.Slice(0, amount));

        rentalBuffer.Dispose();
    }

    public void ResetCardPiles()
    {
        gravePile.Clear();
        deckPile.Clear();
        extinctionPile.Clear();
        handPile.Clear();

        for (int i = 0; i < permanentDeckPile.Count; ++i)
        {
            deckPile.Add(permanentDeckPile[i]);
        }
    }

    public void CardsToExtinction(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
                extinctionPile.Add(cards[i]);
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.CardsToExtinctionEvent, cardSystemContext, cards);
    }

    public void CardsToGrave(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                gravePile.Add(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.CardsToGraveEvent, cardSystemContext, cards);
    }

    public void ExecuteCommand(ICardSystemActionCommand actionCommand)
    {
        cardSystemContext = actionCommand.GetCardSystemContext();
        actionCommand.Execute(this);
    }

    public void DrawAgain(int drawAmount)
    {
        CardAdditionalPileDraw(drawAmount);
    }

    public void PlayerTurnFinished()
    {
        HandToGrave();

        cardManagerEventInvoker.Dispatch(CardSystemEventType.HandCardsToGraveEvent, cardSystemContext);
    }

    public void ApplyValueModifier(int valueModifier)
    {
        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().usingType == UsingType.Nesting)
                handPile[i].valueModifier *= valueModifier;
        }
    }

    public bool DeckConditionCheck(int cardID)
    {
        for (int i = 0; i < deckPile.Count; ++i)
        {
            if (deckPile[i].GetCardData().id != cardID)
                return false;
        }

        return true;
    }

    public void GraveCardsToHand(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                handPile.Add(cards[i]);
                gravePile.Remove(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.GraveCardsToHandEvent, cardSystemContext, cards);
    }

    public CardDataInstance CreateCard(int id)
    {
        CardData cardData = cardDataBase.GetCardData(id);
        if (cardData == null)
            return null;

        ObjectPool<CardDataInstance> pool = cardPools[cardData.id];

        CardDataInstance card = pool.Get();

        return card;
    }

    public void CardsToHand(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                handPile.Add(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.CardsToHandEvent, cardSystemContext, cards);
    }

    public void CardsToDeck(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                deckPile.Add(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.CardsToDeckEvent, cardSystemContext, cards);
    }

    public void ExtinctionCardsToDeck(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                deckPile.Add(cards[i]);
                extinctionPile.Remove(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.ExtinctionCardsToDeckEvent, cardSystemContext, cards);
    }

    public void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                deckPile.Add(cards[i]);
                gravePile.Remove(cards[i]);
            }
        }

        cardManagerEventInvoker.Dispatch(CardSystemEventType.GraveCardsToDeckEvent, cardSystemContext, cards);
    }
}
