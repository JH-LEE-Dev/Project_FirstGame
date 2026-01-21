
namespace CardSystemUISignal
{
    public struct DiscardBulletCardSignal
    {
        public int slotIdx;

        public DiscardBulletCardSignal(int _slotIdx)
        {
            slotIdx = _slotIdx;
        }
    }
    public struct CardUsingFinishedSignal { }
    public struct TryCardUseSignal
    {
        public readonly CardDataInstance usedCard;

        public TryCardUseSignal(CardDataInstance _usedCard)
        {
            usedCard = _usedCard;
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

