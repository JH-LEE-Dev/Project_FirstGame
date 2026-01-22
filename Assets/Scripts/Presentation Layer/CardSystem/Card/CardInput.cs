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



    public void Bind(MainCardInstance card) => owner = card;

    // 호버ON
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;

        if (CardInstanceType.Hand == owner.cardInstanceType)
        {
            if (bIgnoreHover) return;
            // 호 벌리기
            owner.CardSystem.OnCardHoverEnter(owner);
            // 크게 만들기
            owner.Motion?.HoverOn();
        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            // 상우 존.

            // 호버 ON 상황 연출 넣을거면 넣기. 따로 너만의 Component 추가해서 모션추가해도됨


        }
    }

    // 호버OFF
    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;


        if (CardInstanceType.Hand == owner.cardInstanceType)
        {
            if (bIgnoreHover) return;

            // 호 닫기
            owner.CardSystem.OnCardHoverExit(owner);
            // 작게 만들기
            owner.Motion?.HoverOff();
        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            // 상우 존.

            // 호버 OFF인 상황 연출 넣을거면 넣기. 따로 너만의 Component 추가해서 모션추가해도됨


        }
    }

    // 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;
        if (owner.CardSystem.WorkingBlock) return;
        if (CardInstanceType.Hand == owner.cardInstanceType)
        {
            if (bIgnoreHover) return;

            // 사용 가능한 놈들 : 프리뷰 or 패에 있는 카드만 사용 가능.
            if ((CardState.InHand == owner.cardState ||
                CardState.Preview == owner.cardState) == false) return;

            if (owner.CardSystem.GetChooseMode() == false)
                OnPointerClickNormalMode(eventData);
            else if (owner.CardSystem.GetChooseMode() == true)
                OnPointerClickChooseMode(eventData);

        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            // 상우 존.

            // 클릭했을 때의 상황임.

            // owner.CardSystem == UIView_CardSystem임.
        }
    }




    ///////////////    ///////////////    ///////////////
    private void OnPointerClickNormalMode(PointerEventData eventData)
    {
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
    private void OnPointerClickChooseMode(PointerEventData eventData)
    {

    }
}
