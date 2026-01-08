using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    UIView_CardSystem cardSystem;


    [Header("Prefab & Root")]
    [SerializeField] private GameObject cardUIPrefab;

    [Header("Pools")]
    // ºñÈ°¼ºÁßÀÎ ÆÐ
    [SerializeField] private List<CardInstance> inactiveHandPool = new();

    // ¼Ò¸ê, ¿úÈ¦, µ¦
    [SerializeField] private List<CardInstance> otherCardPool = new();
    public List<CardInstance> OtherCardPool { get { return otherCardPool; } }

    [SerializeField] private int handPoolSize = 20;
    [SerializeField] private int otherPoolSize = 50;

    private void Awake()
    {

    }

    private void OnDisable()
    {
        
    }

    public void Init(UIView_CardSystem owner)
    {
        cardSystem = owner;
        cardPooling();
    }

    private void cardPooling()
    {
        // hands
        for (int i = 0; i < handPoolSize; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, this.transform);
            CardInstance card = go.GetComponent<CardInstance>();
            card.gameObject.SetActive(false);

            card.Initialize(cardSystem, CardInstanceType.Hand);
            inactiveHandPool.Add(card);

        }

        // other
        for (int i = 0; i < otherPoolSize; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, cardSystem?.PannelContent.transform);
            go.transform.localScale = new Vector3(5f, 5f, 1f);
            CardInstance card = go.GetComponent<CardInstance>();
            card.gameObject.SetActive(false);
            card.Initialize(cardSystem, CardInstanceType.Other);

            otherCardPool.Add(card);
        }
    }

    // Ä«µå ·£Æ®.
    public CardInstance RentHandCard()
    {
        if (inactiveHandPool.Count == 0) return null;

        int last = inactiveHandPool.Count - 1;


        var card = inactiveHandPool[last];
        inactiveHandPool.RemoveAt(last);

        return card;
    }


    // Ä«µå ¹Ý³³
    public void ReturnHandCard(CardInstance card)
    {
        if (card == null) return;

        card.Clear();
        inactiveHandPool.Add(card);
    }
}
