using UnityEngine;

public interface ICombatSignalHandler
{
    //여기에 필요한 인터페이스를 정의
    void NotifyCombatActionSignal(CombatActionSignal signal);
}
