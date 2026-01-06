using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Strategy/Move/Player")]
public class PlayerMoveStrategy : MoveStrategy
{
    [Header("Ellipse Settings")]
    [SerializeField] private float radiusX = 5f;
    [SerializeField] private float radiusY = 3f;
    [SerializeField] private float deltaY = 3f;
    [SerializeField] private float angleLimit = 0.1f;
    private Vector2 InitialPos;

    [Header("Movement Settings")]
    [SerializeField] private float angleSpeed = 1.5f;

    private float angle = Mathf.PI / 2;

    public override void Accelerate(Vector2 direction,float acceleration, float maxSpeed)
    {
    }

    public override async Task AsyncMove(Vector2 direction)
    {
        await Task.Yield();
    }

    public override void Initialize(Unit _unit)
    {
        unit = _unit;
        InitialPos = unit.transform.position;

        angle = Mathf.Clamp(angle, 0f, Mathf.PI);

        float x = radiusX * Mathf.Cos(angle);
        float y = radiusY * Mathf.Sin(angle);

        unit.transform.position = new Vector3(x, InitialPos.y+y+deltaY, 0f);
    }

    public override void Move(Vector2 direction)
    {
        direction.x = -direction.x;

        angle += direction.x * angleSpeed * Time.deltaTime;

        angle = Mathf.Clamp(angle, Mathf.PI * (1-angleLimit), Mathf.PI * angleLimit);

        float x = radiusX * Mathf.Cos(angle);
        float y = radiusY * Mathf.Sin(angle);

        unit.transform.position = new Vector3(x, InitialPos.y + y +deltaY, 0f);
    }

    public override void Move_Impulse(Vector2 direction, float power)
    {
        return;
    }
}
