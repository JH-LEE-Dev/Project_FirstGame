using System;

namespace UICommandSystemSignals
{
    public struct CardSystem_ActionDispatchSignal 
    {
        public CardUIActionBatch actionDataBatch;

        public CardSystem_ActionDispatchSignal(CardUIActionBatch _actionDataBatch)
        {
            actionDataBatch = _actionDataBatch;
        }
    }
}

