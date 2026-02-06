
namespace CardSystemSignals
{ 
    public struct CardUsePhaseStartedSignal { }
    public struct AfterCardUsePhaseStartedSignal { }
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
    //Scope
    public struct CardActionScopeSignal  { }
}

namespace EffectSystemSignal
{
    public struct CardStatusEffectCommandDispatchSignal 
    {
        public GameSystemCommand command;
        public bool bUndo;

        public CardStatusEffectCommandDispatchSignal(GameSystemCommand _command,bool _bUndo)
        {
            command = _command;
            bUndo = _bUndo;
        }
    }

    public struct ArtifactEffectCommandDispatchSignal
    {
        public GameSystemCommand command;
        public bool bUndo;
        public EffectApplyType type;

        public ArtifactEffectCommandDispatchSignal(GameSystemCommand _command, bool _bUndo,EffectApplyType _type)
        {
            command = _command;
            bUndo = _bUndo;
            type = _type;
        }
    }

}