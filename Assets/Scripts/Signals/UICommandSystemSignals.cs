
namespace UICommandSystemSignals
{
    public struct CardSystem_JobDispatchEvent
    {
        public UIJobBatch_CardSystem jobBatch;

        public CardSystem_JobDispatchEvent(UIJobBatch_CardSystem _jobBatch)
        {
            jobBatch = _jobBatch;
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

