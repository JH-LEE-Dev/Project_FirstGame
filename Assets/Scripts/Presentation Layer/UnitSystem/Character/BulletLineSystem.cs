using UnityEngine;

public class BulletLineSystem : MonoBehaviour
{
    private UIView_Unit_World uIView_Unit_World;
    private Transform characterTransform;



    public void Init(UIView_Unit_World owner, Transform ct)
    {
        if (owner) uIView_Unit_World = owner;
        if (ct) characterTransform = ct;
    }
}
