using System;

namespace UICommandSystemSignals
{
    public struct CardSystem_JobDispatchSignal 
    {
        public ActionDataBatch_CardSystem actionDataBatch;

        public CardSystem_JobDispatchSignal(ActionDataBatch_CardSystem _actionDataBatch)
        {
            actionDataBatch = _actionDataBatch;
        }
    }

    public struct UICommandCompleteSignal 
    {
        public int commandIdx;

        public UICommandCompleteSignal(int idx)
        {
            commandIdx = idx;
        }
    }
}

