
namespace CardSystemSignals
{ 
    public struct CardDrawFinishedEvent { }
    public struct CardUsedEvent
    {
        public bool bVerified;
        public int slotIdx;

        public CardUsedEvent(bool _bVerified, int _slotIdx)
        {
            bVerified = _bVerified; 
            slotIdx = _slotIdx;
        }
    }
    public struct CardDrawStartEvent  { }

    public struct CardPileDrawEvent  { }
    public struct CardAdditionalDrawEvent  { }
    public struct GraveToDeckEvent  { }
    public struct HandToGraveEvent  { }

    //Scope
    public struct CardActionScope  { }

    public struct TryCardUseEvent
    {
        public readonly CardDataInstance usedCard;

        public TryCardUseEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }

    public struct CardUsingFinishedEvent { }
}

namespace CardEffectSystemSignal
{
    public struct CardStatusEffectCommandDispatchEvent 
    {
        public ICardStatusEffectCommand command;

        public CardStatusEffectCommandDispatchEvent(ICardStatusEffectCommand _command)
        {
            command = _command;
        }
    }
}