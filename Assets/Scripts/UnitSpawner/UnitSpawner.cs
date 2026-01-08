using NUnit.Framework;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [Header("Unit Prefabs")]
    private GameObject unitPrefab;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject enemyUnitPrefab;
    [SerializeField] private GameObject spawnPoint;

    private InputManager inputManager;
    private WaveManager waveManager;
    private GameServiceLocator gameServiceLocator;
    private ICardSystemEvent cardSystemEvent;
    private ICardSystemActions cardSystemActions;
    private GameController gameController;
    private UnitLogicSystem unitLogicSystem;

    private uint curUnitCnt;

    public Character characterUnit { get; private set; }

    [Header("Wave Spawn Settings")]
    [SerializeField] private GameObject waveSpawnPoint;
    public float radiusX = 7f;        // 타원의 가로 반지름
    public float radiusY = 3f;        // 타원의 세로 반지름
    [SerializeField] EnemyTypeDataBase enemyTypeDataBase;

    [Header("Enemy Target Point")]
    [SerializeField] private GameObject enemyTargetPoint;

    private GameRuleEventController gameRuleEventController;

    private List<Enemy> enemies = new List<Enemy>();

    public void Initiallize(InputManager _inputManager, WaveManager _waveManager, 
        GameServiceLocator _gameServiceLocator,ICardSystemEvent _cardSystemEvent,
        ICardSystemActions _cardSystemActions,GameController _gameController,
        UnitLogicSystem _unitLogicSystem)
    {
        inputManager = _inputManager;
        waveManager = _waveManager;
        gameServiceLocator = _gameServiceLocator;
        cardSystemEvent = _cardSystemEvent;
        cardSystemActions = _cardSystemActions;
        gameController = _gameController;
        gameRuleEventController = new GameRuleEventController();
        unitLogicSystem = _unitLogicSystem;

        if (inputManager == null)
        {
            Debug.Log("inputReader is null -> UnitSpawner::Initialize");
            return;
        }

        waveManager.SpawnWaveEvent += SpawnWave;

        SpawnCharacter();
    }
    public void OnDestroy()
    {
        gameRuleEventController.Release(characterUnit, gameController, cardSystemEvent,cardSystemActions);
        waveManager.SpawnWaveEvent -= SpawnWave;
    }

    private void SpawnCharacter()
    {
        GameObject spawnedObject = Instantiate(characterPrefab, spawnPoint.transform);

        if (spawnedObject == null)
            return;

        Character spawnedUnit = spawnedObject.GetComponent<Character>();

        if (spawnedUnit != null)
        {
            spawnedUnit.Initialize_Character(inputManager, gameServiceLocator);
            gameRuleEventController.Bind(spawnedUnit, gameController, cardSystemEvent,cardSystemActions);
            characterUnit = spawnedUnit;
        }
    }

    Vector3 GetRandomPointInEllipse()
    {
        // 1. 무작위 각도 (0 ~ 2π) 생성
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        // 2. 균일한 분포를 위해 루트(sqrt)를 씌운 무작위 거리값 생성
        // sqrt를 사용하지 않으면 중심부에 밀도가 높아집니다.
        float r = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));

        // 3. 타원의 방정식 기반 좌표 계산
        float x = Mathf.Cos(angle) * radiusX * r;
        float y = Mathf.Sin(angle) * radiusY * r;

        return new Vector3(waveSpawnPoint.transform.position.x + x, waveSpawnPoint.transform.position.y + y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = waveSpawnPoint.transform.localToWorldMatrix;

        int segments = 50;
        Vector3 lastPoint = new Vector3(radiusX, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float a = i * Mathf.PI * 2 / segments;
            Vector3 nextPoint = new Vector3(Mathf.Cos(a) * radiusX, Mathf.Sin(a) * radiusY, 0);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

        Gizmos.matrix = oldMatrix;
    }

    public void SpawnWave(uint cnt)
    {
        curUnitCnt = cnt;

        for (uint i = 0; i < curUnitCnt; ++i)
        {
            Vector3 spawnPosition = GetRandomPointInEllipse();

            GameObject spawnedObject = Instantiate(enemyUnitPrefab, spawnPosition, Quaternion.identity);

            if (spawnedObject == null)
                return;

            Enemy spawnedUnit = spawnedObject.GetComponent<Enemy>();

            if (spawnedUnit != null)
            {
                int randomInt = UnityEngine.Random.Range(0, enemyTypeDataBase.enemyData.Count - 1);

                EnemyTypeData enemyTypeData = enemyTypeDataBase.GetEnemyData(randomInt);

                spawnedUnit.Initialize_Enemy(inputManager, gameServiceLocator, waveManager, enemyTypeData);
                spawnedUnit.SetTargetPoint(enemyTargetPoint.transform.position);

                enemies.Add(spawnedUnit);
            }
        }

        SetUnitLogicSystem();
    }

    private void SetUnitLogicSystem()
    {
        unitLogicSystem.Initialize(characterUnit, enemies);
    }
}
