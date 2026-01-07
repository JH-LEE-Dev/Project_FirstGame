using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/CardEffect/Bullet/DrawAgain")]
public class DrawAgainStrategy : CardEffectStrategy
{
    [SerializeField] private int drawAmount = 0;

    public override void Execute_Status()
    {

    }

    public override void Execute_System()
    {
        cardLogicSystem.DrawAgain(drawAmount);
    }
}
