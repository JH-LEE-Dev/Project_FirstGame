using UnityEngine;

public class AEH_Impact : MonoBehaviour
{
    private Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ImpactFinished()
    {
        GameObject parentObj = gameObject.transform.parent.gameObject;
        parentObj.SetActive(false);
        animator.SetBool("bImpact", false);
    }
}
