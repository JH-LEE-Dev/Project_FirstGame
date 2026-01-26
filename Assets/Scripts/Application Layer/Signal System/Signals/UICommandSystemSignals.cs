using System;

namespace UICommandSystemSignals
{
    public struct CardSystem_JobDispatchSignal 
    {
        public CardUIActionBatch actionDataBatch;

        public CardSystem_JobDispatchSignal(CardUIActionBatch _actionDataBatch)
        {
            actionDataBatch = _actionDataBatch;
        }
    }
}

