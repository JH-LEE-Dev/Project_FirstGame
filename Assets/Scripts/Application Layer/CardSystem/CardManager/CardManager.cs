using CardSystemSignals;
using GameControlSignals;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using UnityEngine;
using UnityEngine.Pool;

public class CardManager : MonoBehaviour, ICardSystemActionCommandHandler, ICardSystemData
{
    public delegate void CardPileDrawDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void CardAdditionalDrawDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void GraveToDeckDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void HandToGraveDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void GraveToHandDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void CardToExtinctionDelgate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void CardToGraveDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void ExtinctionToDeckDelegate(ReadOnlySpan<CardDataInstance> cards);
    public event CardPileDrawDelegate CardPileDrawEvent;
    public event CardAdditionalDrawDelegate CardAdditionalDrawEvent;
    public event GraveToDeckDelegate GraveToDeckEvent;
    public event HandToGraveDelegate HandToGraveEvent;
    public event GraveToHandDelegate GraveToHandEvent;
    public event CardToExtinctionDelgate CardToExtinctionEvent;
    public event CardToGraveDelegate CardToGraveEvent;
    public event ExtinctionToDeckDelegate ExtinctionToDeckEvent;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>(50);
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

        for (int i = 0; i < 9; ++i)
        {
            CardDataInstance card = pool.Get();
            deckPile.Add(card);
        }

        cardData = cardDataBase.GetCardData(0);
        if (cardData == null)
            return;

        pool = cardPools[cardData.id];

        for (int i = 0; i < 1; ++i)
        {
            CardDataInstance card = pool.Get();
            deckPile.Add(card);
        }
    }

    private void OnDestroy()
    {

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
                CardPileDrawEvent?.Invoke(writeBuffer.Slice(0,amount));
            else
                CardAdditionalDrawEvent?.Invoke(writeBuffer.Slice(0, amount));
        }
        else
        {
            Debug.Log("덱에 더 이상 카드가 없어요");
        }

        if (deckPile.Count == 0 && gravePile.Count != 0 && restDrawCnt != 0)
        {
            GraveToDeck();
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

    public void CardRemoveFromHand(CardDataInstance usedCard)
    {
        handPile.Remove(usedCard);
    }

    public void CardRemoveFromHand(ReadOnlySpan<CardDataInstance> cardPile)
    {
        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                handPile.Remove(cardPile[i]);
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

    private void GraveToDeck()
    {
        if (gravePile.Count == 0)
        {
            Debug.Log("묘지에 더 이상 카드가 없습니다. 씌발");
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

        GraveToDeckEvent?.Invoke(writeBuffer.Slice(0, gravePile.Count));
        rentalBuffer.Dispose();

        gravePile.Clear();
    }

    public void ExtinctionToDeck()
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(extinctionPile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < extinctionPile.Count; ++i)
        {
            var card = extinctionPile[i];
            writeBuffer[i] = card;
            deckPile.Add(card);
        }

        ExtinctionToDeckEvent?.Invoke(writeBuffer.Slice(0, extinctionPile.Count));
        rentalBuffer.Dispose();

        extinctionPile.Clear();
    }

    public void ExtinguishCards(CardDataInstance usedCard)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        extinctionPile.Add(usedCard);
        writeBuffer[0] = usedCard;

        CardToExtinctionEvent?.Invoke(writeBuffer.Slice(0,1));
        rentalBuffer.Dispose();
    }

    public void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile)
    {
        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                extinctionPile.Add(cardPile[i]);
        }

        CardToExtinctionEvent?.Invoke(cardPile);
    }

    public void CardToGrave(CardDataInstance usedCard)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        gravePile.Add(usedCard);
        writeBuffer[0] = usedCard;

        CardToGraveEvent?.Invoke(writeBuffer.Slice(0,1));
        rentalBuffer.Dispose();
    }

    public void CardsToGrave(ReadOnlySpan<CardDataInstance> cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
                gravePile.Add(cards[i]);
        }

        CardToGraveEvent?.Invoke(cards);
    }

    public void ExecuteCommand(ICardSystemActionCommand actionCommand)
    {
        actionCommand.Execute(this);
    }

    public void DrawAgain(int drawAmount)
    {
        CardAdditionalPileDraw(drawAmount);
    }

    public void PlayerTurnFinished()
    {
        HandToGrave();

        HandToGraveEvent?.Invoke(null);
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

    public void GraveToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards)
    {
        for (int i = 0; i < graveToDeckCards.Length; ++i)
        {
            if (graveToDeckCards[i] != null)
            {
                handPile.Add(graveToDeckCards[i]);
                gravePile.Remove(graveToDeckCards[i]);
            }
        }

        GraveToHandEvent?.Invoke(graveToDeckCards);
    }

    public IReadOnlyList<CardDataInstance> GetHandPile()
    {
        return handPile;
    }

    public void RandomExtinctionCardToDeck()
    {
        if (extinctionPile.Count == 0)
        {
            Debug.Log("소멸된 카드가 없습니다.");
            return;
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int randomIdx = UnityEngine.Random.Range(0, extinctionPile.Count - 1);

        deckPile.Add(extinctionPile[randomIdx]);
        writeBuffer[0] = extinctionPile[randomIdx];
        extinctionPile.RemoveAt(randomIdx);

        ExtinctionToDeckEvent?.Invoke(writeBuffer.Slice(0, 1));
        rentalBuffer.Dispose();
    }

    public CardDataInstance CreateCard(int id)
    {
        CardData cardData = cardDataBase.GetCardData(13);
        if (cardData == null)
            return null;

        ObjectPool<CardDataInstance> pool = cardPools[cardData.id];

        CardDataInstance card = pool.Get();

        return card;
    }
}
