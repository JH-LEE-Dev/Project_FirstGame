
namespace CardSystemSignals
{ 
    public struct CardDrawFinishedEvent : IPulicSignal { }
    public struct CardUsingTurnFinishedEvent : IPulicSignal { }
    public struct CardUsedEvent : IPulicSignal
    {
        public readonly CardDataInstance usedCard;

        public CardUsedEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }
    public struct CardUsingVerificationEvent : IPulicSignal
    {
        public readonly bool bVerified;

        public CardUsingVerificationEvent(bool boolean)
        {
            bVerified = boolean;
        }
    }
    public struct CardDrawStartEvent : IPulicSignal { }

    public struct CardPileDrawEvent : ICardSystemPrivateSignal { }
    public struct CardAdditionalDrawEvent : ICardSystemPrivateSignal { }
    public struct GraveToDeckEvent : ICardSystemPrivateSignal { }
    public struct HandToGraveEvent : ICardSystemPrivateSignal { }

    //Scope
    public struct CardActionScope : ICardSystemPrivateSignal { }
}

namespace CardEffectSystemSignal
{
    public struct CardEffectSystemCommandDispatchEvent : IPulicSignal
    {
        public CardEffectSystemCommand command;

        public CardEffectSystemCommandDispatchEvent(CardEffectSystemCommand _command)
        {
            command = _command;
        }
    }

    public struct CardEffectStatusCommandDispatchEvent : IPulicSignal
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
    public struct CardUsedEvent : IPulicSignal
    { 
        public readonly CardDataInstance usedCard;

        public CardUsedEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }

    public struct CardUsingFinishedEvent : IPulicSignal { }
}
