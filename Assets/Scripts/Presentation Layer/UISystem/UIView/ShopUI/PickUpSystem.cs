using UnityEngine;
using UnityEngine.UI;


public class PickUpSystem : MonoBehaviour
{
    private UIView_Shop uIView_Shop;

    [SerializeField] private Image cancelPannel;
    [SerializeField] private Image pack;
    [SerializeField] private ShopPickUpButton pickUpButton;


    public void Init(UIView_Shop shop)
    {
        uIView_Shop = shop;
        pickUpButton.Init();

    }


}
