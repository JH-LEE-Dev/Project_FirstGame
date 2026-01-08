using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class CardManager : MonoBehaviour, ICardSystemProvider, ICardStrategyHandler, ICardSystemEvent, ICardSystemActions
{
    public event Action<CardDataInstance> CardDrawedEvent;
    public event Action<List<CardDataInstance>> CardPileDrawedEvent;
    public event Action CardDrawFinishedEvent;
    public event Action CardUsingTurnFinishedEvent;
    public event Action<CardData> CardUsedEvent;
    public event Action<bool> CardUsingVerificationEvent;

    private IUnitLogicSystemActions unitLogicSystem;
    private IGameFlowController gameFlowController;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>();
    private List<CardDataInstance> handPile = new List<CardDataInstance>();
    private List<CardDataInstance> gravePile = new List<CardDataInstance>();

    IReadOnlyList<CardDataInstance> ICardSystemProvider.deckCards => deckPile;

    private Queue<CardEffectStrategy> cardSystemActions_BeforeAttack = new Queue<CardEffectStrategy>();
    private Queue<CardEffectStrategy> cardSystemActions_AfterAttack = new Queue<CardEffectStrategy>();
    private Queue<CardEffectStrategy> cardSystemActions_NextTurn = new Queue<CardEffectStrategy>();


    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int drawCardCnt = 5;
    [SerializeField] private int initialDeckCnt = 40;
    [SerializeField] private float cardDrawRate = 1f;
    [SerializeField] private float systemActionRate = 1f;

    public int deckCnt { get; private set; }
    public int graveCnt { get; private set; }
    public int handCnt { get; private set; }

    public void Initialize(IUnitLogicSystemActions _unitLogicSystem, IGameFlowController _gameFlowController)
    {
        unitLogicSystem = _unitLogicSystem;
        gameFlowController = _gameFlowController;
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
        CardData cardData = cardDataBase.GetCardData(2);
        if (cardData == null)
            return;

        ObjectPool<CardDataInstance> pool = cardPools[cardData.id];

        for (int i = 0; i < initialDeckCnt; ++i)
        {
            CardDataInstance card = pool.Get();
            deckPile.Add(card);
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

        var card = deckPile[deckPile.Count - 1];
        deckPile.RemoveAt(deckPile.Count - 1);
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

    public void CardPileDraw(int amount)
    {
        List<CardDataInstance> hands = new List<CardDataInstance>();

        for (int i = 0; i < amount; ++i)
        {
            var card = deckPile[deckPile.Count - 1];
            deckPile.RemoveAt(deckPile.Count - 1);
            handPile.Add(card);
            hands.Add(card);
            --deckCnt;
            ++handCnt;
        }

        CardPileDrawedEvent?.Invoke(hands);
    }

    private IEnumerator CardPileDrawCoroutine(int amount)
    {
        yield return new WaitForSeconds(cardDrawRate);

        CardPileDraw(amount);

        CardDrawFinishedEvent?.Invoke();
        handCnt = 0;
    }

    public void CardUsed(CardDataInstance usedCard)
    {
        //unitLogicSystem에 현재 불릿 카드가 사용 가능한 상태인지 물어봐야 함.
        if (usedCard.GetCardData().cardType == CardType.Bullet)
        {
            if (unitLogicSystem.CanApplyBulletEffect() == false)
            {
                CardUsingVerificationEvent?.Invoke(false);
                return; // 불릿 카드를 더 이상 적용할 수 없는 상태임.
            }
        }

        handPile.Remove(usedCard);
        gravePile.Add(usedCard);
        ++graveCnt;
        CardUsingVerificationEvent?.Invoke(true);
        CardUsedEvent?.Invoke(usedCard.GetCardData());

        StartCoroutine(ExecuteSystemAction_BeforeAttack());
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

        deckCnt = 0;
        graveCnt = 0;
    }

    public void CardUsingFinished()
    {
        CardUsingTurnFinishedEvent?.Invoke();

        for (int i = 0; i < handPile.Count; ++i)
        {
            var card = handPile[i];
            gravePile.Add(card);

        }

        handPile.Clear();

        for (int i = 0; i < gravePile.Count; ++i)
        {
            var card = gravePile[i];
            deckPile.Add(card);
        }

        gravePile.Clear();

        deckCnt = deckPile.Count;
        graveCnt = gravePile.Count;
    }

    public void StartDraw(int waveIdx)
    {
        //한장씩 드로우 할 때의 기능.
        //StartCoroutine(CardDrawCoroutine());

        //Pile 드로우.
        StartCoroutine(CardPileDrawCoroutine(drawCardCnt));
    }

    public void StrategyForwarding(CardEffectStrategy effectStrategy)
    {
        CardSystemActionTimingType timing = effectStrategy.GetCardSystemActionTimingType();

        if (timing == CardSystemActionTimingType.BeforeAttack)
        {
            cardSystemActions_BeforeAttack.Enqueue(effectStrategy);
        }
        else if (timing == CardSystemActionTimingType.AfterAttack)
        {
            cardSystemActions_AfterAttack.Enqueue(effectStrategy);
        }
        else
        {
            cardSystemActions_NextTurn.Enqueue(effectStrategy);
        }
    }

    public void DrawAgain(int drawAmount)
    {
        StartCoroutine(CardPileDrawCoroutine(drawAmount));
    }

    public int GetDeckCnt()
    {
        return deckCnt;
    }

    public int GetHandCnt()
    {
        return handCnt;
    }

    public int GetGraveCnt()
    {
        return graveCnt;
    }

    private IEnumerator ExecuteSystemAction_BeforeAttack()
    {
        while (true)
        {
            if (cardSystemActions_BeforeAttack.Count == 0)
                yield break; // 코루틴 정상 종료

            var systemAction = cardSystemActions_BeforeAttack.Dequeue();

            systemAction.Execute_System();

            yield return new WaitForSeconds(systemActionRate);
        }
    }

    private IEnumerator ExecuteSystemAction_AfterAttack()
    {
        if (cardSystemActions_AfterAttack.Count == 0)
        {
            gameFlowController.PlayerTurnIsFinished();
            yield break; // 코루틴 정상 종료
        }

        var systemAction = cardSystemActions_AfterAttack.Dequeue();

        systemAction.Execute_System();

        yield return new WaitForSeconds(systemActionRate);
    }

    private IEnumerator ExecuteSystemAction_NextTurn()
    {
        if (cardSystemActions_NextTurn.Count == 0)
            yield break; // 코루틴 정상 종료

        var systemAction = cardSystemActions_NextTurn.Dequeue();

        systemAction.Execute_System();

        yield return new WaitForSeconds(systemActionRate);
    }

    public void AttackAgain()
    {
        CardUsingTurnFinishedEvent?.Invoke();
    }

    public void PlayerTurnFinished()
    {
        StartCoroutine(ExecuteSystemAction_AfterAttack());
    }
}
