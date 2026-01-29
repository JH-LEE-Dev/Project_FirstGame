using UnityEngine;

public class BattleCardPannel : BaseCardPannel
{
    [Header("Main Settings")]
    private UIView_CardSystem cardSystem;

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    protected override void CompleteSelectedCards()
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
