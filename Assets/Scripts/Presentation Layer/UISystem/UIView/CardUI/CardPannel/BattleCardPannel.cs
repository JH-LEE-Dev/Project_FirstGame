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
