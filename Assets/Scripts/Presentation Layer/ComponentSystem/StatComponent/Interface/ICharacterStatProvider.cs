using UnityEngine;

public interface ICharacterStatProvider
{
    int attackCnt { get; } //공격 횟수
    float attackRange { get; } //추가 공격 범위
    float criticalChance { get; } //치명타 확률
    float resultDamage { get; } //공격력
    int weaknessTurnCnt { get; } //적 약화 디버프 턴 횟수
    float totalDamage { get; }
    float totalDamageValue { get; }
}
