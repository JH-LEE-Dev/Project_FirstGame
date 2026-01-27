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

            bool isSelectMode = owner.CardSystem.HandSystem.GetChooseMode();

            if (isSelectMode == false)
            {
                // 호 벌리기
                owner.CardSystem.OnCardHoverEnter(owner);
                // 크게 만들기
                owner.Motion?.HoverOn();
            }
            else
            {
                // 호 벌리기
                owner.CardSystem.OnCardHoverEnter(owner);
                // 크게 만들기
                owner.Motion?.SelectHoverOn();
            }
        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            if (null == owner.CardSystem.CardPannel)
                return;

            bool isSelectMode = owner.CardSystem.CardPannel.PannelSelectMode;

            if (isSelectMode)
            {
                // 모션 생각 중.
                Debug.Log("지금 셀렉트 모드로 패널에서 Hover On");
            }
        }
    }

    // 호버OFF
    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner == null || owner.CardSystem == null) return;


        if (CardInstanceType.Hand == owner.cardInstanceType)
        {
            if (bIgnoreHover) return;

            bool isSelectMode = owner.CardSystem.HandSystem.GetChooseMode();

            if (isSelectMode == false)
            {
                // 호 닫기
                owner.CardSystem.OnCardHoverExit(owner);
                // 작게 만들기
                owner.Motion?.HoverOff();
            }
            else
            {
                // 호 닫기
                owner.CardSystem.OnCardHoverExit(owner);
                // 크게 만들기
                owner.Motion?.SelectHoverOff();
            }
        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            if (null == owner.CardSystem.CardPannel)
                return;

            bool isSelectMode = owner.CardSystem.CardPannel.PannelSelectMode;

            if (isSelectMode)
            {
                // 모션 생각 중.
                Debug.Log("지금 셀렉트 모드로 패널에서 Hover Off");
            }
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

            bool isSelectMode = owner.CardSystem.HandSystem.GetChooseMode();

            if (isSelectMode == false)
            {
                // 사용 가능한 놈들 : 프리뷰 or 패에 있는 카드만 사용 가능.
                if ((CardState.InHand == owner.cardState ||
                    CardState.Preview == owner.cardState) == false) return;

                OnPointerClickNormalMode(eventData);
            }
            else
            {
                OnPointerClickChooseMode(eventData);
            }
        }
        else if (CardInstanceType.Other == owner.cardInstanceType)
        {
            if (null == owner.CardSystem.CardPannel)
                return;

            bool isSelectMode = owner.CardSystem.CardPannel.PannelSelectMode;

            if (isSelectMode)
            {
                // 모션 생각 중.
                OnPointerClickChooseModeforPannel();
                Debug.Log("지금 셀렉트 모드로 패널에서 Clicked");
            }
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
        owner.CardSystem.HandSystem.ToggleSelect(owner);
    }

    private void OnPointerClickChooseModeforPannel()
    {
        owner.CardSystem.CardPannel.ToggleSelect(owner);
    }
}
