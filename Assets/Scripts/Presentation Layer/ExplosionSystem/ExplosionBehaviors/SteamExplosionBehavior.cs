using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/ExplosionBehaviors/Steam")]
public class SteamExplosionBehavior : ExplosionBehavior
{
    //이벤트

    public override void Explode()
    {
        //여기에 폭발 로직을 구현하면 됨.

        //폭발에 휘말린 적들의 Collider를 이 함수에 넣고 호출하면 됨.
        //ApplyExplosion(Collider2D[] _colliders);

        //폭발이 다 끝났으면 이 함수를 호출하면 됨.
        //ExplosionEnd();
    }
}
