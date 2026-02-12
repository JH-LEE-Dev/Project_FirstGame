using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingSystem : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private GameObject prefab;
    public int maxPoolSize = 15;

    private ObjectPool<GameObject> pool = null;
    private List<GameObject> poolList;
    public ObjectPool<GameObject> Pool { get { return pool; } }

    public List<GameObject> GetPoolList() => poolList;
    private void Awake()
    {
        pool = new ObjectPool<GameObject>(PoolCreate, PoolGet, PoolRelease, PoolDestroy, maxSize: maxPoolSize);
        poolList = new List<GameObject>(maxPoolSize);

        for (int i = 0; i < maxPoolSize; ++i)
        {
            GameObject getObj = pool.Get();
            poolList.Add(getObj);
            pool.Release(getObj);
        }
    }

    private GameObject PoolCreate()
    {
        return Instantiate(prefab, transform);
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
