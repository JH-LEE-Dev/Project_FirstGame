using UnityEngine;

public class AEH_EnemyExplosion : MonoBehaviour
{
    private Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ExplosionFinished()
    {
        GameObject parentObj = gameObject.transform.parent.gameObject;
        parentObj.SetActive(false);
        animator.SetBool("bExplode", false);
    }
}
