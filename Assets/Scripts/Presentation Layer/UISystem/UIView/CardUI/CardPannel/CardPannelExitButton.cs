using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPannelExitButton : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Pannel Settings")]
    [SerializeField] private CardPannel cardPannel = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cardPannel?.ExitPannelEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
