using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class UIView_MainMenu : UIView
{
    public event Action PlayButtonClickedEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;

    [Header("Buttons")]
    [SerializeField] private MenuButton startButton;
    [SerializeField] private MenuButton creditButton;
    [SerializeField] private MenuButton optionButton;
    [SerializeField] private MenuButton exitButton;

    [Header("Other UI's")]
    [SerializeField] private WarningUI warningUI;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);

        startButton?.OnCompleteAction(OnClickStart);
        exitButton?.OnCompleteAction(OnClickExit);
        creditButton?.OnCompleteAction(OnClickCredit);
        optionButton?.OnCompleteAction(OnClickOption);
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

    public void OnClickExit()
    {
        warningUI?.Play("아직 로직이 없어요");
    }

    public void OnClickCredit()
    {
        warningUI?.Play("아직 로직이 없어요");
    }

    public void OnClickOption()
    {
        warningUI?.Play("아직 로직이 없어요");
    }
}
