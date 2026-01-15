
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

        public CardUsingVerificationEvent(bool boolean)
        {
            bVerified = boolean;
        }
    }
    public struct CardDrawedEvent { }
}

namespace CardEffectSystemSignal
{
    public struct CardEffectSystemCommandDispatchEvent
    {
        public CardEffectSystemCommand command;

        public CardEffectSystemCommandDispatchEvent(CardEffectSystemCommand _command)
        {
            command = _command;
        }
    }

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
    public struct CardUsedEvent
    { 
        public readonly CardDataInstance usedCard;

        public CardUsedEvent(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }

    public struct CardUsingFinishedEvent { }
}
