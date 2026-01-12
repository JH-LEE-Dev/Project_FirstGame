using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Strategy/Move/PlayerTempMove")]
public class PlayerTempMoveStrategy : PlayerMoveStrategy
{
    /// <summary>
    /// 속성 존. ---------------------------------------
    /// </summary>
    [Header("Ellipse Settings")]
    [SerializeField] private float radiusX = 5f;
    [SerializeField] private float radiusY = 3f;
    [SerializeField] private float deltaY = 3f;
    [SerializeField] private float angleLimit = 0.1f;
    private Vector2 InitialPos;

    [Header("Movement Settings")]
    [SerializeField] private float angleSpeed = 1.5f;

    private float angle = Mathf.PI / 2;



    /// <summary>
    /// 구현 코드 존. --------------------------------------
    /// </summary>

    //이니셜라이즈 함수.
    public override void Initialize(Unit _unit,IOrbitPathProvider orbitPathProvider)
    {
        unit = _unit;
        InitialPos = unit.transform.position;

        angle = Mathf.Clamp(angle, 0f, Mathf.PI);

        float x = radiusX * Mathf.Cos(angle);
        float y = radiusY * Mathf.Sin(angle);

        unit.transform.position = new Vector3(x, InitialPos.y+y+deltaY, 0f);
    }

    //MoveComponent측에서 매 프레임마다 호출하는 함수. 실질적인 움직임을 담당하는 코드임.
    public override void Move(Vector2 direction)
    {
        direction.x = -direction.x;

        angle += direction.x * angleSpeed * Time.deltaTime;

        angle = Mathf.Clamp(angle, Mathf.PI * (1-angleLimit), Mathf.PI * angleLimit);

        float x = radiusX * Mathf.Cos(angle);
        float y = radiusY * Mathf.Sin(angle);

        unit.transform.position = new Vector3(x, InitialPos.y + y +deltaY, 0f);
    }
}
