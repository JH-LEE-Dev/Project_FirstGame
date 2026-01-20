using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_Gameplay : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject crosshairUIPrefab;

    private GameObject crosshairUI;

    protected override void Awake()
    {
        base.Awake();

        if (crosshairUIPrefab != null)
            crosshairUI = Instantiate(crosshairUIPrefab, this.transform);

        crosshairUI.SetActive(false);
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

    private void OnClickClose()
    {
    }

    public void RenderUI()
    {

    }

    public void CardUsingFinished()
    {
        crosshairUI.SetActive(true);
    }

    public void PointerMoved(Vector2 move)
    {
        RectTransform uiRect = crosshairUI.GetComponent<RectTransform>();
        uiRect.anchoredPosition = move;
    }

    public override void SetupUI()
    {
        base.SetupUI();

        viewCtx.inputManager.inputReader.PointerPositionEvent -= PointerMoved;
        viewCtx.inputManager.inputReader.PointerPositionEvent += PointerMoved;
    }

    public void EnemyTurnStarted()
    {
        crosshairUI.SetActive(false);
    }

    public override void OnDestroy()
    {
        viewCtx.inputManager.inputReader.PointerPositionEvent -= PointerMoved;
    }
}
