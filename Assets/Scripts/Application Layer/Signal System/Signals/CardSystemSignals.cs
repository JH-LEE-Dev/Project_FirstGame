
namespace CardSystemSignals
{ 
    public struct CardDrawFinishedSignal { }
    public struct CardUsedSignal
    {
        public bool bVerified;
        public int slotIdx;

        public CardUsedSignal(bool _bVerified, int _slotIdx)
        {
            bVerified = _bVerified; 
            slotIdx = _slotIdx;
        }
    }
    public struct CardDrawStartSignal  { }

    public struct CardPileDrawSignal  { }
    public struct CardAdditionalDrawSignal  { }
    public struct GraveToDeckSignal  { }
    public struct HandToGraveSignal  { }

    //Scope
    public struct CardActionScopeSignal  { }

    public struct TryCardUseSignal
    {
        public readonly CardDataInstance usedCard;

        public TryCardUseSignal(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
        }
    }

    public struct CardUsingFinishedSignal { }

    public struct DiscardBulletCardSignal
    {
        public int slotIdx;

        public DiscardBulletCardSignal(int _slotIdx)
        {
            slotIdx = _slotIdx;
        }
    }
}

namespace CardEffectSystemSignal
{
    public struct CardStatusEffectCommandDispatchSignal 
    {
        public ICardStatusEffectCommand command;

        public CardStatusEffectCommandDispatchSignal(ICardStatusEffectCommand _command)
        {
            command = _command;
        }
    }
}