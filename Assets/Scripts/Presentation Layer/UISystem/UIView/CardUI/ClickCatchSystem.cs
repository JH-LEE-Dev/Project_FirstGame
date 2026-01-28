using UnityEngine;
using UnityEngine.EventSystems;
public class ClickCatchSystem : MonoBehaviour, IPointerClickHandler
{
    private UIView_Unit_World unitWorldUI;
    private WorldCanvasEnabler canvasEnabler;

    public void Init(UIView_Unit_World _unitWorldUI)
    {
        unitWorldUI = _unitWorldUI;
        canvasEnabler = GetComponent<WorldCanvasEnabler>();
        canvasEnabler.Initialize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
            unitWorldUI?.CancelPreview();
    }
}
