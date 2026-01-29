using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using WaveSystemSignals;

public class UnitSpawner : MonoBehaviour,IUnitSpawnSystemData
{
    public event Action<Character> CharacterCreatedEvent;
    public event Action<Earth> PlayerCreatedEvent;
    public event Action<List<Enemy>> EnemyCreatedEvent;

    [Header("Enemy Pool Settings")]
    [SerializeField] const int enemyMaxCount = 40;

    [Header("Unit Prefabs")]
    private GameObject unitPrefab;
    [SerializeField] private Character characterPrefab;
    [SerializeField] private Earth playerPrefab;
    [SerializeField] private Enemy enemyUnitPrefab;
    [SerializeField] private GameObject characterSpawnPoint;
    [SerializeField] private GameObject playerSpawnPoint;

    //외부 의존성
    private InputManager inputManager;
    private GameServiceLocator gameServiceLocator;
    private IOrbitPathProvider orbitPathProvider;


    private int curUnitCnt;

    public Character characterUnit { get; private set; }
    public Earth playerUnit { get; private set; }

    public ICharacterData characterData => characterUnit;

    public IPlayerData playerData => playerUnit;

    public List<IEnemyData> enemiesData => enemiesData;

    [Header("Wave Spawn Settings")]
    [SerializeField] private GameObject waveSpawnPoint;
    public float radiusX = 7f;        // 타원의 가로 반지름
    public float radiusY = 3f;        // 타원의 세로 반지름
    [SerializeField] EnemyTypeDataBase enemyTypeDataBase;

    [Header("Enemy Target Point")]
    [SerializeField] private GameObject enemyTargetPoint;


    private List<Enemy> enemies = new List<Enemy>(40);
    private List<IEnemyData> enemyData = new List<IEnemyData>(40);

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
        enemy.ActivateEnemy();
    }

    private void OnReleaseEnemy(Enemy enemy)
    {

    }

    private void OnDestroyEnemy(Enemy enemy)
    {
        if (enemy != null)
            Destroy(enemy.gameObject);
    }

    public void Initiallize(InputManager _inputManager,
        GameServiceLocator _gameServiceLocator,
        IOrbitPathProvider _orbitPathProvider)
    {
        inputManager = _inputManager;
        gameServiceLocator = _gameServiceLocator;
        orbitPathProvider = _orbitPathProvider;

        SpawnPlayerAndCharacter();
    }

    public void OnDestroy()
    {
        Release();
    }

    public void SpawnPlayerAndCharacter()
    {
        SpawnPlayer();
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        Character spawnedUnit = Instantiate(characterPrefab, characterSpawnPoint.transform);

        if (spawnedUnit != null)
        {
            spawnedUnit.Initialize_Character(inputManager, orbitPathProvider, gameServiceLocator);
            characterUnit = spawnedUnit;

            CharacterCreatedEvent?.Invoke(characterUnit);

            spawnedUnit.gameObject.SetActive(false);
        }
    }

    private void SpawnPlayer()
    {
        Earth spawnedUnit = Instantiate(playerPrefab, playerSpawnPoint.transform);

        if (spawnedUnit != null)
        {
            playerUnit = spawnedUnit;

            PlayerCreatedEvent?.Invoke(playerUnit);

            spawnedUnit.gameObject.SetActive(false);
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

    public void SpawnWave(SpawnWaveSignal spawnWaveSignal)
    {
        curUnitCnt = spawnWaveSignal.waveIdx;

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

                enemyData.Add(spawnedUnit);
                enemies.Add(spawnedUnit);
            }
        }

        EnemyCreatedEvent?.Invoke(enemies);
    }

    public void ResetCurrentEnemies(AllEnemyDeadSignal allEnemyDeadSignal)
    {
        for (int i = 0; i < enemies.Count; ++i)
        {
            enemyPool.Release(enemies[i]);
        }
    }

    private void ReleaseAllEnemy()
    {
        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i] != null)//씬이 종료되면 gameObject는 즉시 파괴되므로 접근해서는 안됨.
            {
                enemyPool.Release(enemies[i]);
            }
        }

        enemyPool.Dispose();
    }

    public void Release()
    {
        ReleaseAllEnemy();
    }
}
