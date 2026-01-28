using CardSystemUISignal;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using UnityEngine;

public class CardSystemController : MonoBehaviour, ICardSystemControlActionCommandHandler
{
    public event Action<int> CardSlotCntChangedEvent;
    public event Action CardDrawStartEvent;
    public event Action CardDrawFinishedEvent;

    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;

    public event Action<CardSystemCommand> SystemCommandDispatchEvent;
    public event Action<CardSystemCommand> SlotSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> StatusCommandDispatchEvent;
    public event Action<CardSystemCommand> ComplexCommandDispatchEvent;
    public event Action<CardSystemCommand> SelectionSystemCommandDispatchEvent;

    [SerializeField] private List<CardEffectCommand> cardStatusCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> slotSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> complexSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSelectionSystemCommands = new List<CardEffectCommand>();

    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);

    private List<CardDataInstance> usedCardPile = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    private CardSlotManager cardSlotManager;

    private int prevUsedCardCnt;

    public void Initialize()
    {
        cardSlotManager = new CardSlotManager();

        cardSlotManager.Initialize();
        BindEvents();
    }

    private void BindEvents()
    {
        SlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
        SlotSystemCommandDispatchEvent += cardSlotManager.ExecuteCommand;
        cardSlotManager.CardSlotCntChangedEvent -= CardSlotCntChanged;
        cardSlotManager.CardSlotCntChangedEvent += CardSlotCntChanged;
    }

    private void ReleaseEvents()
    {
        SlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
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

        CardDrawFinishedEvent?.Invoke();

        CardActionEndScopeEvent?.Invoke();

        cardSlotManager.ClearAllPrevBulletCard();

        prevUsedCardCnt = 0;
    }

    private void DispatchCardSystemActionCommand_BeforeTurn()
    {
        for (int i = 0; i < cardSystemActionCommands.Count; ++i)
        {
            if (cardSystemActionCommands[i].GetCardActionTimingType() == CardSystemActionTimingType.BeforeTurn)
                SystemCommandDispatchEvent?.Invoke(cardSystemActionCommands[i]);
        }
    }

    private void DispatchCardEffect_BeforeTurn()
    {
        //OCP 위반.
        for (int i = 0; i < cardEffect_BeforeTurn.Count; ++i)
        {
            var command = cardEffect_BeforeTurn[i];

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                ComplexCommandDispatchEvent?.Invoke(command);
            else
                SelectionSystemCommandDispatchEvent?.Invoke(command);
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
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                ComplexCommandDispatchEvent?.Invoke(command);
            else
                SelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeAttack.Clear();
        CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardEffect_AfterAttack()
    {
        for (int i = 0; i < cardEffect_AfterAttack.Count; ++i)
        {
            var command = cardEffect_AfterAttack[i];

            //OCP 위반.
            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.StatusSystem)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.ComplexSystem)
                ComplexCommandDispatchEvent?.Invoke(command);
            else
                SelectionSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_AfterAttack.Clear();
    }

    private void DispatchCardSystemActionCommand_Instant(CardSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default)
    {
        CardSystemActionCommand cardSystemActionCommand = cardSystemActionCommands[(int)type];

        if (cardSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::SetupCardSystemActionCommand -> Command is null!");
            return;
        }

        cardSystemActionCommand.InitializeCommand(cards);

        SystemCommandDispatchEvent?.Invoke(cardSystemActionCommand);
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        ++prevUsedCardCnt;

        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (usedCard.GetCardData().cardType != CardType.Bullet)
        {
            if (usedCard.bUpgrade)
                OrganizeCardEffectCommand(usedCard, 0, 1);
            else
                OrganizeCardEffectCommand(usedCard, 1);

            DispatchCardEffect_BeforeAttack();

            writeBuffer[0] = usedCard;
            DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsRemoveFromHand, writeBuffer.Slice(0, 1));

            if (usedCard.GetCardData().elementType == ElementType.Rotation)
            {
                writeBuffer[0] = usedCard;
                DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsToGrave, writeBuffer.Slice(0, 1));
            }
            else
            {
                writeBuffer[0] = usedCard;
                DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsToExtinction, writeBuffer.Slice(0, 1));
            }
        }
        else
        {
            writeBuffer[0] = usedCard;
            DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsRemoveFromHand, writeBuffer.Slice(0, 1));
        }

        rentalBuffer.Dispose();
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
                if (bulletCardSlot[i][j].bUpgrade)
                    ++upgradeNestingCnt;
            }

            OrganizeCardEffectCommand(bulletCardSlot[i][0], bulletCardSlot[i].Count, upgradeNestingCnt);

            //SlotEffect는 가장 먼저 실행되어야 하므로, Dispatch를 for loop 안에서 해줘서 
            //SlotEffect가 적용되게 해야 함, loop안에서 하지 않으려면, 명령 객체가
            //CardDataInstance에 의존해서 nestingCnt,valueModifier를 받아와야 함.
            DispatchCardEffect_AfterAttack();
        }

        //그래서 일단 임시로 Scope를 여기서 Invoke함.
        CardActionEndScopeEvent?.Invoke();
    }

    private void OrganizeCardEffectCommand(CardDataInstance usedCard, int nestingCnt = 0, int upgradeNestingCnt = 0)
    {
        //OCP 위반.
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;
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

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSlotSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = slotSystemCommands[(int)cardSlotSystemEffectsTypes[i]];

            effectCommand.InitializeCommand(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < complexSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = complexSystemCommands[(int)complexSystemEffectsTypes[i]];

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
        else
        {
            cardEffect_BeforeTurn.Add(command);
        }
    }

    public CardUsedResult TryCardUse(CardDataInstance usedCard)
    {
        CardUsedResult result;

        CardData usedCardData = usedCard.GetCardData();

        if (usedCardData.cardType == CardType.Bullet)
        {
            BulletCardUsedResult bulletCardUseResult = cardSlotManager.InsertCardToSlot(usedCard);

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
                result.usedCard = usedCard;

                CardUsed(usedCard);
            }
        }
        else
        {
            CardUsed(usedCard);

            result.bVerified = true;
            result.slotIdx = -1;
            result.usedCard = usedCard;
        }

        return result;
    }

    public void DiscardBulletCard(int slotIdx)
    {
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
                    ++toGraveCnt;
                    writeBuffer_ToGrave[i] = bulletCardSlot[i][j];
                }
                else
                {
                    ++toExtinctionCnt;
                    writeBuffer_ToExtinction[i] = bulletCardSlot[i][j];
                }
            }
        }

        if (toExtinctionCnt != 0)
            DispatchCardSystemActionCommand_Instant(CardSystemActionType.SlotCardsToExtinction, writeBuffer_ToExtinction.Slice(0, toExtinctionCnt));
        if (toGraveCnt != 0)
            DispatchCardSystemActionCommand_Instant(CardSystemActionType.SlotCardsToGrave, writeBuffer_ToGrave.Slice(0, toGraveCnt));

        rentalBuffer_ToGrave.Dispose();
        rentalBuffer_Extinction.Dispose();

        cardSlotManager.ClearAllBulletCard();
        CardActionEndScopeEvent?.Invoke();
    }

    public void GameStarted()
    {
        DispatchCardSystemActionCommand_Instant(CardSystemActionType.ResetCardPiles);
        CardActionEndScopeEvent?.Invoke();
    }

    //무한 루프 방어 코드 필요. - 도메인 로직이 이를 방어하지만 아키텍쳐에서 방어되지는 않음.
    public void UseCardsAndExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards)
    {
        if (usingCards == null || usingCards[0] == null)
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

        if (currentCard.bUpgrade)
            ++currentUpgradeNestingCnt;

        for (int i = 1; i < usedCardPile.Count; ++i)
        {
            if (usedCardPile[i] != null)
            {
                if (currentCard.GetCardData().id == usedCardPile[i].GetCardData().id)
                {
                    if (usedCardPile[i].bUpgrade)
                        ++currentUpgradeNestingCnt;
                    else
                        ++currentNestingCnt;
                }
                else
                {

                    currentCard = usedCardPile[i];
                    OrganizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

                    if (usedCardPile[i].bUpgrade)
                        currentUpgradeNestingCnt = 1;
                    else
                        currentNestingCnt = 1;
                }
            }
        }

        DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsRemoveFromHand, usingCards);
        DispatchCardSystemActionCommand_Instant(CardSystemActionType.UsedCardsToExtinction, usingCards);

        OrganizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

        usedCardPile.Clear();
    }

    public void RequestCardSystemActionCommand(CardSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards)
    {
        DispatchCardSystemActionCommand_Instant(cardSystemActionType, _cards);

        CardActionEndScopeEvent?.Invoke();
    }

    public int GetPrevUsedCardCnt()
    {
        return prevUsedCardCnt;
    }
}
