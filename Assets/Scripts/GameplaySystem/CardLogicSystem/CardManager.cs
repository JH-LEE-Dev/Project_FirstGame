using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class CardManager : MonoBehaviour, ICardSystemProvider,ICardLogicSystem
{
    public event Action HandChangedEvent;
    public event Action<CardDataInstance> CardDrawedEvent;
    public event Action<List<CardDataInstance>> CardPileDrawedEvent;
    public event Action CardDrawFinishedEvent;
    public event Action CardUsingFinishedEvent;
    public event Action<CardData> CardUsedEvent;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private Stack<CardDataInstance> drawPile = new Stack<CardDataInstance>();
    private List<CardDataInstance> handPile = new List<CardDataInstance>();
    private List<CardDataInstance> gravePile = new List<CardDataInstance>();

    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int drawCardCnt = 5;
    [SerializeField] private float cardDrawRate = 1f;


    public IReadOnlyList<CardData> HandCards => throw new NotImplementedException();

    public int deckCnt { get; private set; }

    public int graveCnt { get; private set; }
    public int handCnt { get; private set; }

    private Queue<CardEffectStrategy> cardSystemEffects = new Queue<CardEffectStrategy>();

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
                defaultCapacity: 5,
                maxSize: 20
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

        for (int i = 0; i < drawCardCnt; ++i)
        {
            CardDataInstance card = pool.Get();
            drawPile.Push(card);
            ++deckCnt;
        }
    }

    public void Draw(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawOne();
        }
    }

    private void DrawOne()
    {
        // if (drawPile.Count == 0)
        //Reshuffle();

        var card = drawPile.Pop();
        handPile.Add(card);

        --deckCnt;
        CardDrawedEvent?.Invoke(card);
    }

    private IEnumerator CardDrawCoroutine()
    {
        while (handCnt < 5)
        {
            yield return new WaitForSeconds(cardDrawRate);
            DrawOne();
            ++handCnt;
        }

        CardDrawFinishedEvent?.Invoke();
        handCnt = 0;
    }

    public void CardPileDraw()
    {
        List<CardDataInstance> hands = new List<CardDataInstance>();

        for (int i = 0; i < drawCardCnt; ++i)
        {
            var card = drawPile.Pop();
            handPile.Add(card);
            hands.Add(card);
            --deckCnt;
            ++handCnt;
        }

        CardPileDrawedEvent?.Invoke(hands);
    }

    private IEnumerator CardPileDrawCoroutine()
    {
        yield return new WaitForSeconds(cardDrawRate);

        CardPileDraw();

        CardDrawFinishedEvent?.Invoke();
        handCnt = 0;
    }

    public void CardUsed(CardDataInstance usedCard)
    {
        handPile.Remove(usedCard);
        gravePile.Add(usedCard);
        ++graveCnt;

        CardUsedEvent?.Invoke(usedCard.GetCardData());
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
        drawPile.Clear();

        deckCnt = 0;
        graveCnt = 0;
    }

    public void CardUsingFinished()
    {
        CardUsingFinishedEvent?.Invoke();

        for (int i = 0; i < handPile.Count; ++i)
        {
            var card = handPile[i];
            gravePile.Add(card);

        }

        handPile.Clear();

        for (int i = 0; i < gravePile.Count; ++i)
        {
            var card = gravePile[i];
            drawPile.Push(card);
        }

        gravePile.Clear();

        deckCnt = drawPile.Count;
        graveCnt = gravePile.Count;
    }

    public void StartDraw(int waveIdx)
    {
        //한장씩 드로우 할 때의 기능.
        //StartCoroutine(CardDrawCoroutine());

        //Pile 드로우.
        StartCoroutine(CardPileDrawCoroutine());
    }

    public void StrategyForwarding(CardEffectStrategy effectStrategy)
    {
        cardSystemEffects.Enqueue(effectStrategy);
    }

    public void DrawAgain(int drawAmount)
    {

    }
}
