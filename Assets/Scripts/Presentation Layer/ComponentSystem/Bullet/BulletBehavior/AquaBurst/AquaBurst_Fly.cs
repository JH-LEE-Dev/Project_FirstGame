using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(menuName = "Strategy/BulletBehavior/AquaBurst/AquaBurst_Fly")]

public class AquaBurst_Fly : AquaBurstBehavior
{
    private float mulTime = 8f;
    private float baseSpeed;

    public override void Enter()
    {
        base.Enter();
        SetBulletInitialPosition();
        baseSpeed = aquaBurst.speed;

        directHitEnemy = null;
        aquaBurst.directHitEnemy = null;

        if (aquaBurst.animator != null)
        {
            aquaBurst.animator.gameObject.SetActive(true);
            aquaBurst.animator.enabled = true;
            aquaBurst.animator.speed = 1f;
            aquaBurst.animator.Play(0, 0, 0f);
            aquaBurst.animator.Update(0f);
        }

        RotateToDirection(aquaBurst.initDir);
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        aquaBurst.speed -= Time.deltaTime * mulTime;
        if (aquaBurst.speed < 0f)
            aquaBurst.speed = 0f;

        BulletUpdate();
    }

    protected override Vector2 ComputeNextPosition(Vector2 current)
    {
        return current + aquaBurst.initDir * aquaBurst.speed * Time.deltaTime;
    }

    protected override ProjectileState TryStop()
    {
        if (IsAnimFinished())
            return ProjectileState.Exit;

        return ProjectileState.None;
    }

    public override void End()
    {
        if (directHitEnemy != null)
        {
            aquaBurst.directHitEnemy = directHitEnemy;
            bool bCritical;
            float damage = damageSystem
            .GetDamageCalc<IAquaBurstDamageCalculator>()
            .GetDefaultDamage(out bCritical);

            ApplyDamage(directHitEnemy, damage, bCritical);
            ApplyKnockBack(directHitEnemy, 4f);
        }

        StopAnim();
        base.End();
    }

    public override void Exit()
    {
        StopAnim();
        base.Exit();
    }


    private void StopAnim()
    {
        if (aquaBurst.animator != null)
        {
            aquaBurst.animator.enabled = false;
            aquaBurst.animator.gameObject.SetActive(false);
        }
        aquaBurst.speed = baseSpeed;
    }
    private bool IsAnimFinished()
    {
        if (aquaBurst.animator == null || !aquaBurst.animator.enabled)
            return false;

        var st = aquaBurst.animator.GetCurrentAnimatorStateInfo(0);
        return st.normalizedTime >= 1f;
    }

    private void RotateToDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;

        aquaBurst.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
