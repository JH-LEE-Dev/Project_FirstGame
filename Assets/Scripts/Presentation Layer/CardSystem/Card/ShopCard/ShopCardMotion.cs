using UnityEngine;

public class ShopCardMotion : MonoBehaviour
{
    private ShopCardInstance owner;
    private RectTransform rt;

    private Vector3 originScale;


    public void Bind(ShopCardInstance card)
    {
        owner = card;
        rt = GetComponent<RectTransform>();
        originScale = transform.localScale;
    }

    public void AllKillTweens(bool bRestoreScale = true)
    {
        if (bRestoreScale) transform.localScale = originScale;
    }

    private void Update()
    {

    }


}
