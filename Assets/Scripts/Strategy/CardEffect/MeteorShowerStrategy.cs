using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/CardEffect/Bullet/MeteorShower")]
public class MeteorShowerStrategy : CardEffectStrategy
{
    public override void Execute_Status()
    {

    }

    public override void Execute_System()
    {
        cardLogicSystem.AttackAgain();
    }
}