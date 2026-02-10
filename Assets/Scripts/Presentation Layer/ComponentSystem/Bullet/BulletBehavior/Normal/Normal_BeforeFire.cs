using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_BeforeFire")]
public class Normal_BeforeFire : BulletBehavior_ProjectileBeforeFire
{
    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        End();
    }
}