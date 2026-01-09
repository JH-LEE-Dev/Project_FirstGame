using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    private UIView_CardSystem cardSystem;
    public UIView_CardSystem CardSystem => cardSystem;

    [Header("Prefab & Root")]
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private GameObject starEffectPrefab;

    [Header("Pools")]
    // ∫Ò»∞º∫¡ﬂ¿Œ ∆–
    [SerializeField] private List<CardInstance> inactiveHandPool = new();

    // º“∏Í, ø˙»¶, µ¶
    [SerializeField] private List<CardInstance> otherCardPool = new();
    public List<CardInstance> OtherCardPool { get { return otherCardPool; } }

    [SerializeField] private int handPoolSize = 20;
    [SerializeField] private int otherPoolSize = 50;

    // µ¶, ø˙»¶ø°º≠ ªÁøÎ«“ ¿Ã∆Â∆Æ «Æ∏µ
    private ObjectPool<GameObject> starEffects;
    public ObjectPool<GameObject> StarEffects { get { return starEffects; } }

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
        StarPooling();
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

    private void StarPooling()
    {
        int maxPool = 30;

        starEffects = new ObjectPool<GameObject>(
            createFunc: CreateStarEffect,
            actionOnGet: ActivateStarEffect,
            actionOnRelease: DeActivateStarEffect,
            actionOnDestroy: DestroyPoolObj,
            maxSize: maxPool);

        for (int i = 0; i < maxPool; ++i)
        {
            GameObject newObj = starEffects.Get();
            starEffects.Release(newObj);
        }
    }

    // ƒ´µÂ ∑£∆Æ.
    public CardInstance RentHandCard()
    {
        if (inactiveHandPool.Count == 0) return null;

        int last = inactiveHandPool.Count - 1;


        var card = inactiveHandPool[last];
        inactiveHandPool.RemoveAt(last);

        return card;
    }


    // ƒ´µÂ π›≥≥
    public void ReturnHandCard(CardInstance card)
    {
        if (card == null) return;

        card.Clear();
        inactiveHandPool.Add(card);
    }

    private GameObject CreateStarEffect()
    {
        GameObject newObj = Instantiate(starEffectPrefab, this.transform);
        StarEffect script = newObj?.GetComponent<StarEffect>();
        script?.Init(this);

        return newObj;
    }

    private void DestroyPoolObj(GameObject obj)
    {
        Destroy(obj);
    }

    private void ActivateStarEffect(GameObject obj)
    {
    }

    private void DeActivateStarEffect(GameObject obj)
    {
        if (null == obj)
            return;

        StarEffect script = obj?.GetComponent<StarEffect>();
        script?.ReturnToOrigin();

        obj.SetActive(false);
    }

}
