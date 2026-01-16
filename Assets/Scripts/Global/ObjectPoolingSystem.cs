using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingSystem : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxPoolSize = 15;

    private ObjectPool<GameObject> pool = null;
    public ObjectPool<GameObject> Pool { get { return pool; } }

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(PoolCreate, PoolGet, PoolRelease, PoolDestroy, maxSize: maxPoolSize);

        for(int i = 0; i < maxPoolSize; ++i)
        {
            GameObject getObj = pool.Get();
            pool.Release(getObj);
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
