using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CardManager : MonoBehaviour, ICardLogicSystemActionCommandHandler, ICardSystemData, ICardLogicSystemProvider
{
    //내부 의존성
    public CardSystemEventInvoker cardSystemEventInvoker;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>(SYSTEM_VAR.limitDeckPileCount);
    private List<CardDataInstance> permanentDeckPile = new List<CardDataInstance>(SYSTEM_VAR.limitDeckPileCount);
    private List<CardDataInstance> handPile = new List<CardDataInstance>(SYSTEM_VAR.maxHandPileCount);
    private List<CardDataInstance> gravePile = new List<CardDataInstance>(SYSTEM_VAR.limitDeckPileCount);
    private List<CardDataInstance> extinctionPile = new List<CardDataInstance>(SYSTEM_VAR.limitDeckPileCount);

    IReadOnlyList<CardDataInstance> ICardSystemData.permenantDeckCards => permanentDeckPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.deckCards => deckPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.handCards => handPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.graveCards => gravePile;
    IReadOnlyList<CardDataInstance> ICardSystemData.extinctionCards => extinctionPile;

    [SerializeField] private CardDeckDataBase cardDeckDataBase;
    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int cardPileDrawAmount = 5;
    //[SerializeField] private int initialDeckCnt = 40;

    private CardSystemContextType cardSystemContext;

    public void Initialize()
    {
        cardSystemEventInvoker = new CardSystemEventInvoker();
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
                defaultCapacity: SYSTEM_VAR.maxDeckPileCount,
                maxSize: SYSTEM_VAR.limitDeckPileCount
            );

            cardPools.Add(cardData.id, pool);
        }
    }

    public void Start()
    {
        int totalCnt = 0;

        for(int i = 0;i< cardDeckDataBase.cardPileData.Count;++i)
        {
            totalCnt += cardDeckDataBase.cardPileData[i].cnt;
        }
        if(totalCnt >= SYSTEM_VAR.maxDeckPileCount)
        {
            Debug.LogWarning("덱에 최대 30장의 카드만 넣을 수 있습니다.");
            return;
        }

        for (int i = 0; i < cardDeckDataBase.cardPileData.Count; ++i)
        {
            var data = cardDeckDataBase.cardPileData[i];

            CardData cardData = cardDataBase.GetCardData((int)data.cardName);
            if (cardData == null)
                return;

            ObjectPool<CardDataInstance> pool = cardPools[cardData.id];

            for (int j = 0; j < data.cnt; ++j)
            {
                CardDataInstance card = pool.Get();
                card.bPermanent = true;

                if (data.bUpgraded)
                    card.SetPermanentlyUpgrade(true);

                permanentDeckPile.Add(card);
            }
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

        using var rentalBuffer = new RentalScope<CardDataInstance>(amount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int n = deckPile.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);

            var card_1 = deckPile[k];
            deckPile[k] = deckPile[n];
            deckPile[n] = card_1;
        }

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
                cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardPileDrawEvent, cardSystemContext, writeBuffer.Slice(0, amount));
            else
                cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardAdditionalDrawEvent, cardSystemContext, writeBuffer.Slice(0, amount));
        }
        else
        {
            Debug.Log("덱에 더 이상 카드가 없어요");
        }

        if (deckPile.Count == 0 && gravePile.Count != 0 && restDrawCnt != 0)
        {
            GraveToDeck();

            if (deckPile.Count != 0)
                CardPileDraw(restDrawCnt, false);
        }
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
        using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < handPile.Count; ++i)
        {
            var card = handPile[i];
            gravePile.Add(card);
            writeBuffer[i] = handPile[i];
        }

        handPile.Clear();

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.HandCardsToGraveEvent, cardSystemContext, writeBuffer);
    }

    private void GraveToDeck()
    {
        if (gravePile.Count == 0)
        {
            Debug.Log("묘지에 더 이상 카드가 없습니다.");
            return;
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < gravePile.Count; ++i)
        {
            var card = gravePile[i];
            writeBuffer[i] = card;
            deckPile.Add(card);
        }

        gravePile.Clear();
        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.GraveCardsToDeckEvent, cardSystemContext, writeBuffer);
    }

    public void ResetCardPiles()
    {
        for (int i = 0; i < deckPile.Count; ++i)
        {
            if (deckPile[i].bPermanent == false)
                cardPools[deckPile[i].GetCardData().id].Release(deckPile[i]);
        }
        deckPile.Clear();

        for (int i = 0; i < gravePile.Count; ++i)
        {
            if (gravePile[i].bPermanent == false)
                cardPools[gravePile[i].GetCardData().id].Release(gravePile[i]);
        }
        gravePile.Clear();

        for (int i = 0; i < extinctionPile.Count; ++i)
        {
            if (extinctionPile[i].bPermanent == false)
                cardPools[extinctionPile[i].GetCardData().id].Release(extinctionPile[i]);
        }
        extinctionPile.Clear();

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].bPermanent == false)
                cardPools[handPile[i].GetCardData().id].Release(handPile[i]);
        }
        handPile.Clear();

        for (int i = 0; i < permanentDeckPile.Count; ++i)
        {
            permanentDeckPile[i].SetUpgrade(false);
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
        Debug.Log(cardSystemContext);
        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardsToExtinctionEvent, cardSystemContext, cards);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardsToGraveEvent, cardSystemContext, cards);
    }

    public void ExecuteCommand(ICardSystemActionCommand actionCommand)
    {
        cardSystemContext = actionCommand.GetCardSystemContext();
        actionCommand.Execute(this);
    }

    public void SetCardSystemContext(CardSystemContextType cardSystemContextType)
    {
        cardSystemContext = cardSystemContextType;
    }

    public void DrawAgain(int drawAmount)
    {
        CardAdditionalPileDraw(drawAmount);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.GraveCardsToHandEvent, cardSystemContext, cards);
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

    public void ReleaseCard(CardDataInstance card)
    {
        cardPools[card.GetCardData().id].Release(card);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardsToHandEvent, cardSystemContext, cards);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.CardsToDeckEvent, cardSystemContext, cards);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.ExtinctionCardsToDeckEvent, cardSystemContext, cards);
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

        cardSystemEventInvoker.Dispatch(CardLogicSystemEventType.GraveCardsToDeckEvent, cardSystemContext, cards);
    }

    public void AddCardsToDeck(ReadOnlySpan<CardDataInstance> _cards)
    {
        if(permanentDeckPile.Count + _cards.Length > SYSTEM_VAR.maxDeckPileCount)
        {
            Debug.LogWarning("덱에 최대 30장의 카드만 넣을 수 있습니다.");
            return;
        }

        for (int i = 0; i < _cards.Length; ++i)
        {
            permanentDeckPile.Add(_cards[i] as CardDataInstance);
        }
    }

    public void DeleteCardsFromDeck(ReadOnlySpan<CardDataInstance> _cards)
    {
        for (int i = 0; i < _cards.Length; ++i)
        {
            CardDataInstance card = _cards[i] as CardDataInstance;

            cardPools[card.GetCardData().id].Release(card);
            permanentDeckPile.Remove(card);
        }
    }
}
