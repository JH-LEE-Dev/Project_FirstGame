using CardSystemUISignal;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using Unity.AppUI.Core;
using UnityEngine;

public class CardSystemController : MonoBehaviour, ICardSystemControlActionCommandHandler
{
    //CardManager로 가는 Event, Delegate 전부 명령 패턴으로 변경할 것.
    //현재 Equipped Bullet Card에 대해서만 명령 패턴으로 전달됨.
    public event Action<int> CardSlotCntChangedEvent;
    public event Action<CardDataInstance> UsedCardRemovedFromHandEvent;
    public event Action<CardDataInstance> UsedCardToGraveEvent;
    public event Action<CardDataInstance> UsedCardToExtinctionEvent;
    public event Action CardDrawStartEvent;
    public event Action CardDrawFinishedEvent;
    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action ExtinctionToDeckEvent;
    public delegate void CardsToExtinctionDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void CardsRemoveFronHandsDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void EquippedCardsToGraveDelegate(ReadOnlySpan<CardDataInstance> cards);
    public delegate void EquippedCardsToExtinctionDelegate(ReadOnlySpan<CardDataInstance> cards);
    public event CardsToExtinctionDelegate CardsToExtinctionEvent;
    public event CardsRemoveFronHandsDelegate CardsRemoveFronHandsEvent;
    public event EquippedCardsToGraveDelegate EquippedCardsToGraveEvent;
    public event EquippedCardsToExtinctionDelegate EquippedCardsToExtinctionEvent;

    public event Action<CardSystemCommand> SystemCommandDispatchEvent;
    public event Action<CardSystemCommand> SlotSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> StatusCommandDispatchEvent;
    public event Action<CardSystemCommand> ComplexCommandDispatchEvent;

