using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopCardPannel : BaseCardPannel
{
    [Header("Main Settings")]
    private UIView_Shop owner;
    private ShopBehaviorType behaviorType;

    private List<ShopCardInstance> rentCards = new(50);
    public List<ShopCardInstance> RentCards { get { return rentCards; } set { rentCards = value; } }

    public void Init(UIView_Shop _owner) => owner = _owner;

    public void StartSelectMode(ShopBehaviorType _type, int _selectCnt, bool _bSelectForcing)
    {
        base.StartSelectMode(_selectCnt, _bSelectForcing);
        behaviorType = _type;
    }

    protected override void CompleteSelectedCards()
    {
        if (null == owner)
            return;

        selectDatas.Clear();

        foreach (MainCardInstance data in selectCards)
        {
            data.OtherMotion.OnClick(false);
            selectDatas.Add(data.CardData);
        }

        owner.OutputSelectedCards(selectDatas, behaviorType);
        selectCards.Clear();
    }

    public void SetupSelectMode(bool bSelectMode, bool bSelectBtnHidden = false)
    {
        if (bSelectBtnHidden)
        {
            selectButton.gameObject.SetActive(false);
            selectButton?.SetState(ButtonInstance.VisualState.Hidden);
        }
        else
        {
            selectButton.gameObject.SetActive(true);
            selectButton?.SetState(ButtonInstance.VisualState.VisibleDisabled);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (null != exitButton)
        {
            exitButton.onClickedEvent -= ResetValues;
            exitButton.onClickedEvent += ResetValues;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (null != exitButton)
        {
            exitButton.onClickedEvent -= ResetValues;
        }
    }

    protected override void DeActivatePannel()
    {
        base.DeActivatePannel();
        
        foreach (ShopCardInstance data in RentCards)
        {
            owner?.ReturnCard(data);
        }
    }

    private void ResetValues()
    {

    }
}
