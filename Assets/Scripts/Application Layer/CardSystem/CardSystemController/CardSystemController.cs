using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemController : MonoBehaviour, ICardSystemControlActionCommandHandler
{
    public event Action<int> CardSlotCntChangedEvent;
    public event Action CardDrawStartEvent;
    public event Action StartCardUsePhaseEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action StartAfterCardUsePhaseEvent;
    public event Action<CardUsingCondition> CardUsingConditionCheckEvent;

    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;

    public event Action<CardSystemCommand, bool> CardLogicSystemCommandDispatchEvent;
    public event Action<CardSystemCommand, bool> CardDataControlSystemCommandDispatchEvent;
    public event Action<CardSystemCommand, bool> CardSlotSystemCommandDispatchEvent;
    public event Action<CardSystemCommand, bool> CardStatusCommandDispatchEvent;
    public event Action<CardSystemCommand, bool> CardComplexCommandDispatchEvent;
    public event Action<CardSystemCommand, bool> CardSelectionSystemCommandDispatchEvent;

    [SerializeField] private List<CardSystemActionCommand> cardLogicSystemActionCommands = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardDataControlSystemActionCommands = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> complexSystemActionCommands = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_BeforeCardUsingPhase = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);
    private List<CardEffectCommand> cardEffect_AfterCardUsingPhase = new List<CardEffectCommand>(SYSTEM_VAR.maxDeckPileCount);

    private CardSlotManager cardSlotManager;

    public delegate void CardLogicSystemCommandCreator(ReadOnlySpan<CardDataInstance> cards = default);
    private CardLogicSystemCommandCreator[] cardLogicSystemCreatorMap;
    public delegate void CardDataControlSystemCommandCreator(ReadOnlySpan<CardDataInstance> cards = default);
    private CardDataControlSystemCommandCreator[] cardDataControlSystemCreatorMap;

    private int prevUsedCardCnt;
    private int cardUsePhaseCnt;
    bool bCardUsingFinished = false;

    bool bEffectApplyStateChanged = false;

    public void Initialize()
    {
        cardSlotManager = new CardSlotManager();

        cardUsePhaseCnt = 1;

        cardSlotManager.Initialize();
        BindEvents();

        ReadyCreatorMap();
    }

    private void ReadyCreatorMap()
    {
        cardLogicSystemCreatorMap = new CardLogicSystemCommandCreator[(int)CardLogicSystemEventType.MAX];
        cardDataControlSystemCreatorMap = new CardDataControlSystemCommandCreator[(int)CardDataControlSystemEventType.MAX];

        //Card Logic System 맵 할당
        //CardSystemContext로 좀 더 세분화하여 HandleSlotEffectsWhenHandChanged함수 호출하기.
        //현재 CardsToExtinction같은 경우에 패에 있는 카드가 extinction으로 가지 않았음에도 호출됨.
        BindLogic(CardLogicSystemEventType.CardPileDrawEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.CardAdditionalDrawEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.HandCardsToGraveEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.CardsToExtinctionEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.GraveCardsToHandEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.CardsToGraveEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.CardsToHandEvent, HandleSlotEffectsWhenHandChanged);
        BindLogic(CardLogicSystemEventType.CardsToDeckEvent, HandleSlotEffectsWhenHandChanged);

        //Card Data Control System 맵 할당
        BindData(CardDataControlSystemEventType.CardsUpgraded, HandleSlotEffectsWhenHandChanged);
        BindData(CardDataControlSystemEventType.CardsValueModified, HandleCardValueChanged);

        void BindLogic(CardLogicSystemEventType type, CardLogicSystemCommandCreator action)
            => cardLogicSystemCreatorMap[(int)type] = action;

        void BindData(CardDataControlSystemEventType type, CardDataControlSystemCommandCreator action)
            => cardDataControlSystemCreatorMap[(int)type] = action;
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

    public void StartGame()
    {
        DispatchCardSystemActionCommand_GameStarted();
    }

    public void StartCardDrawTurn()
    {
        PostPoneBeforeUsingPhaseCommand();

        cardSlotManager.ResetSlotCntModifier();

        CardDrawStartEvent?.Invoke();

        DispatchCardSystemActionCommand_BeforeTurn();

        DispatchCardEffect_BeforeTurn();

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
        bCardUsingFinished = false;

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

    private void PostPoneBeforeUsingPhaseCommand()
    {
        for (int i = 0; i < cardEffect_BeforeCardUsingPhase.Count; ++i)
        {
            cardEffect_BeforeTurn.Add(cardEffect_BeforeCardUsingPhase[i]);
        }

        cardEffect_BeforeCardUsingPhase.Clear();
    }

    public void WaveEnded()
    {
        cardUsePhaseCnt = 1;
        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.HandCardsToGrave);
        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.ResetCardPiles);
        CardActionEndScopeEvent?.Invoke();
    }

    public void ResetAllCommands()
    {
        cardEffect_AfterAttack.Clear();
        cardEffect_BeforeAttack.Clear();
        cardEffect_BeforeTurn.Clear();
        cardEffect_BeforeCardUsingPhase.Clear();
    }

    private void DispatchCardSystemActionCommand_GameStarted()
    {
        for (int i = 0; i < cardLogicSystemActionCommands.Count; ++i)
        {
            if (cardLogicSystemActionCommands[i].GetGameSystemActionTimingType() == GameSystemActionTimingType.GameStarted)
                CardLogicSystemCommandDispatchEvent?.Invoke(cardLogicSystemActionCommands[i], false);
        }
    }

    private void DispatchCardSystemActionCommand_BeforeTurn()
    {
        for (int i = 0; i < cardLogicSystemActionCommands.Count; ++i)
        {
            if (cardLogicSystemActionCommands[i].GetGameSystemActionTimingType() == GameSystemActionTimingType.BeforeTurn)
                CardLogicSystemCommandDispatchEvent?.Invoke(cardLogicSystemActionCommands[i], false);
        }
    }

    private void DispatchCardEffect_BeforeTurn()
    {
        //OCP 위반.
        for (int i = 0; i < cardEffect_BeforeTurn.Count; ++i)
        {
            var command = cardEffect_BeforeTurn[i];

            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);

            command.ResetCommandData();
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
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);

            command.ResetCommandData();
        }

        cardEffect_BeforeAttack.Clear();
    }

    private void DispatchCardEffect_AfterAttack()
    {
        for (int i = 0; i < cardEffect_AfterAttack.Count; ++i)
        {
            var command = cardEffect_AfterAttack[i];

            //OCP 위반.
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);
        }

        cardEffect_AfterAttack.Clear();
    }

    private void DispatchCardEffect_AfterCardUsingPhase()
    {
        for (int i = 0; i < cardEffect_AfterCardUsingPhase.Count; ++i)
        {
            var command = cardEffect_AfterCardUsingPhase[i];

            //OCP 위반.
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);

            command.ResetCommandData();
        }

        cardEffect_AfterCardUsingPhase.Clear();
    }

    private void DispatchCardEffect_BeforeCardUsingPhase()
    {
        for (int i = 0; i < cardEffect_BeforeCardUsingPhase.Count; ++i)
        {
            var command = cardEffect_BeforeCardUsingPhase[i];

            //OCP 위반.
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);

            command.ResetCommandData();
        }

        cardEffect_BeforeCardUsingPhase.Clear();
    }

    private void DispatchCardEffect_AfterAttack_Undo()
    {
        for (int i = 0; i < cardEffect_AfterAttack.Count; ++i)
        {
            var command = cardEffect_AfterAttack[i];

            //OCP 위반.
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, true);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, true);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, true);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, true);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, true);
        }

        cardEffect_AfterAttack.Clear();
    }

    private void DirectOrganizeAndDispatch_AfterAttack(CardDataInstance usedCard)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardStatusCommandDispatchEvent?.Invoke(cardStatusEffects[i], false);

            cardStatusEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardLogicSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], false);

            cardLogicSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardSlotSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], false);

            cardSlotSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardComplexCommandDispatchEvent?.Invoke(cardStatusEffects[i], false);

            complexSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardSelectionSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], false);

            selectionSystemEffects[i].ResetCommandData();
        }
    }

    private void DirectOrganizeAndDispatch_AfterAttack_Undo(CardDataInstance usedCard)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardStatusCommandDispatchEvent?.Invoke(cardStatusEffects[i], true);

            cardStatusEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardLogicSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], true);

            cardLogicSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardSlotSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], true);

            cardSlotSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardComplexCommandDispatchEvent?.Invoke(cardStatusEffects[i], true);

            complexSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                CardSelectionSystemCommandDispatchEvent?.Invoke(cardStatusEffects[i], true);

            selectionSystemEffects[i].ResetCommandData();
        }
    }

    private void DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        CardSystemActionCommand cardSystemActionCommand = cardLogicSystemActionCommands[(int)type];

        if (cardSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardLogicSystemCommandDispatchEvent?.Invoke(cardSystemActionCommand, false);
    }

    private void DispatchComplexSystemActionCommand_Instant(ComplexSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        CardSystemActionCommand cardSystemActionCommand = complexSystemActionCommands[(int)type];

        if (cardSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardComplexCommandDispatchEvent?.Invoke(cardSystemActionCommand, false);
    }

    private void DispatchCardDataControlSystemActionCommand_Instant(CardDataControlSystemActionType type, ReadOnlySpan<CardDataInstance> cards = default, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        CardSystemActionCommand cardDataControlSystemActionCommand = cardDataControlSystemActionCommands[(int)type];

        if (cardDataControlSystemActionCommand == null)
        {
            Debug.LogWarning("CardSystemController::DispatchCardDataControlSystemActionCommand_Instant -> Command is null!");
            return;
        }

        cardDataControlSystemActionCommand.InitializeCommand(cards, _cardSystemContextType);

        CardDataControlSystemCommandDispatchEvent?.Invoke(cardDataControlSystemActionCommand, false);
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        ++prevUsedCardCnt;

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (usedCard.GetCardData().cardType != CardType.Bullet && usedCard.GetCardData().cardType != CardType.Inherence)
        {
            OrganizeCardEffectCommand(usedCard);

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

    public void UndoAfterAttackEffets()
    {
        //cardSlotManager.ReverseSortCardSlot();
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                OrganizeCardEffectCommand_AfterAttack(bulletCardSlot[i][j]);

                DispatchCardEffect_AfterAttack_Undo();
            }
        }
    }

    private void ApplyAfterAttackEffects()
    {
        CheckingEffectCondition_AfterAttack();

        //cardSlotManager.SortCardSlot();
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                OrganizeCardEffectCommand_AfterAttack(bulletCardSlot[i][j]);

                DispatchCardEffect_AfterAttack();
            }
        }
    }

    private void EffectApplyStateChanged()
    {
        bEffectApplyStateChanged = true;

        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                CommandConditionCheck_AfterAttack(bulletCardSlot[i][j]);
            }
        }
    }

    public void UndoAfterAttackEffets_ForEffectConditionCheck()
    {
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                OrganizeCardEffectCommand_AfterAttack(bulletCardSlot[i][j]);

                DispatchCardEffect_AfterAttack_Undo();
            }
        }
    }

    private void CheckingEffectCondition_AfterAttack()
    {
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        while (bEffectApplyStateChanged == true)
        {
            for (int i = 0; i < bulletCardSlot.Count; ++i)
            {
                if (bulletCardSlot[i].Count == 0)
                    continue;

                for (int j = 0; j < bulletCardSlot[i].Count; ++j)
                {
                    OrganizeCardEffectCommand_AfterAttack(bulletCardSlot[i][j], EffectApplyStateChanged);

                    DispatchCardEffect_AfterAttack();
                }
            }

            UndoAfterAttackEffets_ForEffectConditionCheck();

            bEffectApplyStateChanged = false;
        }
    }

    private void ApplyWithoutAfterAttackEffects()
    {
        cardSlotManager.SortCardSlot();
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            if (bulletCardSlot[i].Count == 0)
                continue;

            for (int j = 0; j < bulletCardSlot[i].Count; ++j)
            {
                OrganizeCardEffectCommand_WithoutAfterAttack(bulletCardSlot[i][j]);
            }
        }
    }

    public void CardUsingFinished()
    {
        bCardUsingFinished = true;

        UndoAfterAttackEffets();
        ApplyAfterAttackEffects();

        ApplyWithoutAfterAttackEffects();
        DispatchCardEffect_AfterCardUsingPhase();
        DispatchComplexSystemActionCommand_Instant(ComplexSystemActionType.HandPileExistEffectExecute);

        StartAfterCardUsePhaseEvent?.Invoke();
        CardActionEndScopeEvent?.Invoke();
    }

    private void UndoCardEffect_AfterAttack(CardDataInstance _card)
    {
        OrganizeCardEffectCommand_AfterAttack(_card);

        DispatchCardEffect_AfterAttack_Undo();
    }

    private void OrganizeCardEffectCommand(CardDataInstance usedCard)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();
            InsertCommandToList(timing, cardStatusEffects[i]);
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();
            InsertCommandToList(timing, cardLogicSystemEffects[i]);
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();
            InsertCommandToList(timing, cardSlotSystemEffects[i]);
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();
            InsertCommandToList(timing, complexSystemEffects[i]);
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();
            InsertCommandToList(timing, selectionSystemEffects[i]);
        }
    }

    private void OrganizeCardEffectCommand_AfterAttack(CardDataInstance usedCard, Action handler = null)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            if (handler != null)
            {
                cardStatusEffects[i].EffectCanApplyEvent -= handler;
                cardStatusEffects[i].EffectCanApplyEvent += handler;
            }

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardStatusEffects[i]);
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            if (handler != null)
            {
                cardLogicSystemEffects[i].EffectCanApplyEvent -= handler;
                cardLogicSystemEffects[i].EffectCanApplyEvent += handler;
            }

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardLogicSystemEffects[i]);
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            if (handler != null)
            {
                cardSlotSystemEffects[i].EffectCanApplyEvent -= handler;
                cardSlotSystemEffects[i].EffectCanApplyEvent += handler;
            }

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardSlotSystemEffects[i]);
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            if (handler != null)
            {
                complexSystemEffects[i].EffectCanApplyEvent -= handler;
                complexSystemEffects[i].EffectCanApplyEvent += handler;
            }

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, complexSystemEffects[i]);
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            if (handler != null)
            {
                selectionSystemEffects[i].EffectCanApplyEvent -= handler;
                selectionSystemEffects[i].EffectCanApplyEvent += handler;
            }

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, selectionSystemEffects[i]);
        }
    }

    private void ClearBulletCardStatus(CardDataInstance usedCard, Action handler = null)
    {
        usedCard.effectModifiers.Clear();

        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].ResetCommandData();
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].ResetCommandData();
        }
    }

    private void CommandConditionCheck_AfterAttack(CardDataInstance usedCard)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                cardStatusEffects[i].EffectConditionCheck();
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                cardLogicSystemEffects[i].EffectConditionCheck();
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                cardSlotSystemEffects[i].EffectConditionCheck();
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                complexSystemEffects[i].EffectConditionCheck();
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();

            if (timing == GameSystemActionTimingType.AfterAttack)
                selectionSystemEffects[i].EffectConditionCheck();
        }
    }

    private void OrganizeCardEffectCommand_WithoutAfterAttack(CardDataInstance usedCard)
    {
        //OCP 위반.
        List<CardEffectCommand> cardLogicSystemEffects = usedCard.GetcardLogicSystemEffects();
        List<CardEffectCommand> cardDataControlSystemEffects = usedCard.GetcardDataControlSystemEffects();
        List<CardEffectCommand> cardStatusEffects = usedCard.GetcardStatusEffects();
        List<CardEffectCommand> cardSlotSystemEffects = usedCard.GetcardSlotSystemEffects();
        List<CardEffectCommand> complexSystemEffects = usedCard.GetcomplexSystemEffects();
        List<CardEffectCommand> selectionSystemEffects = usedCard.GetselectionSystemEffects();

        for (int i = 0; i < cardStatusEffects.Count; ++i)
        {
            cardStatusEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardStatusEffects[i].GetGameSystemActionTimingType();

            if (timing != GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardStatusEffects[i]);
        }

        for (int i = 0; i < cardLogicSystemEffects.Count; ++i)
        {
            cardLogicSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardLogicSystemEffects[i].GetGameSystemActionTimingType();

            if (timing != GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardLogicSystemEffects[i]);
        }

        for (int i = 0; i < cardSlotSystemEffects.Count; ++i)
        {
            cardSlotSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = cardSlotSystemEffects[i].GetGameSystemActionTimingType();

            if (timing != GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, cardSlotSystemEffects[i]);
        }

        for (int i = 0; i < complexSystemEffects.Count; ++i)
        {
            complexSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = complexSystemEffects[i].GetGameSystemActionTimingType();

            if (timing != GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, complexSystemEffects[i]);
        }

        for (int i = 0; i < selectionSystemEffects.Count; ++i)
        {
            selectionSystemEffects[i].InitializeCommand(usedCard);

            GameSystemActionTimingType timing = selectionSystemEffects[i].GetGameSystemActionTimingType();

            if (timing != GameSystemActionTimingType.AfterAttack)
                InsertCommandToList(timing, selectionSystemEffects[i]);
        }
    }

    private void InsertCommandToList(GameSystemActionTimingType timingType, CardEffectCommand command)
    {
        if (timingType == GameSystemActionTimingType.BeforeAttack)
        {
            cardEffect_BeforeAttack.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.AfterAttack)
        {
            cardEffect_AfterAttack.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.BeforeTurn)
        {
            cardEffect_BeforeTurn.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.BeforeCardUsingPhase)
        {
            cardEffect_BeforeCardUsingPhase.Add(command);
        }
        else if (timingType == GameSystemActionTimingType.AfterCardUsingPhase)
        {
            cardEffect_AfterCardUsingPhase.Add(command);
        }
    }

    public CardUsedResult TryCardUse(ICardDataInstanceProvider usedCard)
    {
        CardUsedResult result;

        if (usedCard is CardDataInstance card == false)
        {
            result.bVerified = false;
            result.slotIdx = -1;
            result.usedCard = null;

            return result;
        }

        if (card.cardUsingCondition != null)
        {
            CardUsingConditionCheckEvent?.Invoke(card.cardUsingCondition);

            if (card.cardUsingCondition.bResult == false)
            {
                result.bVerified = false;
                result.slotIdx = -1;
                result.usedCard = card;

                return result;
            }
        }

        ICardDataProvider usedCardData = usedCard.GetCardDataProvider();

        if (usedCardData.cardType == CardType.Bullet || usedCardData.cardType == CardType.Inherence)
        {
            UndoAfterAttackEffets();

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

            ApplyAfterAttackEffects();
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

        DispatchCardSystemActionCommand_Instant(CardLogicSystemActionType.CardsToHand, writeBuffer.Slice(0, cards.Count));

        UndoAfterAttackEffets();
        cardSlotManager.DiscardBulletCard(slotIdx);
        ApplyAfterAttackEffects();

        for (int i = 0; i < writeBuffer.Length; ++i)
        {
            ClearBulletCardStatus(writeBuffer[i]);
        }
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
                if (bulletCardSlot[i][j] == null)
                    continue;

                ClearBulletCardStatus(bulletCardSlot[i][j], EffectApplyStateChanged);

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

    //무한 루프 방어 코드 필요. - 도메인 로직이 이를 방어하지만 아키텍쳐에서 방어되지는 않음.
    public void UseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards)
    {
        if (usingCards == null || usingCards.Length == 0)
            return;

        for (int i = 0; i < usingCards.Length; ++i)
        {
            if (usingCards[i] != null)
            {
                DirectOrganizeAndDispatch_AfterAttack(usingCards[i]);
            }
        }
    }

    public void UndoUseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards)
    {
        if (usingCards == null || usingCards.Length == 0)
            return;

        for (int i = 0; i < usingCards.Length; ++i)
        {
            if (usingCards[i] != null)
            {
                DirectOrganizeAndDispatch_AfterAttack_Undo(usingCards[i]);
            }
        }
    }

    public void RequestCardLogicSystemActionCommand(CardLogicSystemActionType cardLogicSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant)
    {
        if (_type == GameSystemActionTimingType.Instant)
            DispatchCardSystemActionCommand_Instant(cardLogicSystemActionType, _cards);

        CardActionEndScopeEvent?.Invoke();
    }

    public void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant)
    {
        DispatchCardDataControlSystemActionCommand_Instant(cardDataControlSystemActionType, _cards, _cardSystemContextType);

        CardActionEndScopeEvent?.Invoke();
    }

    public void ReserveCardEffect(CardEffectCommand command)
    {
        InsertCommandToList(command.GetGameSystemActionTimingType(), command);
    }

    public int GetPrevUsedCardCnt()
    {
        return prevUsedCardCnt;
    }

    public void ApplyCardUsePhaseCntModifier(int cnt)
    {
        cardUsePhaseCnt += cnt;

        if (cardUsePhaseCnt > 2)
            cardUsePhaseCnt = 2;
    }

    public void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards)
    {
        if (cards.Length == 0)
            return;

        for (int i = 0; i < cards.Length; ++i)
        {
            var command = cards[i].GetHandPileExistEffect();

            if (command == null)
                continue;

            command.InitializeCommand(cards[i]);

            //OCP 위반.
            if (command.GetEffectApplyType() == EffectApplyType.System)
                CardLogicSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.StatusSystem)
                CardStatusCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.SlotSystem)
                CardSlotSystemCommandDispatchEvent?.Invoke(command, false);
            else if (command.GetEffectApplyType() == EffectApplyType.ComplexSystem)
                CardComplexCommandDispatchEvent?.Invoke(command, false);
            else
                CardSelectionSystemCommandDispatchEvent?.Invoke(command, false);
        }
    }

    public void CatchCardLogicSystemEvent(CardLogicSystemEventData data, ReadOnlySpan<CardDataInstance> cards = default)
    {
        cardLogicSystemCreatorMap[(int)data.eventType]?.Invoke();
    }

    public void CatchCardDataControlSystemEvent(CardDataControlSystemEventData data, ReadOnlySpan<CardDataInstance> cards = default)
    {
        if (data.contextType == GameSystemActionContextType.NoContext)
            return;

        cardDataControlSystemCreatorMap[(int)data.eventType]?.Invoke();
    }

    private void HandleSlotEffectsWhenHandChanged(ReadOnlySpan<CardDataInstance> cards = default)
    {
        if (bCardUsingFinished == true)
            return;

        UndoAfterAttackEffets();
        ApplyAfterAttackEffects();
    }

    private void HandleCardValueChanged(ReadOnlySpan<CardDataInstance> cards = default)
    {
        if (bCardUsingFinished == true)
            return;

        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                UndoCardEffect_AfterAttack(cards[i]);
                
            }
        }
    }
}
