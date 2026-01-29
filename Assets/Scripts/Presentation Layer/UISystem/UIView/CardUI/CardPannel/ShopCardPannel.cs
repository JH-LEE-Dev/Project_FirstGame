using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopCardPannel : BaseCardPannel
{
    [Header("Main Settings")]
    private UIView_Shop owner;
    private List<ShopCardInstance> rentCards = new(50);

    public List<ShopCardInstance> RentCards { get { return rentCards; } set { rentCards = value; } }

    public CardPannelSelectButton SelectBtn { get { return selectButton; }  set { selectButton = value; } }

    public void Init(UIView_Shop _owner) => owner = _owner;

    protected override void CompleteSelectedCards()
    {
        if (null == owner)
            return;

        owner.SelectComplete();
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
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void DeActivatePannel()
    {
        base.DeActivatePannel();
        
        foreach (ShopCardInstance data in RentCards)
        {
            owner?.ReturnCard(data);
        }
    }
}
