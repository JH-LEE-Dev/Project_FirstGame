using UnityEngine;


[CreateAssetMenu(menuName = "Strategy/Move/PlayerNormalMove")]
public class PlayerNormalMoveStrategy : PlayerMoveStrategy
{

    public override void Initialize(Unit unit,IOrbitPathProvider _orbitPathProvider)
    {
        orbitPathProvider = _orbitPathProvider;
    }

    public override void Move(Vector2 direction)
    {

    }
}
