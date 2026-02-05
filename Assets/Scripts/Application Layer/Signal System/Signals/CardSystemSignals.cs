
namespace CardSystemSignals
{ 
    public struct CardUsePhaseStarted { }
    public struct PlayerTurnFinishedSignal { }
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

    public struct CardLogicSystemEventSignal 
    {
        public CardLogicSystemEventData data;

        public CardLogicSystemEventSignal(CardLogicSystemEventData data)
        {
            this.data = data;
        }
    }
    public struct CardDataControlSystemEventSignal
    {
        public CardDataControlSystemEventData data;

        public CardDataControlSystemEventSignal(CardDataControlSystemEventData data)
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
    public struct CardSelectionModeStartSignal
    {
        public CardSelectionModeData data;
        public CardSelectionModeStartSignal(CardSelectionModeData _data)
        {
            data = _data;
        }
    }
    public struct  IsInherenceCardEquippedSignal
    {
        public bool bEquipped;
        public IsInherenceCardEquippedSignal(bool _bEquipped)
        {
            bEquipped = _bEquipped;
        }
    }
    //Scope
    public struct CardActionScopeSignal  { }
}

namespace CardEffectSystemSignal
{
    public struct CardStatusEffectCommandDispatchSignal 
    {
        public CardSystemCommand command;
        public bool bUndo;

        public CardStatusEffectCommandDispatchSignal(CardSystemCommand _command,bool _bUndo)
        {
            command = _command;
            bUndo = _bUndo;
        }
    }
}