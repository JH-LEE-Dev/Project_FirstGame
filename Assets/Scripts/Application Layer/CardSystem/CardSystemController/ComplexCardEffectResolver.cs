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
    private ICardSystemActionCommandHandler cardSystemActionCommandHandler;
    private ICardStatusEffectCommandHandler cardStatusEffectCommandHandler;
    private ICardSlotSystemActionCommandHandler slotSystemActionCommandHandler; 
    private ICardSystemControlActionCommandHandler cardSystemControlActionCommandHandler; 

    public void Initialize(ICardSystemActionCommandHandler _cardSystemActionCommandHandler,
        ICardStatusEffectCommandHandler _cardStatusEffectCommandHandler,
        ICardSlotSystemActionCommandHandler _cardSlotSystemActionCommandHandler,
        ICardSystemControlActionCommandHandler _cardSystemControlActionCommandHandler)
    {
        cardStatusEffectCommandHandler = _cardStatusEffectCommandHandler;
        cardSystemActionCommandHandler = _cardSystemActionCommandHandler;
        slotSystemActionCommandHandler = _cardSlotSystemActionCommandHandler;
        cardSystemControlActionCommandHandler = _cardSystemControlActionCommandHandler;
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
        return slotSystemActionCommandHandler.GetPrevUsedRotationBulletCard();
    }

    public void GraveToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards)
    {
        cardSystemActionCommandHandler.GraveToHand(graveToDeckCards);
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
}