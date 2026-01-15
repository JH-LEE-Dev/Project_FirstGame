using NUnit.Framework;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections.Generic;
using UnityEngine.Pool;
using Unity.VisualScripting;

public class UnitSpawner : MonoBehaviour, IUnitEventAccessor, IUnitSpawnSystemEvent, IUnitSignalHubProvider
{
    public ICharacterSignalHub characterSignalHub => characterUnit;
    public IPlayerSignalHub playerSignalHub => earthUnit;

    public event Action<IPlayerData> PlayerSpawnedEvent;
    public event Action EnemySpawnedEvent;
    public event Action<ICharacterData> CharacterSpawnedEvent;

    [Header("Enemy Pool Settings")]
    [SerializeField] const int enemyMaxCount = 40;

    [Header("Unit Prefabs")]
    private GameObject unitPrefab;
    [SerializeField] private Character characterPrefab;
    [SerializeField] private Earth earthPrefab;
    [SerializeField] private Enemy enemyUnitPrefab;
    [SerializeField] private GameObject characterSpawnPoint;
    [SerializeField] private GameObject earthSpawnPoint;

    //외부 의존성
    private InputManager inputManager;
    private IWaveSystemActions waveSystemActions;
    private IWaveSystemEvents waveSystemEvents;
    private GameServiceLocator gameServiceLocator;
    private ICardSystemEvents cardSystemEvents;
    private ICardSystemFlowActions cardSystemFlowActions;
    private IUnitLogicSystemActions unitLogicSystemActions;
    private IOrbitPathProvider orbitPathProvider;

    //내부 의존성
    private GameRuleEventController gameRuleEventController;

    private uint curUnitCnt;

    public Character characterUnit { get; private set; }
    public Earth earthUnit { get; private set; }


    [Header("Wave Spawn Settings")]
    [SerializeField] private GameObject waveSpawnPoint;
    public float radiusX = 7f;        // 타원의 가로 반지름
    public float radiusY = 3f;        // 타원의 세로 반지름
    [SerializeField] EnemyTypeDataBase enemyTypeDataBase;

    [Header("Enemy Target Point")]
    [SerializeField] private GameObject enemyTargetPoint;


    private List<Enemy> enemies = new List<Enemy>(40);

    // Enemy 풀
    ObjectPool<Enemy> enemyPool;

    private void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(
            createFunc: OnCreateEnemy,
            actionOnGet: OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: false,
            defaultCapacity: 40,
            maxSize: 40
        );
    }

    private Enemy OnCreateEnemy()
    {
        Enemy instance = Instantiate(enemyUnitPrefab);
        return instance;
    }

    private void OnGetEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnReleaseEnemy(Enemy enemy)
    {
        waveSystemEvents.StartMoveEvent -= enemy.OnMove;
    }

    //풀 용량 초기화 상황에서 Enemy 파괴.
    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void Initiallize(InputManager _inputManager, IWaveSystemActions _waveSystemActions,
        IWaveSystemEvents _waveSystemEvents,
        GameServiceLocator _gameServiceLocator, ICardSystemEvents _cardSystemEvent,
        ICardSystemFlowActions _cardSystemFlowActions, GameController _gameController,
        UnitLogicSystem _unitLogicSystem, IOrbitPathProvider _orbitPathProvider)
    {
        inputManager = _inputManager;
        waveSystemActions = _waveSystemActions;
        waveSystemEvents = _waveSystemEvents;
        gameServiceLocator = _gameServiceLocator;
        cardSystemEvents = _cardSystemEvent;
        cardSystemFlowActions = _cardSystemFlowActions;
        unitLogicSystemActions = _unitLogicSystem;
        orbitPathProvider = _orbitPathProvider;

        gameRuleEventController = new GameRuleEventController();
    }

    public void OnDestroy()
    {
        Release();

        PlayerSpawnedEvent = null;
        EnemySpawnedEvent = null;
    }

    public void SpawnPlayerAndCharacter()
    {
        SpawnEarth();
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        Character spawnedUnit = Instantiate(characterPrefab, characterSpawnPoint.transform);

        if (spawnedUnit != null)
        {
            spawnedUnit.Initialize_Character(inputManager, orbitPathProvider, gameServiceLocator);
            gameRuleEventController.Bind_Character(spawnedUnit, cardSystemEvents, cardSystemFlowActions);
            characterUnit = spawnedUnit;

            CharacterSpawnedEvent?.Invoke(spawnedUnit);

            SetUnitLogicSystem_Character();
        }
    }

    private void SpawnEarth()
    {
        Earth spawnedUnit = Instantiate(earthPrefab, earthSpawnPoint.transform);

        if (spawnedUnit != null)
        {
            earthUnit = spawnedUnit;

            PlayerSpawnedEvent?.Invoke(earthUnit);

            SetUnitLogicSystem_Earth();
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

            Enemy spawnedUnit = enemyPool.Get();

            if (spawnedUnit != null)
            {
                int randomInt = UnityEngine.Random.Range(0, enemyTypeDataBase.enemyData.Count - 1);

                EnemyTypeData enemyTypeData = enemyTypeDataBase.GetEnemyData(randomInt);

                spawnedUnit.Activate(spawnPosition);
                spawnedUnit.Initialize_Enemy(inputManager, gameServiceLocator, enemyTypeData);
                spawnedUnit.SetTargetPoint(enemyTargetPoint.transform.position);

                gameRuleEventController.Bind_Enemy(spawnedUnit, waveSystemEvents, waveSystemActions);
                enemies.Add(spawnedUnit);
            }
        }

        EnemySpawnedEvent?.Invoke();

        SetUnitLogicSystem_Enemy();
    }

    private void SetUnitLogicSystem_Character()
    {
        unitLogicSystemActions.Initialize(characterUnit);
    }

    private void SetUnitLogicSystem_Enemy()
    {
        unitLogicSystemActions.Initialize(enemies);
    }

    private void SetUnitLogicSystem_Earth()
    {
        unitLogicSystemActions.Initialize(earthUnit);
    }

    public IUnitEvent GetPlayerEventSource()
    {
        return earthUnit;
    }

    public void ResetCurrentEnemies()
    {
        for (int i = 0; i < enemies.Count; ++i)
        {
            enemies[i].DeActivate();
        }
    }

    private void ReleaseAllEnemy()
    {
        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i] != null)//씬이 종료되면 gameObject는 즉시 파괴되므로 접근해서는 안됨.
            {
                gameRuleEventController.Release_Enemy(enemies[i],waveSystemEvents,waveSystemActions);
                enemyPool.Release(enemies[i]);
            }
        }

        enemyPool.Dispose();
    }

    public void Release()
    {
        gameRuleEventController.Release_Character(characterUnit, cardSystemEvents, cardSystemFlowActions);

        ReleaseAllEnemy();
    }
}
