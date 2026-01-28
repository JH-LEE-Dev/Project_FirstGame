using GameControlSignals;
using ShopSystemSignals;

public class ShopSystem 
{
    //외부 의존성
    private SignalHub signalHub;
    private ShopManager shopManager;

    public void Initialize(SignalHub _signalHub,ShopManager _shopManager)
    {
        signalHub = _signalHub;
        shopManager = _shopManager;

        SubscribeSignals();
        BindEvents();
    }

    private void SubscribeSignals()
    {
        signalHub.Subscribe<ShopTimeStartedSignal>(ShopOpened);
    }

    private void UnSubscribeSignals()
    {
        signalHub.UnSubscribe<ShopTimeStartedSignal>(ShopOpened);
    }

    private void BindEvents()
    {
        shopManager.ShopIsReadyEvent -= ShopIsReady;
        shopManager.ShopIsReadyEvent += ShopIsReady;
    }

    private void ReleaseEvents()
    {
        shopManager.ShopIsReadyEvent -= ShopIsReady;
    }

    public void Release()
    {
        UnSubscribeSignals();
        ReleaseEvents();
    }

    private void ShopOpened(ShopTimeStartedSignal shopOpenedSignal)
    {
        shopManager.OpenShop();
    }

    private void ShopIsReady()
    {
        signalHub.Publish(new ShopIsReadySignal());
    }
}
