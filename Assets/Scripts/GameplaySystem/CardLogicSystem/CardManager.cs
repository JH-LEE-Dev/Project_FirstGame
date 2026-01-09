using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Pool;


public class CardManager : MonoBehaviour, ICardSystemProvider, ICardStrategyHandler,
    ICardSystemEvent, ICardSystemActions
{
    public event Action CardDrawFinishedEvent;
    public event Action CardUsingTurnFinishedEvent;
    public event Action<CardData> CardUsedEvent;
    public event Action<bool> CardUsingVerificationEvent;

    //외부 의존성
    private IUnitLogicSystemActions unitLogicSystem;
    private IGameFlowController gameFlowController;
    private ICardUICommandSystem cardUICommandSystem;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>(50);
    private List<CardDataInstance> handPile = new List<CardDataInstance>(20);
    private List<CardDataInstance> gravePile = new List<CardDataInstance>(50);

    IReadOnlyList<CardDataInstance> ICardSystemProvider.deckCards => deckPile;
    IReadOnlyList<CardDataInstance> ICardSystemProvider.handCards => handPile;
    IReadOnlyList<CardDataInstance> ICardSystemProvider.graveCards => gravePile;

    private Queue<CardEffectStrategy> cardSystemActions_BeforeAttack = new Queue<CardEffectStrategy>();
    private Queue<CardEffectStrategy> cardSystemActions_AfterAttack = new Queue<CardEffectStrategy>();
    private Queue<CardEffectStrategy> cardSystemActions_NextTurn = new Queue<CardEffectStrategy>();


    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int drawCardCnt = 5;
    [SerializeField] private int initialDeckCnt = 40;

    public int deckCnt { get; private set; }
    public int graveCnt { get; private set; }
    public int handCnt { get; private set; }

    public void Initialize(IUnitLogicSystemActions _unitLogicSystem, IGameFlowController _gameFlowController,
        ICardUICommandSystem _cardUICommandSystem)
    {
        unitLogicSystem = _unitLogicSystem;
        gameFlowController = _gameFlowController;
        cardUICommandSystem = _cardUICommandSystem;
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
        CardData cardData = cardDataBase.GetCardData(3);
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

    public void CardPileDraw(int amount)
    {
        using var temp = new RentalScope<CardDataInstance>(amount);

        Span<CardDataInstance> writeBuffer = temp.Span;

        for (int i = 0; i < amount; ++i)
        {
            var card = deckPile[deckPile.Count - 1];
            deckPile.RemoveAt(deckPile.Count - 1);

            handPile.Add(card);
            writeBuffer[i] = card;
        }

        cardUICommandSystem.CreateCommand(JobType_CardSystemUI.Draw, writeBuffer);
    }

    private void StartCardPileDraw(int amount)
    {
        CardPileDraw(amount);

        cardUICommandSystem.DispatchCommand();
        CardDrawFinishedEvent?.Invoke();
    }

    private void CardAdditionalPileDraw(int amount)
    {
        CardPileDraw(amount);
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

        ExecuteSystemAction_BeforeAttack();
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


    public void HandToGrave()
    {
        for (int i = 0; i < handPile.Count; ++i)
        {
            var card = handPile[i];
            gravePile.Add(card);
        }

        handPile.Clear();
    }

    private void GraveToDeckMove(int moveAmount)
    {
        for (int i = 0; i < gravePile.Count; ++i)
        {
            var card = gravePile[i];
            deckPile.Add(card);
        }

        gravePile.Clear();
    }

    public void StartCardDrawTurn(int waveIdx)
    {
        ExecuteSystemAction_BeforeTurn();
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
        CardAdditionalPileDraw(drawAmount);
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

    private void ExecuteSystemAction_BeforeAttack()
    {
        while (true)
        {
            if (cardSystemActions_BeforeAttack.Count == 0)
                return;

            var systemAction = cardSystemActions_BeforeAttack.Dequeue();

            systemAction.Execute_System();
        }
    }

    private void ExecuteSystemAction_AfterAttack()
    {
        while (true)
        {
            if (cardSystemActions_AfterAttack.Count == 0)
            {
                HandToGrave();
                gameFlowController.PlayerTurnIsFinished();
                cardUICommandSystem.CreateCommand(JobType_CardSystemUI.HandToGrave);
                cardUICommandSystem.DispatchCommand();
                return;
            }

            var systemAction = cardSystemActions_AfterAttack.Dequeue();

            systemAction.Execute_System();
        }
    }

    private void ExecuteSystemAction_BeforeTurn()
    {
        while (true)
        {
            if (cardSystemActions_NextTurn.Count == 0)
            {
                StartCardPileDraw(drawCardCnt);
                return;
            }

            var systemAction = cardSystemActions_NextTurn.Dequeue();

            systemAction.Execute_System();
        }
    }

    public void AttackAgain()
    {
        CardUsingTurnFinishedEvent?.Invoke();
    }

    public void PlayerTurnFinished()
    {
        ExecuteSystemAction_AfterAttack();
    }

    public void CardUsingFinished()
    {
        CardUsingTurnFinishedEvent?.Invoke();
    }
}
