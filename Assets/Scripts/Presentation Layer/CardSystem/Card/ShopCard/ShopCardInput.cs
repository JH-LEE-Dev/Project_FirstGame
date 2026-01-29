using UnityEngine.EventSystems;
using UnityEngine;

public class ShopCardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ShopCardInstance owner;
    public void Bind(ShopCardInstance card)
    {
        owner = card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        owner?.Motion?.HoverOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        owner?.Motion?.HoverOff();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner?.Shop?.ToggleSelect(owner);
    }
}
