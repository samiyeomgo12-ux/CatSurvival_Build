
using UnityEngine;

public class NewSpawner : MonoBehaviour
{
    private Transform[] spawnPoint;
    [SerializeField] private NewSpawnData[] spawnData;
    [SerializeField] private EnemyEnumId basicEnemyKey = EnemyEnumId.EnemyBird;
    [SerializeField] private float levelTime;
    private float timer;
    private int level;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameFacade.Instance.MaxGameTime[GameFacade.Instance.CurrentMaxGameTimeIdx] / spawnData.Length;
        Spawn();
    }

    private void Update()
    {
        if (!GameFacade.Instance.IsLive)
            return;

        timer += Time.deltaTime;

        level = Mathf.Min(Mathf.FloorToInt(GameFacade.Instance.GameTime / levelTime), spawnData.Length - 1);

        if(timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn(); 
        }
    }

    private void Spawn()
    {

        int randomLevel = Random.Range(0, level + 1);

        NewSpawnData data = spawnData[randomLevel];

        GameObject enemy = GameFacade.Instance.Get<EnemyEnumId>(basicEnemyKey);

        if(enemy != null)
        {
            enemy.GetComponent<NewEnemy>().Init(data); //현재 레벨 이하 몬스터 등장
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        }
        else
        {
            Debug.Log("에너미 스포너에서, 에너미를 가져오지 못했음");
        }



    }
}
