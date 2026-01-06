using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Strategy/CardEffect/Bullet/Normal")]
public class ST_NormalBulletCardEffect : CardEffectStrategy
{
    public override void Execute()
    {
        
    }

    public override void Initialize(Character _character)
    {
        character = _character;

    }
}