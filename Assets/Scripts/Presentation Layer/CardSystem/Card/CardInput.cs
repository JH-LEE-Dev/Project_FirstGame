using UnityEngine.EventSystems;
using UnityEngine;

[DisallowMultipleComponent]
public class CardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private MainCardInstance owner;

    private bool bIgnoreHover = false;

    public void SetIgnoreHover(bool value)
    {
        bIgnoreHover = value;
    }
    private void Update()
    {
        Debug.Log(bIgnoreHover);
    }
    public void Bind(MainCardInstance card) => owner = card;

    // 호버되는 상황.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;

        // 패가 아니면 반응하지 않는다.
        if (CardInstanceType.Hand != owner.cardInstanceType) return;

        if (bIgnoreHover) return;

        // 최종적으로 handsystem이 패를 벌려줌.
        owner.CardSystem.OnCardHoverEnter(owner);
        // 커지는 단순한 모션.
        owner.Motion?.HoverOn();
    }

    // 호버 OFF 되는 상황
    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (CardInstanceType.Hand != owner.cardInstanceType) return;

        if (bIgnoreHover) return;

        // 호버카드 null만들고 다시 부채꼴 재 계산.
        owner.CardSystem.OnCardHoverExit(owner);
        owner.Motion?.HoverOff();
    }

    // HandSystem와 관련된 카드가 아니면 일단 거른다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (owner.CardSystem.WorkingBlock) return;
        if (CardInstanceType.Hand != owner.cardInstanceType) return;

        // 사용 가능한 놈들 : 프리뷰 or 패에 있는 카드만 사용 가능.
        if ((CardState.InHand == owner.cardState ||
            CardState.Preview == owner.cardState) == false) return;

        if (bIgnoreHover) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            owner.CardSystem.TryUseCard(owner);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            owner.CardSystem.OnCardLeftClick(owner);
            return;
        }
    }
}
