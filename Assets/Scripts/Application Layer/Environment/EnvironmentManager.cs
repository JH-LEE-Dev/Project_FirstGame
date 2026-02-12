using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IOrbitPathProvider
{
    private OrbitPathComponent orbitPathComponent;
    private FallBoundaryComponent fallBoundaryLineComponent;

    public void Initialize()
    {

    }

    public void Release()
    {

    }

    // OrbitPath
    public Vector3 GetPathPosition(float value)
    {
        return orbitPathComponent.GetPathPosition(value);
    }

    public void SetPathActive(bool value)
    {
        orbitPathComponent.SetPathActive(value);
        fallBoundaryLineComponent.SetPathActive(value);
    }

    // FallBoundaryComponent

    // 덩어리로 추가하는 함수
    // clearBefore를 true로 하면, 넣은 덩어리로 교체이고 false때리면 기존거에 덩어리 더 추가 
    public void SetMonsters(IEnumerable<Transform> list, bool clearBefore = true)
    {
        fallBoundaryLineComponent.SetMonsters(list, clearBefore);
    }
    // 등록
    public void RegisterMonster(Transform monster)
    {
        fallBoundaryLineComponent.RegisterMonster(monster);
    }
    // 등록 해제
    public bool UnregisterMonster(Transform monster)
    {
        return fallBoundaryLineComponent.UnregisterMonster(monster);
    }
    // 그냥 싸그리 날리는 함수 (씬넘어가거나, 그럴때 추천임
    public void CleanupMonsters()
    {
        fallBoundaryLineComponent.CleanupMonsters();
    }




    private void Awake()
    {
        orbitPathComponent = GetComponentInChildren<OrbitPathComponent>();
        fallBoundaryLineComponent = GetComponentInChildren<FallBoundaryComponent>();
    }
}
