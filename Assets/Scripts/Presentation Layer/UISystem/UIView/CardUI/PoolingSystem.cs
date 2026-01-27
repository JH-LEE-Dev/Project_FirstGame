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
    // ºñÈ°¼ºÁßÀÎ ÆÐ
    [SerializeField] private List<MainCardInstance> inactiveHandPool = new();

    // ¼Ò¸ê, ¿úÈ¦, µ¦
    [SerializeField] private List<MainCardInstance> otherCardPool = new();
    public List<MainCardInstance> OtherCardPool { get { return otherCardPool; } }

    private int handPoolSize = 20;
    private int otherPoolSize = 50;

    // µ¦, ¿úÈ¦¿¡¼­ »ç¿ëÇÒ ÀÌÆåÆ® Ç®¸µ
    private ObjectPool<GameObject> starEffects;
    public ObjectPool<GameObject> StarEffects { get { return starEffects; } }

    // Ä«µå¿¡¼­ »ç¿ëÇÒ ÀÌÆåÆ® Ç®¸µ
    [SerializeField] private VFX_CardUseEffect UseCardEffectPrefab;
    private ObjectPool<VFX_CardUseEffect> UseCardEffects;
    private int defaultCapacity = 10;
    private int maxSize = 50;


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
        UseCardEffectPooling();
    }

    private void cardPooling()
    {
        // hands
        for (int i = 0; i < handPoolSize; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, this.transform);
            MainCardInstance card = go.GetComponent<MainCardInstance>();
            card.gameObject.SetActive(false);

            card.Initialize(cardSystem, CardInstanceType.Hand);
            inactiveHandPool.Add(card);

        }

        // other
        for (int i = 0; i < otherPoolSize; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, cardSystem?.PannelContent.transform);
            go.transform.localScale = new Vector3(5f, 5f, 1f);

            MainCardInstance card = go.GetComponent<MainCardInstance>();
            if (null == card) continue;

            card.TurnOffGlowFilter();
            card.gameObject.SetActive(false);
            card.Initialize(cardSystem, CardInstanceType.Other);

            otherCardPool.Add(card);
        }
    }

    private void StarPooling()
    {
        int maxPool = 50;

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

    private void UseCardEffectPooling()
    {
        UseCardEffects = new ObjectPool<VFX_CardUseEffect>(
                    createFunc: () =>
                    {
                        var inst = Instantiate(UseCardEffectPrefab, transform);
                        inst.gameObject.SetActive(false);
                        inst.SetReleaseHandler(UseEffectRelease);
                        return inst;
                    },
                    actionOnGet: e => e.gameObject.SetActive(true),
                    actionOnRelease: e => e.gameObject.SetActive(false),
                    actionOnDestroy: e => Destroy(e.gameObject),
                    collectionCheck: false,
                    defaultCapacity: defaultCapacity,
                    maxSize: maxSize
                );
    }

    private void UseEffectRelease(VFX_CardUseEffect e) => UseCardEffects.Release(e);

    public void PlayMagicCardEffect(Vector3 worldPos, float initialLocalScale, System.Action onComplete = null)
    {
        var e = UseCardEffects.Get();
        e.Play(worldPos, initialLocalScale, onComplete);
    }



    // Ä«µå ·£Æ®.
    public MainCardInstance RentHandCard()
    {
        if (inactiveHandPool.Count == 0) return null;

        int last = inactiveHandPool.Count - 1;


        var card = inactiveHandPool[last];
        inactiveHandPool.RemoveAt(last);

        return card;
    }


    // Ä«µå ¹Ý³³
    public void ReturnHandCard(MainCardInstance card)
    {
        if (card == null) return;

        card.Clear();
        inactiveHandPool.Add(card);
    }

    private GameObject CreateStarEffect()
    {
        GameObject newObj = Instantiate(starEffectPrefab, this.transform);
        VFX_CardStar script = newObj?.GetComponent<VFX_CardStar>();
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

        obj.SetActive(false);
    }

}
