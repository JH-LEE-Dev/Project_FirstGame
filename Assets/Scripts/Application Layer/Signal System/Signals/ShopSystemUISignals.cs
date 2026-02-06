using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemUISignals
{
    public struct ShopIsClosedSignal { }
    public struct CardPackRerollSignal { }
    public struct ShopOutputSignal
    {
        public List<ICardDataInstanceProvider> cards;
        public ShopBehaviorType behaviorType;

        public ShopOutputSignal(List<ICardDataInstanceProvider> _cards, ShopBehaviorType _behaviorType)
        {
            cards = _cards;
            behaviorType = _behaviorType;
        }
    }
    public struct ShopBillingSignal
    {
        public int usedMoney;
        public ShopBillingSignal(int _usedMoney)
        {
            usedMoney = _usedMoney;
        }
    }

}

