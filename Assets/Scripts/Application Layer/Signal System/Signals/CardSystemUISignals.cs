using System.Collections.Generic;

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
        public readonly ICardDataInstanceProvider usedCard;

        public TryCardUseSignal(ICardDataInstanceProvider _usedCard)
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
    public struct UICardSelectionEndSignal
    {
        public CardSelectionModeData data;
        public List<ICardDataInstanceProvider> cards;

        public UICardSelectionEndSignal(CardSelectionModeData _data,List<ICardDataInstanceProvider> _cards)
        {
            data = _data;
            cards = _cards;
        }
    }

}

