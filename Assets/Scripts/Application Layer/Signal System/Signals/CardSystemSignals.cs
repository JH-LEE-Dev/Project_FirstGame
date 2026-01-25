
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
    public struct UsedCardToExtinctionSignal { }
    public struct UsedCardToGraveSignal { }
    public struct ExtinctionToDeckSignal { }
    public struct GraveToHandSignal { }
    public struct CardSlotCntChangedSignal 
    {
        public int cnt;

        public CardSlotCntChangedSignal(int _cnt)
        {
            cnt = _cnt;
        }
    }
    //Scope
    public struct CardActionScopeSignal  { }
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