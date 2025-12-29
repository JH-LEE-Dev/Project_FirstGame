using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Strategy/Move/Enemy/Normal")]
public class NormalEnemyMoveStrategy : MoveStrategy
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float accelTime = 0.2f;
    [SerializeField] private float decelTime = 0.2f;
    [SerializeField] private float force = 2f;
    [SerializeField] private float forceDelta = 1f;

    private Rigidbody2D rb;

    private Vector2 initialPos;

    public override void Initialize(Unit _unit)
    {
        forceDelta = UnityEngine.Random.Range(-forceDelta, forceDelta);
        force += forceDelta;

        unit = _unit;
        initialPos = unit.transform.position;
        rb = unit.GetComponent<Rigidbody2D>();
    }

    public override async Task AsyncMove(Vector2 direction)
    {
        direction.Normalize();

        float accel = maxSpeed / accelTime;
        float decel = maxSpeed / decelTime;

        float speed = 0f;

        while (speed < maxSpeed)
        {
            speed += accel * Time.fixedDeltaTime;
            speed = Mathf.Min(speed, maxSpeed);

            rb.linearVelocity = direction * speed;
            await Task.Yield();
        }

        while (speed > 0f)
        {
            speed -= decel * Time.fixedDeltaTime;
            speed = Mathf.Max(speed, 0f);

            rb.linearVelocity = direction * speed;
            await Task.Yield();
        }

        rb.linearVelocity = Vector2.zero;
    }

    public override void Move(Vector2 direction)
    {
        if (rb != null)
            rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}