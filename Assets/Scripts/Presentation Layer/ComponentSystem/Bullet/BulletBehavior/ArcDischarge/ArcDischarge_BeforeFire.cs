using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/ArcDischarge_BeforeFire")]
public class ArcDischarge_BeforeFire : ArcDischargeBehavior
{
    public override void Enter()
    {
        bUpdateEnd = false;
    }

    public override void Update()
    {
        if (true == bUpdateEnd)
            return;

        // TODO: 나중에 연출할 때 사용할 것임
        // 에너지 모아서 기 방출하는 느낌의 썬더 차지느낌


        End();
    }

    public override void End()
    {
        bUpdateEnd = true;
        BulletBehaviorEndEvent?.Invoke();
    }

    public override void Exit()
    {
        BulletEffectEndEvent?.Invoke();
    }
}
