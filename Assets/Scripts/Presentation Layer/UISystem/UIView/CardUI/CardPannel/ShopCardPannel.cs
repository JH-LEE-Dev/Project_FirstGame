using UnityEngine;

public class ShopCardPannel : BaseCardPannel
{
    [Header("Main Settings")]
    private UIView_Shop owner;

    public void Init(UIView_Shop _owner) => owner = _owner;

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

        //owner.EndCardSelectModefromPannel(selectDatas);
        selectCards.Clear();
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
}
