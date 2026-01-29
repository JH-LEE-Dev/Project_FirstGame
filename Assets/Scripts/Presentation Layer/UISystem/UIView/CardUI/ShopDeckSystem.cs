using UnityEngine;

public class ShopDeckSystem : BaseDeckSystem
{
    [Header("MainSettings")]
    private UIView_Shop owner;

    public void Init(UIView_Shop _owner) => owner = _owner;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
