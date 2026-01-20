using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Game/EnemyType")]
public class EnemyTypeDataBase : ScriptableObject
{
    public List<EnemyTypeData> enemyData;

    public EnemyTypeData GetEnemyData(string id)
    {
        return enemyData.Find(x => x.id == id);
    }

    public EnemyTypeData GetEnemyData(int id)
    {
        return enemyData[id];
    }
}