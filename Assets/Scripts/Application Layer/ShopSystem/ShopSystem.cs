using GameControlSignals;
using ShopSystemSignals;
using ShopSystemUISignals;

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
        signalHub.Subscribe<CardPackRerollSignal>(CardPackReroll);
        signalHub.Subscribe<ShopIsClosedSignal>(ShopIsClosed);
        signalHub.Subscribe<ShopOutputSignal>(ShopOutput);
    }

    private void UnSubscribeSignals()
    {
        signalHub.UnSubscribe<ShopTimeStartedSignal>(ShopOpened);
        signalHub.UnSubscribe<CardPackRerollSignal>(CardPackReroll);
        signalHub.UnSubscribe<ShopIsClosedSignal>(ShopIsClosed);
        signalHub.UnSubscribe<ShopOutputSignal>(ShopOutput);
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

    private void ShopIsReady()
    {
        signalHub.Publish(new ShopIsReadySignal());
    }

    private void CardPackReroll(CardPackRerollSignal cardPackRerollSignal)
    {
        shopManager.RerollMerchandise();
    }

    private void ShopIsClosed(ShopIsClosedSignal shopIsClosedSignal)
    {
        shopManager.CloseShop();
    }

    private void ShopOpened(ShopTimeStartedSignal shopOpenedSignal)
    {
        shopManager.OpenShop();
    }

    private void ShopOutput(ShopOutputSignal shopOutputSignal)
    {
        shopManager.AnalysisShopBehavior(shopOutputSignal.cards, shopOutputSignal.behaviorType);
    }
}
