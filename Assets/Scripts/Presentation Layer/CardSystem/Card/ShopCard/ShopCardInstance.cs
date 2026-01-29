using UnityEngine;

public class ShopCardInstance : CardInstance
{
    private UIView_Shop uIView_Shop;
    public UIView_Shop Shop => uIView_Shop;


    public ShopCardMotion Motion { get; private set; }
    public ShopCardVisual Visual { get; private set; }
    public ShopCardInput Input { get; private set; }

    private void Awake()
    {
        Motion = GetComponent<ShopCardMotion>();
        Input = GetComponent<ShopCardInput>();
        Visual = GetComponentInChildren<ShopCardVisual>(true);

        if (Motion) Motion.Bind(this);
        if (Input) Input.Bind(this);
        if (Visual) Visual.Bind(this);
    }

    public virtual void Initialize(UIView_Shop shop, Material template, ICardLocalizationSystem cls)
    {
        base.Initialize(template, cls);
        uIView_Shop = shop;
    }
    public void SetVisible(bool visible)
    {
        Visual?.SetVisible(visible);
    }
}
