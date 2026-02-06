using ShopSystemSignals;
using ShopSystemUISignals;
using System.Collections.Generic;

public class ShopUIModuleCoordinator
{
    private ShopUICoordinator shopUICoordinator;
    private SignalHub signalHub;

    public void Initialize(SignalHub _signalHub,ShopUICoordinator _shopUICoordinator)
    {
        signalHub = _signalHub;
        shopUICoordinator = _shopUICoordinator;

        SubscribeSignals();
        BindEvents();
    }

    public void SubscribeSignals()
    {
        signalHub.Subscribe<ShopIsReadySignal>(ShopOpened);
    }

    private void UnSubscribeSignals()
    {
        signalHub.UnSubscribe<ShopIsReadySignal>(ShopOpened);
    }

    public void Release()
    {
        UnSubscribeSignals();
        ReleaseEvents();
    }

    private void BindEvents()
    {
        shopUICoordinator.ShopIsClosedEvent -= ShopIsClosed;
        shopUICoordinator.ShopIsClosedEvent += ShopIsClosed;

        shopUICoordinator.CardPackRerollEvent -= CardPackReroll;
        shopUICoordinator.CardPackRerollEvent += CardPackReroll;

        shopUICoordinator.ShopOutputEvent -= ShopOutput;
        shopUICoordinator.ShopOutputEvent += ShopOutput;

        shopUICoordinator.ShopBillingEvent -= ShopBilling;
        shopUICoordinator.ShopBillingEvent += ShopBilling;
    }

    private void ReleaseEvents()
    {
        shopUICoordinator.ShopIsClosedEvent -= ShopIsClosed;

        shopUICoordinator.CardPackRerollEvent -= CardPackReroll;

        shopUICoordinator.ShopBillingEvent -= ShopBilling;
    }

    private void ShopOpened(ShopIsReadySignal shopOpenedSignal)
    {
        shopUICoordinator.ShopOpened();
    }

    private void ShopIsClosed()
    {
        signalHub.Publish(new ShopIsClosedSignal());
    }

    private void CardPackReroll()
    {
        signalHub.Publish(new CardPackRerollSignal());
    }

    private void ShopOutput(List<ICardDataInstanceProvider> _cards,ShopBehaviorType _shopBehaviorType)
    {
        signalHub.Publish(new ShopOutputSignal(_cards,_shopBehaviorType));
    }

    private void ShopBilling(int usedMoney)
    {
        signalHub.Publish(new ShopBillingSignal(usedMoney));
    }
}
