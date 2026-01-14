using UnityEngine;
using UnityEngine.Pool;

public class UIDamage_Pooling : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private GameObject prefab;

    private ObjectPool<GameObject> damagePool = null;
    public ObjectPool<GameObject> DamagePool { get { return damagePool; } }

    private void Awake()
    {
        int poolCnt = 15;

        damagePool = new ObjectPool<GameObject>(PoolCreate, PoolGet, PoolRelease, PoolDestroy, maxSize: poolCnt);

        for(int i = 0; i < poolCnt; ++i)
        {
            GameObject getObj = damagePool.Get();
            damagePool.Release(getObj);
        }
    }

    private GameObject PoolCreate()
    {
        return Instantiate(prefab, this.transform);
    }

    private void PoolGet(GameObject target)
    {
    }

    private void PoolRelease(GameObject target)
    {
        if (null == target)
            return;

        target.SetActive(false);
    }

    private void PoolDestroy(GameObject target)
    {
        if (null == target)
            return;

        Destroy(target);
    }
}
