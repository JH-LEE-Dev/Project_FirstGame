using System.Collections.Generic;
using UnityEngine;

public class ShopPoolingSystem : MonoBehaviour
{
    UIView_Shop uIView_Shop;

    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Material dissolveTemplate;
    [SerializeField] private List<ShopCardInstance> cardPool = new();

    public List<ShopCardInstance> CardPool;

    private int poolSize = 50;

    public void Init(UIView_Shop owner)
    {
        uIView_Shop = owner;
        cardPooling();
    }

    private void cardPooling()
    {
        for (int i = 0; i < poolSize; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, transform);
            ShopCardInstance card = go.GetComponent<ShopCardInstance>();
            card.gameObject.SetActive(false);

            card.Initialize(uIView_Shop, dissolveTemplate);
            cardPool.Add(card);
        }
    }

    // Ä«µå ·»Æ®
    public ShopCardInstance RentCard()
    {
        if (cardPool.Count == 0) return null;

        int last = cardPool.Count - 1;

        var card = cardPool[last];
        cardPool.RemoveAt(last);

        card.gameObject.SetActive(true);
        return card;
    }
    // Ä«µå ¹Ý³³
    public void ReturnCard(ShopCardInstance card)
    {
        if (card == null) return;

        card.Clear();
        card.gameObject.SetActive(false);
        cardPool.Add(card);
    }

}
