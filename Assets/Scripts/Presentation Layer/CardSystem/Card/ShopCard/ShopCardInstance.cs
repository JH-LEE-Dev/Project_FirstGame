using UnityEngine;

public class ShopCardInstance : CardInstance
{

    UIView_Shop uIView_Shop;


    public void Initialize(UIView_Shop shop, Material template)
    {
        uIView_Shop = shop;
        dissolveMatInstance = new Material(template);
        ApplyDissolveMaterialToVisuals();
    }


}
