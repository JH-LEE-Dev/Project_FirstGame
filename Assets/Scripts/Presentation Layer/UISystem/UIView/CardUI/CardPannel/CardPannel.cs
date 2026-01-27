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

    private List<CardDataInstance> selectCards = new List<CardDataInstance>(10);
    public List<CardDataInstance> SelectCards
    {
        get { return selectCards; }
        set { selectCards = value; }
    }

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
            exitButton?.SetState(ButtonInstance.VisualState.Hidden);
            selectButton?.SetState(ButtonInstance.VisualState.VisibleDisabled);
        }
        else
        {
            exitButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);
            selectButton?.SetState(ButtonInstance.VisualState.Hidden);
        }
    }

    public void ToggleSelect(MainCardInstance card)
    {
        if (maxSelectCardCnt <= selectCards.Count)
            return;

        if (CardState.Selecting == card.cardState)
        {
            selectCards.Remove(card.CardData);
            card.SetUIState(CardState.Hidden);
            // card 한테 원래대로 돌아가라고 명령
            return;
        }

        card.SetUIState(CardState.Selecting);
        // card 한테 모션 재생하라고 명령
        selectCards.Add(card.CardData);

        if (maxSelectCardCnt <= selectCards.Count && selectForcing)
            selectButton?.SetState(ButtonInstance.VisualState.VisibleEnabled);
    }

    public void CompleteSelectedCards()
    {
        if (null == cardSystem)
            return;

        cardSystem.EndCardSelectModefromPannel(selectCards);

        // 여기서 원래대로 모양 돌려 놓는 거 진행, 상태까지 다

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
