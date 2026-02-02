using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemSignals
{
    public struct ShopIsReadySignal { }
    public struct ShopOutputSignal 
    {
        public List<ICardDataInstanceProvider> cards;
        public ShopBehaviorType behaviorType;

        public ShopOutputSignal(List<ICardDataInstanceProvider> _cards,ShopBehaviorType _behaviorType)
        {
            cards = _cards;
            behaviorType = _behaviorType;   
        }
    }
}

