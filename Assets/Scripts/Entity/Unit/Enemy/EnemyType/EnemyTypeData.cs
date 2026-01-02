using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class EnemyTypeData
{
    public string id;
    public Sprite sprite;
    public float scale;
    public float moveForce;
    public float health;
    public float attack;
    public float shield;
}