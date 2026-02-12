using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/ExplosionBehaviors/Spark")]
public class SparkExplosionBehavior : ExplosionBehavior
{
    private SparkExplosion sparkExplosion;
    private float sparkExplosionRange = 2f;


    private Coroutine waitCo;
    private int explodeToken = 0;

    public override void Initialize(Explosion explosion)
    {
        base.Initialize(explosion);
        sparkExplosion = explosion as SparkExplosion;
        sparkExplosionRange = 2f;

    }

    //이벤트

    public override void Explode(Vector2 pos)
    {
        PlayExplosionAnim();

        Vector2 pos = sparkExplosion.transform.position;
        Collider2D[] targets = Physics2D.OverlapCircleAll(pos, sparkExplosionRange, sparkExplosion.targetMask);
        //Collider2D[] earthtargets = Physics2D.OverlapCircleAll(pos, sparkExplosionRange, sparkExplosion.EarthMask);
        ApplyExplosion(targets);
        


        // 코루틴 중복 방지 코드라고함
        explodeToken++;
        if (waitCo != null)
            sparkExplosion.StopCoroutine(waitCo);
        var owner = sparkExplosion;
        int token = explodeToken;

        waitCo = owner.StartCoroutine(Co_WaitAnimEnd(owner, token));
    }


    private void PlayExplosionAnim()
    {
        var anim = sparkExplosion.animator;
        if (!anim)
            return;

        anim.gameObject.SetActive(true);
        anim.enabled = true;
        anim.Play(0, 0, 0f);
        anim.Update(0f);
    }

    private IEnumerator Co_WaitAnimEnd(SparkExplosion owner, int token)
    {
        Animator anim = owner.animator;
        yield return null;

        while (true)
        {
            if (token != explodeToken)
                yield break;

            if (!anim.IsInTransition(0))
            {
                var state = anim.GetCurrentAnimatorStateInfo(0);
                if (state.normalizedTime >= 1f)
                    break;
            }
            yield return null;
        }

        if (token == explodeToken)
            ExplosionEnd();
    }
}
