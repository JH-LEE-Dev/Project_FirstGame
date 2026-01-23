using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class SelectEndButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image button;

    private HandSystem owner;

    private bool bCanClick;
    private bool bIsActive;

    public void Init(HandSystem _owner)
    {
        owner = _owner;
        button.raycastTarget = bIsActive = bCanClick = false;
        ComputeAlpha();
    }

    public void SelectEndButtonActive(bool active)
    {
        button.raycastTarget = bIsActive = active;
        ComputeAlpha();
    }

    public void SetCanClick(bool value)
    {
        bCanClick = value;
        ComputeAlpha();
    }



    private void ComputeAlpha()
    {
        if (bIsActive == false) SetAlphaImmediate(0f);
        else if (bIsActive == true && bCanClick == false) SetAlphaImmediate(0.3f);
        else if (bIsActive == true && bCanClick == true) SetAlphaImmediate(1f);
    }

    private void SetAlphaImmediate(float alpha)
    {
        Color c = button.color;
        c.a = alpha;
        button.color = c;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bCanClick == false) return;

        owner.EndCardSelectMode();
    }
}
