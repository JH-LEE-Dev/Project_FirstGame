using UnityEngine;

public class UIView_Unit : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
 
    protected override void Awake()
    {
        base.Awake();

    }

    public override void Update()
    {
        base.Update();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
}
