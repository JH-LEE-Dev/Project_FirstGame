using CardSystemSignals;
using System;
using System.Collections.Generic;
using UnitLogicSystemSignals;
using Unity.VisualScripting;
using UnityEngine;

public class CardSystemController : MonoBehaviour
{
    public event Action CardDrawStartEvent;
    public event Action CardDrawFinishedEvent;
    //public event Action CardActionBeginScopeEvent;
    public event Action CardActionEndScopeEvent;
    public event Action PlayerTurnFinishedEvent;

    public event Action<CardSystemCommand> SystemCommandDispatchEvent;
    public event Action<CardSystemCommand> StatusCommandDispatchEvent;

    [SerializeField] private List<CardEffectCommand> cardStatusCommands = new List<CardEffectCommand>();
    [SerializeField] private List<CardEffectCommand> cardSystemCommands = new List<CardEffectCommand>();

    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_BeforeTurn = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_BeforeAttack = new List<CardSystemActionCommand>();
    [SerializeField] private List<CardSystemActionCommand> cardSystemActionCommands_AfterAttack = new List<CardSystemActionCommand>();

    private List<CardEffectCommand> cardEffect_BeforeTurn = new List<CardEffectCommand>(30);
    private List<CardEffectCommand> cardEffect_BeforeAttack = new List<CardEffectCommand>(30);
    private List<CardEffectCommand> cardEffect_AfterAttack = new List<CardEffectCommand>(30);

    private List<CardDataInstance> bulletCardSlot = new List<CardDataInstance>(30);
    private int bulletCardSlotCnt = 2;

    public void Initialize()
    {

    }

    public void Release()
    {

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
        while (true)
        {
            if (cardEffect_BeforeTurn.Count == 0)
                return;

            var command = cardEffect_BeforeTurn[cardEffect_BeforeTurn.Count - 1];
            cardEffect_BeforeTurn.RemoveAt(cardEffect_BeforeTurn.Count - 1);

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else
                StatusCommandDispatchEvent?.Invoke(command);
        }
    }

    private void DispatchCardEffect_BeforeAttack()
    {
        while (true)
        {
            if (cardEffect_BeforeAttack.Count == 0)
            {
                CardActionEndScopeEvent?.Invoke();
                return;
            }

            var command = cardEffect_BeforeAttack[cardEffect_BeforeAttack.Count - 1];
            cardEffect_BeforeAttack.RemoveAt(cardEffect_BeforeAttack.Count - 1);

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else
                StatusCommandDispatchEvent?.Invoke(command);
        }
    }

    private void DispatchCardEffect_AfterAttack()
    {
        while (true)
        {
            if (cardEffect_AfterAttack.Count == 0)
                return;

            var command = cardEffect_AfterAttack[cardEffect_AfterAttack.Count - 1];
            cardEffect_AfterAttack.RemoveAt(cardEffect_AfterAttack.Count - 1);

            if (command.GetCardEffectApplyType() == CardEffectApplyType.System)
                SystemCommandDispatchEvent?.Invoke(command);
            else
                StatusCommandDispatchEvent?.Invoke(command);
        }
    }

    private void CardUsed(CardDataInstance usedCard)
    {
        if (usedCard.GetCardData().cardType != CardType.Bullet)
        {
            OrginizeCardEffectCommand(usedCard);
            DispatchCardEffect_BeforeAttack();
        }
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        for (int i = 0; i < bulletCardSlot.Count; ++i)
        {
            OrginizeCardEffectCommand(bulletCardSlot[i]);
        }

        DispatchCardEffect_AfterAttack();
    }

    public void PlayerAttackFinished(PlayerAttackFinishedSignal playerAttackFinishedSignal)
    {
        for(int i = 0;i<bulletCardSlot.Count; ++i)
        {
            bulletCardSlot[i].ResetCardData();
        }

        bulletCardSlot.Clear();

        PlayerTurnFinishedEvent?.Invoke();
    }

    private void OrginizeCardEffectCommand(CardDataInstance usedCard)
    {
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;
        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.nestingCnt = usedCard.nestingCnt;

            CardSystemActionTimingType timing = effectCommand.GetCardActionTimingType();
            InsertCommandToList(timing, effectCommand);
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            if (usedCard.GetCardData().cardType == CardType.Bullet)
                effectCommand.nestingCnt = usedCard.nestingCnt;

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

        for (int i = 0; i < bulletCardSlotCnt; ++i)
        {
            if (i >= bulletCardSlot.Count)
            {
                CardUsed(usedCard);
                bulletCardSlot.Add(usedCard);
                result.bVerified = true;
                result.slotIdx = i;
                result.usedCard = usedCard;

                return result;
            }

            CardData currentCardData = bulletCardSlot[i].GetCardData();

            if (currentCardData.id == usedCardData.id)
            {
                ++bulletCardSlot[i].nestingCnt;
                result.bVerified = true;
                result.slotIdx = i;
                result.usedCard = usedCard;

                return result;
            }
        }

        result.bVerified = false;
        result.slotIdx = -1;
        result.usedCard = null;

        return result;
    }

    public void DiscardBulletCard(int slotIdx)
    {
        var slotCard = bulletCardSlot[slotIdx];
        slotCard.ResetCardData();
        bulletCardSlot.RemoveAt(slotIdx);
    }
}
