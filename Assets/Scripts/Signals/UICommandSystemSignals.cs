using System;

namespace UICommandSystemSignals
{
    public struct CardSystem_JobDispatchEvent 
    {
        public ActionDataBatch_CardSystem actionDataBatch;

        public CardSystem_JobDispatchEvent(ActionDataBatch_CardSystem _actionDataBatch)
        {
            actionDataBatch = _actionDataBatch;
        }
    }

    public struct UICommandCompleteEvent 
    {
        public int commandIdx;

        public UICommandCompleteEvent(int idx)
        {
            commandIdx = idx;
        }
    }
}

