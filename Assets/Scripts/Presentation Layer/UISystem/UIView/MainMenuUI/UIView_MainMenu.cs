using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIView_MainMenu : UIView
{
    public event Action PlayButtonClickedEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;

    [Header("Buttons")]
    [SerializeField] private MenuButton startButton;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);

        startButton.OnCompleteAction(OnClickStart);
    }

    public override void OnDestroy()
    {
        PlayButtonClickedEvent = null;
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

    public void OnClickStart()
    {
        PlayButtonClickedEvent?.Invoke();
    }
}
