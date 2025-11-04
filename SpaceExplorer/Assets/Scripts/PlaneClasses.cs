using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaneColorOption
{
    public string colorName;
    public string hexCode;
    public Sprite spriteName;
}

[System.Serializable]
public class PlaneData
{
    public string planeName;
    public float speed;
    public float fireRate;
    public Sprite selectedSprite;
    public List<PlaneColorOption> colorOptions = new();
    public bool isUnlocked;
    public int price;
    public GameObject planePrefab;
}

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public float enemySpeed;
    public float enemyFireRate;
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    // public EnemyMovementPattern movementPattern; // For path following
}

public enum EnemyType
{
    Boss,
    FootSoldier,
}

public enum EnemyMovementPattern
{
    ArcPath,
    GridPattern,
}

[CreateAssetMenu(fileName = "PlaneDatabase", menuName = "Game Data/Plane Database")]
public class PlaneDatabase : ScriptableObject
{
    public List<PlaneData> allPlanes;

    public List<EnemyData> enemyPlanes;
}
