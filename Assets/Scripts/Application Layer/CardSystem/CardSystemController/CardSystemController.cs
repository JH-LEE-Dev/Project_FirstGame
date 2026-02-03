using CardSystemUISignal;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using UnityEngine;

public class CardSystemController : MonoBehaviour, ICardSystemControlActionCommandHandler
{
    public event Action<int> CardSlotCntChangedEvent;
    public event Action CardDrawStartEvent;
    public event Action StartCardUsePhaseEvent;
    public event Action PlayerTurnFinishedEvent;

    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;

    public event Action<CardSystemCommand> CardLogicSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> CardDataControlSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> CardSlotSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> CardStatusCommandDispatchEvent;
    public event Action<CardSystemCommand> CardComplexCommandDispatchEvent;
    public event Action<CardSystemCommand> CardSelectionSystemCommandDispatchEvent;

    [SerializeField] private List<CardEffectCommand> cardLogicSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardDataControlSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardStatusCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSlotSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardComplexSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSelectionSystemCommands = new List<CardEffectCommand>();

    [SerializeField] private List<CardSystemActionCommand> cardLogicSystemActionCommands = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardDataControlSystemActionCommands = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> complexSystemActionCommands = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeCardUsingPhase = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);

    private List<CardDataInstance> usedCardPile = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    private CardSlotManager cardSlotManager;

    private int prevUsedCardCnt;

    private int cardUsePhaseCnt;

    public void Initialize()
    {
        cardSlotManager = new CardSlotManager();

        cardUsePhaseCnt = 1;

        cardSlotManager.Initialize();
        BindEvents();
    }

    private void BindEvents()
    {
        CardSlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
        CardSlotSystemCommandDispatchEvent += cardSlotManager.ExecuteCommand;
        cardSlotManager.CardSlotCntChangedEvent -= CardSlotCntChanged;
        cardSlotManager.CardSlotCntChangedEvent += CardSlotCntChanged;
    }

    private void ReleaseEvents()
    {
        CardSlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
        cardSlotManager.CardSlotCntChangedEvent -= CardSlotCntChanged;
    }

    public void Release()
    {
        ReleaseEvents();
    }

    public CardSlotManager GetCardSlotManager()
    {
        return cardSlotManager;
    }

    public void CardSlotCntChanged(int cnt)
    {
        CardSlotCntChangedEvent?.Invoke(cnt);
    }

    public void StartCardDrawTurn()
    {
        cardSlotManager.ResetSlotCntModifier();

        CardDrawStartEvent?.Invoke();

        DispatchCardEffect_BeforeTurn();

        DispatchCardSystemActionCommand_BeforeTurn();

        StartCardUsePhaseEvent?.Invoke();

        CardActionEndScopeEvent?.Invoke();

        cardSlotManager.ClearAllPrevBulletCard();

        prevUsedCardCnt = 0;
    }

    public void StartCardUsePhase()
    {
        StartCardUsePhaseEvent?.Invoke();
        DispatchCardEffect_BeforeCardUsingPhase();
        CardActionEndScopeEvent?.Invoke();
    }

    public void PlayerTurnFinished()
    {
        --cardUsePhaseCnt;

        if (cardUsePhaseCnt != 0)
        {
            StartCardUsePhase();
            return;
        }

        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.HandCardsToGrave);
        CardActionEndScopeEvent?.Invoke();
        cardUsePhaseCnt = 1;

        PlayerTurnFinishedEvent?.Invoke();
    }

    public void WaveEnded()
    {
        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.HandCardsToGrave);
        CardActionEndScopeEvent?.Invoke();
    }

    public void ResetAllCommands()
    {
        cardEffect_AfterAttack.Clear();
        cardEffect_BeforeAttack.Clear();
        cardEffect_BeforeTurn.Clear();
        cardEffect_BeforeCardUsingPhase.Clear();
    }

    private void DispatchCardSystemActionCommand_BeforeTurn()
    {
        for (int i = 0; i < cardLogicSystemActionCommands.Count; ++i)
        {
            if (cardLogicSystemActionCommands[i].GetCardActionTimingType() == CardSystemActionTimingType.BeforeTurn)
                CardLogicSystemCommandDispatchEvent?.Invoke(cardLogicSystemActionCommands[i]);
        }
    }

    private void DispatchCardEffect_BeforeTurn()
    {
        //OCP 위반.
        for (int i = 0; i < cardEffect_BeforeTurn.Count; ++i)
        {
            var command = cardEffect_BeforeTurn[i];

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeTurn.Clear();
        CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardEffect_BeforeAttack()
    {
        for (int i = 0; i < cardEffect_BeforeAttack.Count; ++i)
        {
            var command = cardEffect_BeforeAttack[i];

            //OCP 위반.
            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeAttack.Clear();
    }

    private void DispatchCardEffect_AfterAttack()
    {
        for (int i = 0; i < cardEffect_AfterAttack.Count; ++i)
        {
            var command = cardEffect_AfterAttack[i];

            //OCP 위반.
            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_AfterAttack.Clear();
    }

    private void DispatchCardEffect_BeforeCardUsingPhase()
    {
        for (int i = 0; i < cardEffect_BeforeCardUsingPhase.Count; ++i)
        {
            var command = cardEffect_BeforeCardUsingPhase[i];

            //OCP 위반.
            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeCardUsingPhase.Clear();
    }

    private void DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        CardSystemActionCommand cardSystemActionCommand = cardLogicSystemActionCommands[(int)type];

        if (cardSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardLogicSystemCommandDispatchEvent?.Invoke(cardSystemActionCommand);
    }

    private void DispatchComplexSystemActionCommand_Instant(ComplexSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        CardSystemActionCommand cardSystemActionCommand = complexSystemActionCommands[(int)type];

        if (cardSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardComplexCommandDispatchEvent?.Invoke(cardSystemActionCommand);
    }

    private void DispatchCardDataControlSystemActionCommand_Instant(CardDataControlSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        CardSystemActionCommand cardDataControlSystemActionCommand = cardDataControlSystemActionCommands[(int)type];

        if (cardDataControlSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardDataControlSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardDataControlSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardDataControlSystemCommandDispatchEvent?.Invoke(cardDataControlSystemActionCommand);
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        ++prevUsedCardCnt;

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (usedCard.GetCardData().cardType != CardType.Bullet)
        {
            if (usedCard.IsUpgraded())
                OrganizeCardEffectCommand(usedCard, 0, 1);
            else
                OrganizeCardEffectCommand(usedCard, 1);

            writeBuffer[0] = usedCard;
            DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.UsedCardsRemoveFromHand, writeBuffer);

            DispatchCardEffect_BeforeAttack();

            if (usedCard.GetCardData().elementType == ElementType.Rotation)
            {
                writeBuffer[0] = usedCard;
                DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.UsedCardsToGrave, writeBuffer);
            }
            else
            {
                writeBuffer[0] = usedCard;
                DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.UsedCardsToExtinction, writeBuffer);
            }

            CardActionEndScopeEvent?.Invoke();
        }
        else
        {
            writeBuffer[0] = usedCard;
            DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.UsedCardsRemoveFromHand, writeBuffer);
        }
    }

    public void CardUsingFinished()
    {
        cardSlotManager.SortCardSlot();
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            int upgradeNestingCnt = 0;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                if (bulletCardSlot[i][j].IsUpgraded())
                    ++upgradeNestingCnt;
            }

            OrganizeCardEffectCommand(bulletCardSlot[i][0], bulletCardSlot[i].Count, upgradeNestingCnt);

            //SlotEffect는 가장 먼저 실행되어야 하므로, Dispatch를 for loop 안에서 해줘서 
            //SlotEffect가 적용되게 해야 함, loop안에서 하지 않으려면, 명령 객체가
            //CardDataInstance에 의존해서 nestingCnt,valueModifier를 받아와야 함.
            DispatchCardEffect_AfterAttack();
        }

        DispatchComplexSystemActionCommand_Instant(ComplexSystemActionType.HandPileExistEffectExecute);

        //그래서 일단 임시로 Scope를 여기서 Invoke함.
        CardActionEndScopeEvent?.Invoke();
    }

    private void OrganizeCardEffectCommand(CardDataInstance usedCard, int nestingCnt = 0, int upgradeNestingCnt = 0)
    {
        //OCP 위반.
        List<CardLogicSystemEffectType> cardLogicSystemEffectTypes = usedCard.GetCardData().cardLogicSystemEffects;
        List<CardDataControlSystemEffectType> cardDataControlSystemEffectTypes = usedCard.GetCardData().cardDataControlSystemEffects;
        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;
        List<CardSlotSystemEffectType> cardSlotSystemEffectsTypes = usedCard.GetCardData().cardSlotSystemEffects;
        List<ComplexSystemEffectType> complexSystemEffectsTypes = usedCard.GetCardData().complexSystemEffects;
        List<CardSelectionSystemEffectType> selectionSystemEffectTypes = usedCard.GetCardData().selectionSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardLogicSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardLogicSystemCommands[(int)cardLogicSystemEffectTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSlotSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSlotSystemCommands[(int)cardSlotSystemEffectsTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < complexSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardComplexSystemCommands[(int)complexSystemEffectsTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < selectionSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSelectionSystemCommands[(int)selectionSystemEffectTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }
    }

    private void InsertCommandToList(CardSystemActionTimingType timingType, CardEffectCommand command)
    {
        if (timingType == CardSystemActionTimingType.BeforeAttack)
        {
            cardEffect_BeforeAttack.Add(command);
        }
        else if (timingType == CardSystemActionTimingType.AfterAttack)
        {
            cardEffect_AfterAttack.Add(command);
        }
        else if (timingType == CardSystemActionTimingType.BeforeTurn)
        {
            cardEffect_BeforeTurn.Add(command);
        }
        else if (timingType == CardSystemActionTimingType.BeforeCardUsingPhase)
        {
            cardEffect_BeforeCardUsingPhase.Add(command);
        }
    }

    public CardUsedResult TryCardUse(ICardDataInstanceProvider usedCard)
    {
        if (usedCard is CardDataInstance card == false)
            return default;

        CardUsedResult result;

        ICardDataProvider usedCardData = usedCard.GetCardDataProvider();

        if (usedCardData.cardType == CardType.Bullet)
        {
            BulletCardUsedResult bulletCardUseResult = cardSlotManager.InsertCardToSlot(card);

            if (bulletCardUseResult.bVerified == false)
            {
                result.bVerified = false;
                result.slotIdx = -1;
                result.usedCard = null;
            }
            else
            {
                result.bVerified = true;
                result.slotIdx = bulletCardUseResult.slotIdx;
                result.usedCard = card;

                CardUsed(card);
            }
        }
        else
        {
            CardUsed(card);

            result.bVerified = true;
            result.slotIdx = -1;
            result.usedCard = card;
        }

        return result;
    }

    public void DiscardBulletCard(int slotIdx)
    {
        var cards = cardSlotManager.GetBulletCardSpecificSlot(slotIdx);

        using var rentalBuffer = new RentalScope<CardDataInstance>(cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < cards.Count; ++i)
        {
            writeBuffer[i] = cards[i];
        }

        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.CardsToHand, writeBuffer);

        cardSlotManager.DiscardBulletCard(slotIdx);
    }

    public void ClearAllBulletCard()
    {
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        using var rentalBuffer_ToGrave = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_ToGrave = rentalBuffer_ToGrave.Span;

        using var rentalBuffer_Extinction = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer_ToExtinction = rentalBuffer_Extinction.Span;

        int toGraveCnt = 0;
        int toExtinctionCnt = 0;

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                if (bulletCardSlot[i][j].GetCardData().elementType != ElementType.Extinction)
                {
                    writeBuffer_ToGrave[toGraveCnt] = bulletCardSlot[i][j];
                    ++toGraveCnt;
                }
                else
                {
                    writeBuffer_ToExtinction[toExtinctionCnt] = bulletCardSlot[i][j];
                    ++toExtinctionCnt;
                }
            }
        }

        if (toExtinctionCnt != 0)
            DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.SlotCardsToExtinction, writeBuffer_ToExtinction.Slice(0, toExtinctionCnt));
        if (toGraveCnt != 0)
            DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.SlotCardsToGrave, writeBuffer_ToGrave.Slice(0, toGraveCnt));

        cardSlotManager.ClearAllBulletCard();
        CardActionEndScopeEvent?.Invoke();
    }

    public void GameStarted()
    {
        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.ResetCardPiles);
        CardActionEndScopeEvent?.Invoke();
    }

    //무한 루프 방어 코드 필요. - 도메인 로직이 이를 방어하지만 아키텍쳐에서 방어되지는 않음.
    public void UseCards(ReadOnlySpan<CardDataInstance> usingCards)
    {
        if (usingCards == null || usingCards.Length == 0)
            return;

        for (int i = 0; i < usingCards.Length; ++i)
        {
            if (usingCards[i] != null)
            {
                usedCardPile.Add(usingCards[i]);
            }
        }

        usedCardPile.Sort(new CardIdComparer());

        var currentCard = usedCardPile[0];
        int currentNestingCnt = 1;
        int currentUpgradeNestingCnt = 0;

        if (currentCard.IsUpgraded())
            ++currentUpgradeNestingCnt;

        for (int i = 1; i < usedCardPile.Count; ++i)
        {
            if (usedCardPile[i] != null)
            {
                if (currentCard.GetCardData().id == usedCardPile[i].GetCardData().id)
                {
                    if (usedCardPile[i].IsUpgraded())
                        ++currentUpgradeNestingCnt;
                    else
                        ++currentNestingCnt;
                }
                else
                {
                    currentCard = usedCardPile[i];
                    OrganizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

                    if (usedCardPile[i].IsUpgraded())
                        currentUpgradeNestingCnt = 1;
                    else
                        currentNestingCnt = 1;
                }
            }
        }

        OrganizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

        usedCardPile.Clear();
    }

    public void RequestCardLogicSystemActionCommand(CardLogicSystemActionType cardLogicSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType)
    {
        DispatchCardSystemActionCommand_Instant(cardLogicSystemActionType, _cards);

        CardActionEndScopeEvent?.Invoke();
    }

    public void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType)
    {
        DispatchCardDataControlSystemActionCommand_Instant(cardDataControlSystemActionType, _cards, _cardSystemContextType);

        CardActionEndScopeEvent?.Invoke();
    }

    public int GetPrevUsedCardCnt()
    {
        return prevUsedCardCnt;
    }

    public void ApplyCardUsePhaseCntModifier(int cnt)
    {
        cardUsePhaseCnt += cnt;
    }

    public void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards)
    {
        if (cards.Length == 0)
            return;

        for (int i = 0; i < cards.Length; ++i)
        {
            var command = cards[i].GetCardData().HandPileExistEffect;

            if (command == null)
                continue;

            if (cards[i].IsUpgraded())
                command.InitializeCommand(0, 1, 1);
            else
                command.InitializeCommand(1, 0, 1);

            //OCP 위반.
            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command);
        }
    }
}
