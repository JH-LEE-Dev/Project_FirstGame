using System;
using System.Collections.Generic;
using UnityEngine;

//이 Mediator의 존재 이유는, CardEffect를 실행하기 위해서 두 모듈이 모두 필요한 경우
//완벽한 모듈화를 위해서는 CardManager가 조건 체크 명령을 수행하고 이 결과를 뱉어내면 이걸 다시
//CardSystemController가 받아서 Status Effect를 Dispatch해야 하는 복잡도가 생긴다. 또한 CardManager가 뱉어낸
//조건이 어떤 조건인지도 CardSystemController에서 해석하여 그에 맞는 Status Effect를 Dispatch해야 함. 이러면
//아키텍쳐가 너무 복잡해지기 때문에 Mediator에 의존성을 추가해서 실행한다.
//사실, 이렇게 여러 모듈이 모두 필요한 카드 효과는 CardEffectCommand를 생성,관리하고 이 게임의 카드 로직을
//관리하는 CardSystemController의 고유 기능이라고 볼 수 있다. 즉, CardSystemController 모듈에 이 Mediator들이
//속해있다고 보면 됨. 

public class ComplexCardEffectResolver : IComplexSystemActionCommandHandler
{
    private ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler;
    private ICardStatusEffectCommandHandler cardStatusEffectCommandHandler;
    private ICardSlotSystemActionCommandHandler slotSystemActionCommandHandler;
    private ICardSystemControlActionCommandHandler cardSystemControlActionCommandHandler;
    private ICardSelectionSystemActionCommandHandler cardSelectionSystemActionCommandHandler;
    private ICardDataControlSystemActionCommandHandler cardDataControlActionCommandHandler;

    public void Initialize(ICardLogicSystemActionCommandHandler _cardSystemActionCommandHandler,
        ICardStatusEffectCommandHandler _cardStatusEffectCommandHandler,
        ICardSlotSystemActionCommandHandler _cardSlotSystemActionCommandHandler,
        ICardSystemControlActionCommandHandler _cardSystemControlActionCommandHandler,
        ICardSelectionSystemActionCommandHandler _cardSelectionSystemActionCommandHandler,
        ICardDataControlSystemActionCommandHandler _cardDataControlActionCommandHandler)
    {
        cardStatusEffectCommandHandler = _cardStatusEffectCommandHandler;
        cardSystemActionCommandHandler = _cardSystemActionCommandHandler;
        slotSystemActionCommandHandler = _cardSlotSystemActionCommandHandler;
        cardSystemControlActionCommandHandler = _cardSystemControlActionCommandHandler;
        cardSelectionSystemActionCommandHandler = _cardSelectionSystemActionCommandHandler;
        cardDataControlActionCommandHandler = _cardDataControlActionCommandHandler;
    }

    public void ExecuteCommand(CardSystemCommand cardSystemCommand)
    {
        cardSystemCommand.Execute(this);
    }

    public void ApplyAttackCntModifier(int attckCnt)
    {
        cardStatusEffectCommandHandler.ApplyAttackCntModifier(attckCnt);
    }

    public bool DeckConditionCheck(int cardId)
    {
        return cardSystemActionCommandHandler.DeckConditionCheck(cardId);
    }

    public IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCards()
    {
        return slotSystemActionCommandHandler.GetPrevUsedBulletCard();
    }

    public void GraveCardsToHand(ReadOnlySpan<CardDataInstance> cards)
    {
        cardSystemActionCommandHandler.GraveCardsToHand(cards);
    }

    public IReadOnlyList<CardDataInstance> GetHandPile()
    {
        return cardSystemActionCommandHandler.GetHandPile();
    }

    public void CardPileUse(ReadOnlySpan<CardDataInstance> cardPile)
    {
        cardSystemControlActionCommandHandler.UseCardsAndExtinguishAll(cardPile);
    }

    public void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile)
    {
        cardSystemActionCommandHandler.CardsToExtinction(cardPile);
    }

    public void ApplyAttackModifier(int attack)
    {
        cardStatusEffectCommandHandler.ApplyAttackModifier(attack);
    }

    public int GetPrevUsedBulletCardCnt()
    {
        return slotSystemActionCommandHandler.GetPrevUsedBulletCardCnt();
    }

    public void AdditionalDraw(int amount)
    {
        cardSystemActionCommandHandler.DrawAgain(amount);
    }

    public int GetPrevUsedCardCnt()
    {
        return cardSystemControlActionCommandHandler.GetPrevUsedCardCnt();
    }

    public void StartCardSelectionMode(SelectCardPileType selectCardPileType,CardSelectionMode cardSelectionMode, int amount)
    {
        cardSelectionSystemActionCommandHandler.StartCardSelectionMode(selectCardPileType,cardSelectionMode, amount);
    }

    public IReadOnlyList<CardDataInstance> GetDeckPile()
    {
        return cardSystemActionCommandHandler.GetDeckPile();
    }

    public IReadOnlyList<CardDataInstance> GetGravePile()
    {
        return cardSystemActionCommandHandler.GetGravePile();
    }

    public IReadOnlyList<CardDataInstance> GetExtinctionPile()
    {
        return cardSystemActionCommandHandler.GetExtinctionPile();
    }

    public void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards)
    {
        cardSystemActionCommandHandler.GraveCardsToDeck(cards);
    }

    public void RequestCardSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType)
    {
        cardSystemControlActionCommandHandler.RequestCardLogicSystemActionCommand(cardSystemActionType, _cards, _cardSystemContextType);
    }

    public void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType)
    {
        cardSystemControlActionCommandHandler.RequestCardDataControlSystemActionCommand(cardDataControlSystemActionType, _cards, _cardSystemContextType);
    }

    public void UpgradeCards(ReadOnlySpan<CardDataInstance> cards)
    {
        cardDataControlActionCommandHandler.UpgradeCards(cards);
    }

    public IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards()
    {
        return slotSystemActionCommandHandler.GetCurrentBulletCards();
    }

    public void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier)
    {
        cardDataControlActionCommandHandler.ApplyValueModifier(cards, valueModifier);
    }
}