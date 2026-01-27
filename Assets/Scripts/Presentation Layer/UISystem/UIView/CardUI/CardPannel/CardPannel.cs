using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPannel : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private CardPannelExitButton exitButton;
    [SerializeField] private CardPannelSelectButton selectButton;
    private UIView_CardSystem cardSystem;
    private ScrollRect pannelScroll = null;

    private CurrentPannel currPannelType = CurrentPannel.NONE;

    private bool pannelSelectMode = false;
    public bool PannelSelectMode { get { return pannelSelectMode; } set { pannelSelectMode = value; } }

    private bool selectForcing = false;
    private int maxSelectCardCnt = 0;

    private List<MainCardInstance> selectCards = new List<MainCardInstance>(10);
    private List<CardDataInstance> selectDatas = new List<CardDataInstance>(10);

    public CurrentPannel CurrPannelType
    {
        get { return currPannelType; }
        set { currPannelType = value; }
    }

    public void StartSelectMode(int _selectCnt, bool _bSelectForcing)
    {
        pannelSelectMode = true;
        selectForcing = _bSelectForcing;
        maxSelectCardCnt = _selectCnt;

        if (!_bSelectForcing)
            selectButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);
    }

    public void SetupSelectMode(bool bSelectMode)
    {
        if(bSelectMode)
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

    public void ToggleSelect(MainCardInstance card)
    {
        if (CardState.Selecting == card.cardState)
        {
            selectCards.Remove(card);
            card.SetUIState(CardState.Hidden);
            card.OtherMotion.OnClick(false);

            if (maxSelectCardCnt > selectCards.Count && selectForcing)
            {
                selectButton?.SetState(ButtonInstance.VisualState.VisibleDisabled);
            }

            return;
        }

        if (maxSelectCardCnt <= selectCards.Count)
            return;

        card.SetUIState(CardState.Selecting);
        card.OtherMotion.OnClick(true);

        selectCards.Add(card);

        if (maxSelectCardCnt <= selectCards.Count && selectForcing)
        {
            selectButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);
        }
    }

    public void CompleteSelectedCards()
    {
        if (null == cardSystem)
            return;

        selectDatas.Clear();

        foreach (MainCardInstance data in selectCards)
        {
            data.OtherMotion.OnClick(false);
            selectDatas.Add(data.CardData);
        }

        cardSystem.EndCardSelectModefromPannel(selectDatas);
        selectCards.Clear();
    }

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    private void Awake()
    {
        pannelScroll = gameObject.GetComponentInChildren<ScrollRect>();

        if(null != exitButton)
            exitButton.onClickedEvent += DeActivatePannel;

        if (null != selectButton)
        {
            selectButton.onClickedEvent += DeActivatePannel;
            selectButton.onClickedEvent += CompleteSelectedCards;

            selectButton.SetState(ButtonInstance.VisualState.Hidden);
            selectButton.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (null != exitButton)
        {
            exitButton.onClickedEvent -= DeActivatePannel;
            exitButton.onClickedEvent += DeActivatePannel;
        }

        if (null != selectButton)
        {
            selectButton.onClickedEvent -= DeActivatePannel;
            selectButton.onClickedEvent -= CompleteSelectedCards;

            selectButton.onClickedEvent += DeActivatePannel;
            selectButton.onClickedEvent += CompleteSelectedCards;
        }
    }

    private void OnDisable()
    {
        if (null != exitButton)
            exitButton.onClickedEvent -= DeActivatePannel;

        if (null != selectButton)
        {
            selectButton.onClickedEvent -= DeActivatePannel;
            selectButton.onClickedEvent -= CompleteSelectedCards;
        }
    }

    private void DeActivatePannel()
    {
        gameObject.SetActive(false);
        pannelScroll.verticalNormalizedPosition = 1f;
    }
}
