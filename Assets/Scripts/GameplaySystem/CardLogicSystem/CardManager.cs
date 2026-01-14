using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.GPUSort;


public class CardManager : MonoBehaviour, ICardSystemActions, ICardEffectCommandHandler,
    ICardSystemEvents, ICardSystemFlowActions,ICardSystemData
{
    public event Action CardDrawFinishedEvent;
    public event Action CardUsingTurnFinishedEvent;
    public event Action<CardDataInstance> CardUsedEvent;
    public event Action<bool> CardUsingVerificationEvent;
    public event Action CardDrawedEvent;

    //외부 의존성
    private IUnitLogicSystemActions unitLogicSystem;
    private IGameFlowController gameFlowController;
    private ICardUICommandSystem cardUICommandSystem;

    private Dictionary<int, ObjectPool<CardDataInstance>> cardPools
    = new Dictionary<int, ObjectPool<CardDataInstance>>();

    private List<CardDataInstance> deckPile = new List<CardDataInstance>(50);
    private List<CardDataInstance> handPile = new List<CardDataInstance>(20);
    private List<CardDataInstance> gravePile = new List<CardDataInstance>(50);

    IReadOnlyList<CardDataInstance> ICardSystemData.deckCards => deckPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.handCards => handPile;
    IReadOnlyList<CardDataInstance> ICardSystemData.graveCards => gravePile;

    private Queue<CardEffectSystemCommand> cardSystemActions_BeforeAttack = new Queue<CardEffectSystemCommand>();
    private Queue<CardEffectSystemCommand> cardSystemActions_AfterAttack = new Queue<CardEffectSystemCommand>();
    private Queue<CardEffectSystemCommand> cardSystemActions_NextTurn = new Queue<CardEffectSystemCommand>();


    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private int drawCardCnt = 5;
    [SerializeField] private int initialDeckCnt = 40;

    public int deckCnt { get; private set; }
    public int graveCnt { get; private set; }
    public int handCnt { get; private set; }

    private int attackCnt = 1;

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

    private void OnDestroy()
    {
        CardDrawFinishedEvent = null;
        CardUsingTurnFinishedEvent = null;
        CardUsedEvent = null;
        CardUsingVerificationEvent = null;
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

        if (bAdditional == false)
            CreateUICommand(JobType_CardSystemUI.Draw, rentalBuffer);
        else
            CreateUICommand(JobType_CardSystemUI.AdditionalDraw, rentalBuffer);

        if (deckPile.Count == 0 && gravePile.Count != 0 && restDrawCnt != 0)
        {
            GraveToDeckMove();
            CardPileDraw(restDrawCnt, false);
        }
    }

    private void CreateUICommand(JobType_CardSystemUI jobType, RentalScope<CardDataInstance> cardsBuffer)
    {
        try
        {
            cardUICommandSystem.CreateCommand(jobType, cardsBuffer.Span);
        }
        finally
        {

            cardsBuffer.Dispose();
        }
    }

    private void StartCardPileDraw(int amount)
    {
        CardDrawedEvent?.Invoke();
        CardPileDraw(amount, false);

        cardUICommandSystem.DispatchCommand();
        CardDrawFinishedEvent?.Invoke();
    }

    private void CardAdditionalPileDraw(int amount)
    {
        CardPileDraw(amount, true);

        cardUICommandSystem.DispatchCommand();
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
        CardUsedEvent?.Invoke(usedCard);

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

    private void GraveToDeckMove()
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < gravePile.Count; ++i)
        {
            var card = gravePile[i];
            deckPile.Add(card);
        }

        gravePile.Clear();

        CreateUICommand(JobType_CardSystemUI.GraveToDeck, rentalBuffer);
    }

    public void StartCardDrawTurn(int waveIdx)
    {
        attackCnt = 1;
        ExecuteSystemAction_BeforeTurn();
    }

    public void InsertCommand(CardEffectSystemCommand effectCommand)
    {
        CardSystemActionTimingType timing = effectCommand.GetCardSystemActionTimingType();

        if (timing == CardSystemActionTimingType.BeforeAttack)
        {
            cardSystemActions_BeforeAttack.Enqueue(effectCommand);
        }
        else if (timing == CardSystemActionTimingType.AfterAttack)
        {
            cardSystemActions_AfterAttack.Enqueue(effectCommand);
        }
        else
        {
            cardSystemActions_NextTurn.Enqueue(effectCommand);
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

            systemAction.Execute(this);
        }
    }

    private void ExecuteSystemAction_AfterAttack()
    {
        while (true)
        {
            if (cardSystemActions_AfterAttack.Count == 0)
            {
                return;
            }

            var systemAction = cardSystemActions_AfterAttack.Dequeue();

            systemAction.Execute(this);
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

            systemAction.Execute(this);
        }
    }

    public void AttackAgain()
    {
        ++attackCnt;
    }

    public void PlayerTurnFinished()
    {
        ExecuteSystemAction_AfterAttack();

        if (CheckRemainingAttacks() == true)
        {
            HandToGrave();
            gameFlowController.PlayerTurnIsFinished();
            cardUICommandSystem.CreateCommand(JobType_CardSystemUI.HandToGrave);
            cardUICommandSystem.DispatchCommand();
        }
        else
        {
            CardUsingTurnFinishedEvent?.Invoke();
        }
    }

    private bool CheckRemainingAttacks()
    {
        --attackCnt;

        if (attackCnt < 0)
            attackCnt = 0;

        return attackCnt == 0;
    }

    public void CardUsingFinished()
    {
        CardUsingTurnFinishedEvent?.Invoke();
    }
}
