using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/Move/Enemy/Normal")]
public class EnemyTempMoveStrategy : EnemyMoveStrategy
{
    /// <summary>
    /// 구현 속성 존, ----------------------------------------
    /// </summary>
    [SerializeField] private float forceDelta = 1f;

    private Rigidbody2D rb;

    public override void Initialize(Unit _unit)
    {
        unit = _unit;

        rb = unit.GetComponent<Rigidbody2D>();
    }

    //RigidBody에 Impulse를 적용하여 움직이는 함수임, 매 프레임 호출이 아니라
    //한 번만 트리거됨.
    public override void Move_Impulse(Vector2 direction, float power)
    {
        forceDelta = UnityEngine.Random.Range(-forceDelta, forceDelta);
        power += forceDelta;

        if (rb != null)
            rb.AddForce(direction * power, ForceMode2D.Impulse);
    }

    //이건 지구로 가속할 때 매 프레임 호출되는 함수.
    public override void Accelerate(Vector2 direction,float acceleration, float maxSpeed)
    {
        Vector2 v = rb.linearVelocity;
        v += direction * acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = Vector2.ClampMagnitude(v, maxSpeed);
    }
}