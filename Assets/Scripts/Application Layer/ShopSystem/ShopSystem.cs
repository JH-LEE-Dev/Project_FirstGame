using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    //외부 의존성
    private SignalHub signalHub;
    private ShopManager shopManager;

    public void Initialize(SignalHub _signalHub,ShopManager _shopManager)
    {
        signalHub = _signalHub;
        shopManager = _shopManager;
    }
}
