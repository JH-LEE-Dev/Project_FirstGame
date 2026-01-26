
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

    public struct CardSystemEventSignal 
    {
        public CardSystemEventData data;

        public CardSystemEventSignal(CardSystemEventData data)
        {
            this.data = data;
        }
    }
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
    public struct CardSelectionModeStartSignal
    {
        public CardSelectionModeData data;
        public CardSelectionModeStartSignal(CardSelectionModeData _data)
        {
            data = _data;
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