using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnData", menuName = "Scriptable Objects/NewSpawnData")]
public class NewSpawnData : ScriptableObject
{
    public EnemyEnumId enemyKey;
    public int animConIdx;
    public float spawnTime;
    public float enemyHp;
    public float enemySpeed;
}
