using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/CardEffect/Bullet/Flare")]
public class FlareStrategy : CardEffectStrategy
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