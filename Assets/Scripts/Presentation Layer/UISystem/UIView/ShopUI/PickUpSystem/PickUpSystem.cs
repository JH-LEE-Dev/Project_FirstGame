using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PickUpSystem : MonoBehaviour
{
    private enum State { None, WaitingOpen, Opened }
    private State state = State.None;

    private UIView_Shop uIView_Shop;

    [SerializeField] private PickUpCancelPannal cancelPannel;
    [SerializeField] private CardPack pack;
    [SerializeField] private CardPannelSelectButton pickUpButton;
    [SerializeField] private RectTransform pickUpCardPivot;

    public CardPannelSelectButton GetPickUpButton() { return pickUpButton; }

    [Header("Layout")]
    private float cardGap = 320f;
    private float revealDur = 0.3f;

    private readonly List<ShopCardInstance> newCards = new();

    public void Init(UIView_Shop shop)
    {
        gameObject.SetActive(false);

        uIView_Shop = shop;
        pickUpButton.Init();
        pack.Init();
        cancelPannel.Init();


        // 취소 버튼
        cancelPannel.Bind(OnCancelClicked);

        // 팩 버튼
        pack.Bind(OnPackClicked);
    }

    private void OnEnable()
    {
        pickUpButton.onClickedEvent += OnConfirmClicked;
    }

    private void OnDisable()
    {
        pickUpButton.onClickedEvent -= OnConfirmClicked;
    }


    public void PickUpCardMode(IReadOnlyList<CardDataInstance> datas)
    {
        gameObject.SetActive(true);
        state = State.WaitingOpen;

        cancelPannel.Show(true);
        pack.Show(true);

        // 오픈 전에는 비활성
        pickUpButton.SetState(ButtonInstance.VisualState.Hidden);


        newCards.Clear();
        foreach (var data in datas)
        {
            var card = uIView_Shop.RentCard(data);
            newCards.Add(card);

            card.SetVisible(false);
            card.SetCardState(ShopCardState.Idle);

            card.GetComponent<RectTransform>().anchoredPosition = 
            pack.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    private void OnPackClicked()
    {
        if (state != State.WaitingOpen) return;

        state = State.Opened;

        cancelPannel.SetCanCancel(false);

        pack.gameObject.SetActive(false);
        pack.SetCanClick(false);
        pack.PlayOpenAnim(); // 러프 애니메이션

        RevealAndLayoutCards();

        pickUpButton.SetState(ButtonInstance.VisualState.VisibleEnabled);
    }

    private void RevealAndLayoutCards()
    {
        // 가운데 정렬: [-2,-1,0,1,2] * gap
        int n = newCards.Count;
        float mid = (n - 1) * 0.5f;

        for (int i = 0; i < n; i++)
        {
            var card = newCards[i];
            if (!card) continue;

            card.SetVisible(true);

            float x = (i - mid) * cardGap;
            Vector2 target = new Vector2(x, pickUpCardPivot.anchoredPosition.y);

            card.Motion?.PickUpMoveTo(target, revealDur);
        }
    }

    private void OnCancelClicked()
    {
        if (state == State.WaitingOpen)
        {
            ExitMode();
        }
    }

    private void OnConfirmClicked()
    {
        if (state != State.Opened) return;

        if (uIView_Shop.SelectComplete())
            ExitMode();
    }

    private void ExitMode()
    {
        for (int i = 0; i < newCards.Count; i++)
        {
            if (!newCards[i]) continue;
            uIView_Shop.ReturnCard(newCards[i]);
        }
        newCards.Clear();

        // UI 끄기
        cancelPannel.Hide();
        pack.Hide();
        pickUpButton.SetState(ButtonInstance.VisualState.Hidden);

        state = State.None;
        gameObject.SetActive(false);
    }
}