    [SerializeField] private List<CardEffectCommand> cardStatusCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> slotSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> complexSystemCommands = new List<CardEffectCommand>();

    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);

    private List<CardDataInstance> usedCardPile = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    private CardSlotManager cardSlotManager;

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
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else
                ComplexCommandDispatchEvent?.Invoke(command);
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
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else
                ComplexCommandDispatchEvent?.Invoke(command);
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
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.SlotSystem)
                SlotSystemCommandDispatchEvent?.Invoke(command);
            else
                ComplexCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_AfterAttack.Clear();
        //CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardSystemActionCommand_Instant(CardSystemActionCommand command)
    {
        SystemCommandDispatchEvent?.Invoke(command);
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        if (usedCard.GetCardData().cardType != CardType.Bullet)
        {
            if (usedCard.bUpgrade)
                OrginizeCardEffectCommand(usedCard, 0, 1);
            else
                OrginizeCardEffectCommand(usedCard);

            DispatchCardEffect_BeforeAttack();

            UsedCardRemovedFromHandEvent?.Invoke(usedCard);

            if (usedCard.GetCardData().elementType == ElementType.Rotation)
                UsedCardToGraveEvent?.Invoke(usedCard);
            else
                UsedCardToExtinctionEvent?.Invoke(usedCard);
        }
        else
        {
            UsedCardRemovedFromHandEvent?.Invoke(usedCard);
        }
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
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

            OrginizeCardEffectCommand(bulletCardSlot[i][0], bulletCardSlot[i].Count, upgradeNestingCnt);

            //SlotEffect는 가장 먼저 실행되어야 하므로, Dispatch를 for loop 안에서 해줘서 
            //SlotEffect가 적용되게 해야 함, loop안에서 하지 않으려면, 명령 객체가
            //CardDataInstance에 의존해서 nestingCnt,valueModifier를 받아와야 함.
            DispatchCardEffect_AfterAttack();
        }

        //그래서 일단 임시로 Scope를 여기서 Invoke함.
        CardActionEndScopeEvent?.Invoke();
    }

    public void PlayerAttackFinished(PlayerAttackFinishedSignal playerAttackFinishedSignal)
    {
        PlayerTurnFinishedEvent?.Invoke();
    }

    private void OrginizeCardEffectCommand(CardDataInstance usedCard, int nestingCnt = 0, int upgradeNestingCnt = 0)
    {
        //OCP 위반.
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;
        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;
        List<CardSlotSystemEffectType> cardSlotSystemEffectsTypes = usedCard.GetCardData().cardSlotSystemEffects;
        List<ComplexSystemEffectType> complexSystemEffectsTypes = usedCard.GetCardData().complexSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSlotSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = slotSystemCommands[(int)cardSlotSystemEffectsTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < complexSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = complexSystemCommands[(int)complexSystemEffectsTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(nestingCnt, upgradeNestingCnt, usedCard.valueModifier);

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

        CardSystemActionCommand cardSystemActionCommand = cardSystemActionCommands[(int)CardSystemActionType.CardToGrave];
        ActionCommand_CardToGrave toGraveCommand = cardSystemActionCommand as ActionCommand_CardToGrave;

        cardSystemActionCommand = cardSystemActionCommands[(int)CardSystemActionType.CardToExtinction];
        ActionCommand_CardToExtinction toExtinctionCommand = cardSystemActionCommand as ActionCommand_CardToExtinction;

        if (toGraveCommand == null || toExtinctionCommand == null)
        {
            Debug.LogWarning("CardSystemController::ClearAllBulletCard -> Command is null!");
            return;
        }

        int toGraveCnt = 0;
        int toExtinctionCnt = 0;

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                if (bulletCardSlot[i][j].GetCardData().elementType == ElementType.Extinction)
                {
                    ++toGraveCnt;
                    toExtinctionCommand.toExtinctionCards.Add(bulletCardSlot[i][j]);
                    writeBuffer_ToGrave[i] = bulletCardSlot[i][j];
                }
                else
                {
                    ++toExtinctionCnt;
                    toGraveCommand.toGraveCards.Add(bulletCardSlot[i][j]);
                    writeBuffer_ToExtinction[i] = bulletCardSlot[i][j];
                }
            }
        }

        DispatchCardSystemActionCommand_Instant(toExtinctionCommand);
        DispatchCardSystemActionCommand_Instant(toGraveCommand);

        //이건 CardUI가 받을 게 아니고, 슬롯을 가진 UnitUI만 알면 됨.
        //CardUI는 그냥 어떤 식으로든 해당 카드가 묘지나 소멸로 갔다는 것만 알면 됨.
        //CardUI는 그냥 CardsToGrave,CardsToExtinction으로 받기 때문에 안보이게만 하면 됨.
        EquippedCardsToExtinctionEvent?.Invoke(writeBuffer_ToExtinction.Slice(0,toExtinctionCnt));
        EquippedCardsToGraveEvent?.Invoke(writeBuffer_ToGrave.Slice(0,toGraveCnt));

        rentalBuffer_ToGrave.Dispose();
        rentalBuffer_Extinction.Dispose();

        cardSlotManager.ClearAllBulletCard();

        CardActionEndScopeEvent?.Invoke();
    }

    public void GameStarted()
    {
        ExtinctionToDeckEvent?.Invoke();
        CardActionEndScopeEvent?.Invoke();
    }

    //무한 루프 방어 코드 필요. - 도메인 로직이 이를 방어하지만 아키텍쳐에서 방어되지는 않음.
    public void UseCardnExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards)
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
                    OrginizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

                    if (usedCardPile[i].bUpgrade)
                        currentUpgradeNestingCnt = 1;
                    else
                        currentNestingCnt = 1;
                }
            }
        }

        CardsRemoveFronHandsEvent?.Invoke(usingCards);
        CardsToExtinctionEvent?.Invoke(usingCards);

        OrginizeCardEffectCommand(currentCard, currentNestingCnt, currentUpgradeNestingCnt);

        usedCardPile.Clear();
    }
}
