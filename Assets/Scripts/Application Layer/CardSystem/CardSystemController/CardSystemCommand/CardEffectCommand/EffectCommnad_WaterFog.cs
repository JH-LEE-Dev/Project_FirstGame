using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/WaterFog")]
public class EffectCommnad_WaterFog : CardEffectCommand<IStatusEffectCommandHandler>
{
    private DebuffElementEffectType targetDebuff = DebuffElementEffectType.Wet;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        var enemies = cardStatusEffectCommandHandler.GetEnemyHandlers();

        using var rentalBuffer = new RentalScope<IEnemyHandler>(enemies.Count);
        Span<IEnemyHandler> writeBuffer = rentalBuffer.Span;
        int enemyCnt = 0;

        using var rentalBuffer_Applied = new RentalScope<IEnemyHandler>(enemies.Count);
        Span<IEnemyHandler> writeBuffer_Applied = rentalBuffer_Applied.Span;
        int enemyCnt_Applied = 0;

        var debuffData = new DebuffElementData(targetDebuff, 1);

        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i].currentAppliedDebuff.ContainsKey(targetDebuff))
            {
                IEnemyHandler target = enemies[i];
                writeBuffer[enemyCnt] = target;
                ++enemyCnt;
            }
        }

        for (int i = 0; i < enemyCnt; ++i)
        {
            if (writeBuffer[i] != null)
            {
                var target = writeBuffer[i];

                if (bUpgraded == false)
                {
                    var targets = GetCollider(target, bUpgraded);

                    if (targets == null)
                        return;

                    for (int j = 0; j < targets.Length; ++j)
                    {
                        var enemyHandler = targets[j].GetComponent<IEnemyHandler>();

                        if (enemyHandler != null && Contains(writeBuffer_Applied, enemyHandler) == false)
                        {
                            writeBuffer_Applied[enemyCnt_Applied] = enemyHandler;
                            ++enemyCnt_Applied;
                            enemyHandler.ApplyElementDebuff(debuffData,enemyHandler.GetTransform().position);
                        }
                    }
                }
                else
                {
                    var targets = GetCollider(target, bUpgraded);

                    if (targets == null)
                        return;

                    for (int j = 0; j < targets.Length; ++j)
                    {
                        var enemyHandler = targets[j].GetComponent<IEnemyHandler>();

                        if (enemyHandler != null && Contains(writeBuffer_Applied, enemyHandler) == false)
                        {
                            writeBuffer_Applied[enemyCnt_Applied] = enemyHandler;
                            ++enemyCnt_Applied;
                            debuffData.turnCnt = 2;
                            enemyHandler.ApplyElementDebuff(debuffData, enemyHandler.GetTransform().position);
                        }
                    }
                }
            }
        }
    }

    private bool Contains(Span<IEnemyHandler> span, IEnemyHandler target)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == target)
                return true;
        }

        return false;
    }

    private Collider2D[] GetCollider(IEnemyHandler _enemyHandler, bool _bUpgraded)
    {
        float localRadius = _enemyHandler.statusCollider.radius;

        float worldScale = _enemyHandler.GetTransform().lossyScale.x;
        float finalRadius = localRadius * worldScale;

        if (_bUpgraded)
            finalRadius *= 2;
        finalRadius *= 5f;
        return Physics2D.OverlapCircleAll(
            _enemyHandler.GetTransform().position,
            finalRadius,
           LayerMask.GetMask("Enemy"));
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}