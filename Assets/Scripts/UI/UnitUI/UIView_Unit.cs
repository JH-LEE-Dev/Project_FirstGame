using UnityEngine;

public class UIView_Unit : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    ICharacterData characterData;

    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection()
    {
       
    }

    public void Initialize(ICharacterData _characterData)
    {
        characterData = _characterData;
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
