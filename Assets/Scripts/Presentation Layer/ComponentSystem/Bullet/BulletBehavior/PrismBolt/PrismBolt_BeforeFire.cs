using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/PrismBolt_BeforeFire")]
public class PrismBolt_BeforeFire : BulletBehavior_ProjectileBeforeFire
{
    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        End();
    }
}
