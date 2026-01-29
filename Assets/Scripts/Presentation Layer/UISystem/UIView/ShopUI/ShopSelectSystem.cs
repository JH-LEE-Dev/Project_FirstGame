using System.Collections.Generic;
using UnityEngine;

public class ShopSelectSystem : MonoBehaviour
{
    private UIView_Shop uIView_Shop;
    private ShopBehaviorType type;

    private int selectCount = 0;
    private bool selectforce = false;

    private readonly List<ShopCardInstance> selected = new();

    private ButtonInstance confirmButton;

    public void Init(UIView_Shop owner)
    {
        uIView_Shop = owner;
        selected.Clear();
        RefreshConfirmButton();
    }
    public void SetSelectMode(ShopBehaviorType _type, int _selectCount, bool _selectforce, ButtonInstance _buttonInstance = null)
    {
        type = _type;
        selectCount = _selectCount;
        selectforce = _selectforce;
        confirmButton = _buttonInstance;

        ClearSelection();
        RefreshConfirmButton();
    }

    public void ToggleSelect(ShopCardInstance card)
    {
        if (!card) return;

        if (selectCount <= 0) return;

        bool isSelected = selected.Contains(card);

        // 선택 시도
        if (!isSelected)
        {
            if (selected.Count >= selectCount)
                return;

            selected.Add(card);
            card.SetCardState(ShopCardState.Select);
        }
        // 해제 시도
        else
        {
            selected.Remove(card);
            card.SetCardState(ShopCardState.Idle);
        }

        RefreshConfirmButton();
    }

    public bool SelectComplete()
    {
        if (!CanConfirm())
        {
            Debug.Log("더 선택하십쇼");
            return false;
        }

        var cardDatas = new List<CardDataInstance>(selected.Count);
        foreach (var c in selected)
        {
            if (c == null) continue;
            cardDatas.Add(c.CardData);
        }

        uIView_Shop?.OutputSelectedCards(cardDatas, type);

        ClearSelection();
        return true;
    }

    public void ClearSelection()
    {
        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i] == null) continue;
            selected[i].SetCardState(ShopCardState.Idle);
        }
        selected.Clear();

        RefreshConfirmButton();
    }




    /////////////
    private bool CanConfirm()
    {
        if (selectCount <= 0) return false;

        if (selectforce)
            return selected.Count == selectCount;

        return selected.Count > 0;
    }

    private void RefreshConfirmButton()
    {
        if (!confirmButton) return;

        // 선택 모드가 아니면 버튼 숨김(원하면 Disabled로 바꿔도 됨)
        if (selectCount <= 0)
        {
            confirmButton.SetState(ButtonInstance.VisualState.Hidden);
            return;
        }

        confirmButton.SetActiveVisible(true);
        confirmButton.SetCanClick(CanConfirm());
    }
}
