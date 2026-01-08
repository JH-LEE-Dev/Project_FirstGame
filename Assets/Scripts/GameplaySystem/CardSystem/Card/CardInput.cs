using UnityEngine.EventSystems;
using UnityEngine;

[DisallowMultipleComponent]
public class CardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardInstance owner;

    public void Bind(CardInstance card) => owner = card;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (owner.Motion != null && owner.Motion.IgnoreHandLayout) return;

        if (CardInstanceType.Hand != owner.cardInstanceType) return;

        owner.CardSystem.OnCardHoverEnter(owner);
        owner.Motion?.HoverOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (owner.Motion != null && owner.Motion.IgnoreHandLayout) return;

        if (CardInstanceType.Hand != owner.cardInstanceType) return;


        owner.CardSystem.OnCardHoverExit(owner);
        owner.Motion?.HoverOff();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (owner.CardSystem.WorkingBlock) return;
        if (CardInstanceType.Hand != owner.cardInstanceType) return;


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
