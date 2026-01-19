
namespace CardSystemSignals
{ 
    public struct CardDrawFinishedEvent { }
    public struct CardUsingTurnFinishedEvent { }
    public struct CardUsedEvent
    {
        public readonly CardDataInstance usedCard;

        public CardUsedEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }
    public struct CardUsingVerificationEvent 
    {
        public readonly bool bVerified;
        //public readonly int slotIdx;

        public CardUsingVerificationEvent(bool boolean)
        {
            bVerified = boolean;
            //slotIdx = _slotIdx;
        }
    }
    public struct CardDrawStartEvent  { }

    public struct CardPileDrawEvent  { }
    public struct CardAdditionalDrawEvent  { }
    public struct GraveToDeckEvent  { }
    public struct HandToGraveEvent  { }

    //Scope
    public struct CardActionScope  { }
}

namespace CardEffectSystemSignal
{
    public struct CardEffectStatusCommandDispatchEvent 
    {
        public CardEffectStatusCommand command;

        public CardEffectStatusCommandDispatchEvent(CardEffectStatusCommand _command)
        {
            command = _command;
        }
    }
}

namespace CardUISystemSignals
{
    public struct TryCardUseEvent 
    { 
        public readonly CardDataInstance usedCard;

        public TryCardUseEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }

    public struct CardUsingFinishedEvent  { }
}
