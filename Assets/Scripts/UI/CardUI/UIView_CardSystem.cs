using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_CardSystem : UIView
{
    public event Action TurnFinishedEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text deckCntText;
    [SerializeField] private TMP_Text graveCntText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button turnFinishedButton;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [SerializeField] private List<CardInstance> cards = new();

    [Header("Fan Settings")]
    [SerializeField] private float radius = 100f;          // 부채 반경
    [SerializeField] private float maxAngle = 15f;         // 최대 벌어지는 각도 (좌우)
    [SerializeField] private float verticalOffset = -50f;  // 하단 보정

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        base.OnShow();

        Refresh();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void RenderUI()
    {

    }

    public void CardDrawed(CardInstance cardInstance)
    {
        cardInstance.GetComponent<RectTransform>().SetParent(handRoot, false);
        cardInstance.CardUsedEvent -= CardUsed;
        cardInstance.CardUsedEvent += CardUsed;

        cards.Add(cardInstance);

        Refresh();

        deckCntText.text = "Deck : " + viewCtx.deckProvider.deckCnt.ToString();
    }

    public void Refresh()
    {
        int count = cards.Count;

        if (count == 0)
            return;

        float angleStep = count == 1 ? 0f : (maxAngle * 2f) / (count - 1);
        float startAngle = -maxAngle;

        for (int i = 0; i < count; i++)
        {
            RectTransform card = cards[i].GetComponent<RectTransform>();

            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // 부채꼴 위치 계산
            Vector2 pos = new Vector2(
                Mathf.Sin(rad) * radius,
                Mathf.Cos(rad) * radius + verticalOffset
            );

            if (card == null)
            {
                Debug.Log("Card is null!");
                return;
            }

            card.localPosition = pos;

            // 카드 회전 (부채 방향으로)
            card.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }

    public void CardUsed(CardInstance usedCard)
    {
        if (viewCtx.deckProvider.CardUsed(usedCard) == false)
            return;

        usedCard.gameObject.SetActive(false);
        cards.Remove(usedCard);
        Refresh();
        graveCntText.text = "Grave : " + viewCtx.deckProvider.graveCnt.ToString();
    }

    public void CardUsingFinished()
    {
        turnFinishedButton.gameObject.SetActive(false);
        TurnFinishedEvent?.Invoke();
    }

    public void CardDrawFinished()
    {
        turnFinishedButton.gameObject.SetActive(true);
    }
}
