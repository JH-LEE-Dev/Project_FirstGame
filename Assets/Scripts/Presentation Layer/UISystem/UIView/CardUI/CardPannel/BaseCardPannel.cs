using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCardPannel : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] protected CardPannelExitButton exitButton;
    [SerializeField] protected CardPannelSelectButton selectButton;

    protected ScrollRect pannelScroll = null;

    protected CardZone currPannelType = CardZone.NONE;

    protected bool pannelSelectMode = false;
    public bool PannelSelectMode { get { return pannelSelectMode; } set { pannelSelectMode = value; } }

    protected bool selectForcing = false;
    protected int maxSelectCardCnt = 0;

    protected List<MainCardInstance> selectCards = new List<MainCardInstance>(10);
    protected List<ICardDataInstanceProvider> selectDatas = new List<ICardDataInstanceProvider>(10);

    public CardZone CurrPannelType
    {
        get { return currPannelType; }
        set { currPannelType = value; }
    }

    public virtual void StartSelectMode(int _selectCnt, bool _bSelectForcing)
    {
        pannelSelectMode = true;
        selectForcing = _bSelectForcing;
        maxSelectCardCnt = _selectCnt;

        if (!_bSelectForcing)
            selectButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);
    }

    public virtual void SetupSelectMode(bool bSelectMode)
    {
        if (bSelectMode)
        {
            exitButton.gameObject.SetActive(false);
            exitButton?.SetState(ButtonInstance.VisualState.Hidden);

            selectButton.gameObject.SetActive(true);
            selectButton?.SetState(ButtonInstance.VisualState.VisibleDisabled);
        }
        else
        {
            exitButton.gameObject.SetActive(true);
            exitButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);

            selectButton.gameObject.SetActive(false);
            selectButton?.SetState(ButtonInstance.VisualState.Hidden);
        }
    }

    protected abstract void CompleteSelectedCards();

    protected virtual void Awake()
    {
        pannelScroll = gameObject.GetComponentInChildren<ScrollRect>();

        if (null != selectButton)
        {
            selectButton.SetState(ButtonInstance.VisualState.Hidden);
            selectButton.gameObject.SetActive(false);
        }
    }

    protected virtual void OnEnable()
    {
        if (null != exitButton)
        {
            exitButton.onClickedEvent -= DeActivatePannel;
            exitButton.onClickedEvent += DeActivatePannel;
        }

        if (null != selectButton)
        {
            selectButton.onClickedEvent -= CompleteSelectedCards;
            selectButton.onClickedEvent += CompleteSelectedCards;

            selectButton.onClickedEvent -= DeActivatePannel;
            selectButton.onClickedEvent += DeActivatePannel;
        }
    }

    protected virtual void OnDisable()
    {
        if (null != exitButton)
            exitButton.onClickedEvent -= DeActivatePannel;

        if (null != selectButton)
        {
            selectButton.onClickedEvent -= DeActivatePannel;
            selectButton.onClickedEvent -= CompleteSelectedCards;
        }
    }

    protected virtual void DeActivatePannel()
    {
        gameObject.SetActive(false);
        pannelScroll.verticalNormalizedPosition = 1f;
    }
}
