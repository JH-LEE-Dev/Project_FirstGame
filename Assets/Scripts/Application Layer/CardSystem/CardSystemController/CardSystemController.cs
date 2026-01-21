using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using UnityEngine;
using CardSystemUISignal;

public class CardSystemController : MonoBehaviour
{
    public event Action<CardDataInstance> CardUsedEvent;
    public event Action CardDrawStartEvent;
    public event Action CardDrawFinishedEvent;
    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;
    public event Action PlayerTurnFinishedEvent;
    public event Action<CardDataInstance> BulletCardAppliedEvent;
    public event Action ExtinctionToDeckEvent;

    public event Action<CardSystemCommand> SystemCommandDispatchEvent;
    public event Action<CardSystemCommand> SlotSystemCommandDispatchEvent;
    public event Action<CardSystemCommand> StatusCommandDispatchEvent;

    [SerializeField] private List<CardEffectCommand> cardStatusCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSystemCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> slotSystemCommands = new List<CardEffectCommand>();

    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_BeforeTurn = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_BeforeAttack = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_AfterAttack = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(30);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(30);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(30);

    private CardSlotManager cardSlotManager;

    public void Initialize()
    {
        cardSlotManager = new CardSlotManager();

        BindEvents();
    }

    private void BindEvents()
    {
        SlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
        SlotSystemCommandDispatchEvent += cardSlotManager.ExecuteCommand;
    }

    private void ReleaseEvents()
    {
        SlotSystemCommandDispatchEvent -= cardSlotManager.ExecuteCommand;
    }

    public void Release()
    {
        ReleaseEvents();
    }

    public void StartCardDrawTurn()
    {
        CardDrawStartEvent?.Invoke();
        DispatchCardEffect_BeforeTurn();
        DispatchCardSystemActionCommand_BeforeTurn();
        CardDrawFinishedEvent?.Invoke();
        CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardSystemActionCommand_BeforeTurn()
    {
        for (int i = 0; i < cardSystemActionCommands_BeforeTurn.Count; ++i)
        {
            SystemCommandDispatchEvent?.Invoke(cardSystemActionCommands_BeforeTurn[i]);
        }
    }

    private void DispatchCardEffect_BeforeTurn()
    {
        for (int i = 0; i < cardEffect_BeforeTurn.Count; ++i)
        {
            var command = cardEffect_BeforeTurn[i];

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else
                SlotSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeTurn.Clear();
        CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardEffect_BeforeAttack()
    {
        for(int i = 0;i< cardEffect_BeforeAttack.Count;++i)
        {
            var command = cardEffect_BeforeAttack[i];

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else
                SlotSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_BeforeAttack.Clear();
        CardActionEndScopeEvent?.Invoke();
    }

    private void DispatchCardEffect_AfterAttack()
    {
        for (int i = 0; i < cardEffect_AfterAttack.Count; ++i)
        {
            var command = cardEffect_AfterAttack[i];

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else if (command.GetCardEffectApplyType() == CardEffectApplyType.Status)
                StatusCommandDispatchEvent?.Invoke(command);
            else
                SlotSystemCommandDispatchEvent?.Invoke(command);
        }

        cardEffect_AfterAttack.Clear();
        //CardActionEndScopeEvent?.Invoke();
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        CardUsedEvent?.Invoke(usedCard);

        if (usedCard.GetCardData().cardType != CardType.Bullet)
        {
            OrginizeCardEffectCommand(usedCard);
            DispatchCardEffect_BeforeAttack();
        }
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        cardSlotManager.SortCardSlot();
        var bulletCardSlot = cardSlotManager.GetCardSlot();

        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            OrginizeCardEffectCommand(bulletCardSlot[i]);

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

    private void OrginizeCardEffectCommand(CardDataInstance usedCard)
    {
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;
        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;
        List<CardSlotSystemEffectType> cardSlotSystemEffectsTypes = usedCard.GetCardData().cardSlotSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(usedCard.nestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(usedCard.nestingCnt, usedCard.valueModifier);

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSlotSystemEffectsTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSystemCommands[(int)cardSlotSystemEffectsTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.ApplyCardState(usedCard.nestingCnt, usedCard.valueModifier);

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

        for(int i = 0;i<bulletCardSlot.Count;++i)
        {
            BulletCardAppliedEvent?.Invoke(bulletCardSlot[i]);
        }

        cardSlotManager.ClearAllBulletCard();

        CardActionEndScopeEvent?.Invoke();
    }

    public void GameStarted()
    {
        ExtinctionToDeckEvent?.Invoke();
        CardActionEndScopeEvent?.Invoke();
    }
}
