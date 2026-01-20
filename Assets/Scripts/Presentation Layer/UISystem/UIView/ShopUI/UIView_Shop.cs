using UnityEngine;

public class UIView_Shop : UIView
{
    [SerializeField] private Canvas canvasPrefab;
    private Canvas canvas;

    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);

        canvas = Instantiate(canvasPrefab,transform);
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        CanvasEnabler canvasEnabler = canvas.GetComponent<CanvasEnabler>();
        canvasEnabler.Initialize();
    }
}
