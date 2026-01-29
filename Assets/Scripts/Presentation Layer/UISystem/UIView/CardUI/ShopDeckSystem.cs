using UnityEngine;
using UnityEngine.EventSystems;

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

    public override void OnPointerDown(PointerEventData _eventData)
    {
        base.OnPointerDown(_eventData);
    }

    public override void OnPointerUp(PointerEventData _eventData)
    {
        base.OnPointerUp(_eventData);
        owner?.CallPannel();
    }

    public override void OnPointerEnter(PointerEventData _eventData)
    {
        base.OnPointerEnter(_eventData);
    }

    public override void OnPointerExit(PointerEventData _eventData)
    {
        base.OnPointerExit(_eventData);
    }
}
