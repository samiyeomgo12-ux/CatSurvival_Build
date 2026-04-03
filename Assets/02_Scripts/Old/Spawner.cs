/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime; //레벨 변경 시간 
    float timer;
    int level;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length; //최대 시간에 몬스터 마리수 데이터 크기로 나눠서 자동 구간 시간 계산 
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        //FloorToInt
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length-1); // 0�� 1�� ���� 10f->10�ʸ��� �ٸ� ����
        //level = 7;
        *//*Debug.Log(level);*/

        /*Debug.Log(GameManager.instance.gameTime);*//*

        if(timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
     
    }

    void Spawn()
    {
       GameObject enemy = GameManager.instance.pool.Get(0);
       enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
       
       int randomLevel = Random.Range(0, level + 1); //���� 0 ~ ���� ���� ���̿��� �������� ���� ��ȯ 
        
       enemy.GetComponent<Enemy>().Init(spawnData[randomLevel]);
    }
}

[System.Serializable] //����ȭ 
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int enemyHP;
    public float enemySpeed;
}*/