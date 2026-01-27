using UnityEngine;
using GameControlSignals;

public class ShopUIModuleCoordinator
{
    private ShopUICoordinator shopUICoordinator;
    private SignalHub signalHub;

    public void Initialize(SignalHub _signalHub,ShopUICoordinator _shopUICoordinator)
    {
        signalHub = _signalHub;
        shopUICoordinator = _shopUICoordinator;

        SubscribeSignals();
    }

    public void SubscribeSignals()
    {
        signalHub.Subscribe<ShopOpenedSignal>(ShopOpened);
    }

    private void UnSubscribeSignals()
    {
        signalHub.UnSubscribe<ShopOpenedSignal>(ShopOpened);
    }

    private void ShopOpened(ShopOpenedSignal shopOpenedSignal)
    {
        shopUICoordinator.ShopOpened();
    }

    public void Release()
    {
        UnSubscribeSignals();
    }
}
