using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIView_CardSystem : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;

    UIPresenter_CardSystem presenter;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [SerializeField] private List<RectTransform> cards = new();

    [Header("Fan Settings")]
    [SerializeField] private float radius = 100f;          // 부채 반경
    [SerializeField] private float maxAngle = 15f;         // 최대 벌어지는 각도 (좌우)
    [SerializeField] private float verticalOffset = -50f;  // 하단 보정


    protected override void Awake()
    {
        base.Awake();

        presenter = new UIPresenter_CardSystem(this);
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

    public void Refresh()
    {
        int count = cards.Count;
        if (count == 0) return;

        float angleStep = count == 1 ? 0f : (maxAngle * 2f) / (count - 1);
        float startAngle = -maxAngle;

        for (int i = 0; i < count; i++)
        {
            RectTransform card = cards[i];

            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // 부채꼴 위치 계산
            Vector2 pos = new Vector2(
                Mathf.Sin(rad) * radius,
                Mathf.Cos(rad) * radius + verticalOffset
            );

            card.localPosition = pos;

            // 카드 회전 (부채 방향으로)
            card.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }

    /// <summary>
    /// 카드 추가 / 제거 시 호출
    /// </summary>
    public void SetCards(List<RectTransform> newCards)
    {
        cards = newCards;
        Refresh();
    }
}
