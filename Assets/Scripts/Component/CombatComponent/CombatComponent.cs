using UnityEngine;

public class CombatComponent : EntityComponent
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    protected ICombatSignalHandler combatSignalHandler;







    /// <summary>
    /// 구현 속성 존. -----------------------------------------
    /// </summary>












    /// <summary>
    /// 시스템 코드 존. -----------------------------------------
    /// </summary>

    public void Initialize(UnitContext _ctx, ICombatSignalHandler _combatSignalHandler)
    {
        base.Initialize(_ctx);
        combatSignalHandler = _combatSignalHandler;
    }






    /// <summary>
    /// 구현 코드 존. -----------------------------------------
    /// </summary>
}
