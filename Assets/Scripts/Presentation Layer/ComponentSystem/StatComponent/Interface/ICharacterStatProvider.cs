using System.Collections.Generic;

public interface ICharacterStatProvider
{
    int attackCnt { get; } //공격 횟수
    float attackRange { get; } //추가 공격 범위
    float criticalChance { get; } //치명타 확률
    float resultDamage { get; } //최종 공격력
    float defaultAttack { get; } //기본 공격력
    float additionalAttack { get; } //추가 공격력
    int weaknessTurnCnt { get; } //적 약화 디버프 턴 횟수
    float CalcBaseDamage(out bool bCritical);
}
