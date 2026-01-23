using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/RiftDetection")]
public class EffectCommand_RiftDetection : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int weaknessTurn = 0;
    [SerializeField] private int bonusAttack = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyAttackModifier(bonusAttack * valueModifier);
        cardStatusEffectCommandHandler.ApplyWeaknessModifier(weaknessTurn * valueModifier);

        ResetCommandData();
    }
}
