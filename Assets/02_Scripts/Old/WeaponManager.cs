/*using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count =2 ; //무기 개수
    public float speed; //회전속도

    float timer;
    Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }
 
    void Update()
    {
        if(!GameManager.instance.isLive)
            return;

           switch (id)
           {
               case 0:
                   transform.Rotate(Vector3.back * speed * Time.deltaTime);
                   break;
               default:
                   timer += Time.deltaTime;
                   if(timer > speed) 
                   {
                       timer = 0f;
                       Fire();
                   }
                   break;
           }

        //Test Code
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10f, 1); //데미지, 늘어나는 숫자 
        }

    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("AppleyGear", SendMessageOptions.DontRequireReceiver);
        //player 오브젝트와 그 밑에 붙어있는 모든 자식오브젝트에 ApplyGear 이름의 함수 실행
    }
    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for(int index=0; index<GameManager.instance.pool.prefebs.Length; index++)
        {
            if(data.projecttile == GameManager.instance.pool.prefebs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0: //CatPaw
                speed = 150* Character.WeaponSpeed;
                Batch();
                break;

          *//*  case 1:
                speed = -150; //CatScratch
                Batch1();
                break;*//*

            default:
                speed = 0.3f * Character.WeaponRate;
                break;
        }

        //CatAcc

       *//* CatAcc catAccs = player.catAccs[(int)data.itemType];
        catAccs.spriter.sprite = data.catFire;
        catAccs.gameObject.SetActive(true);*//*

        player.BroadcastMessage("AppleyGear", SendMessageOptions.DontRequireReceiver);
    }

    
    //CatPaw
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            *//*Transform catPaw = GameManager.instance.pool.Get(prefabId).transform;
            catPaw.parent = transform;*//*

            Transform catAttack;

            if (index < transform.childCount)
            {
                catAttack = transform.GetChild(index);
            }
            else
            {
                catAttack = GameManager.instance.pool.Get(prefabId).transform;
            }

            catAttack.parent = transform;

            catAttack.localPosition = Vector3.zero; // 초기화 
            catAttack.localRotation = Quaternion.identity; //초기화 


            //1. 무기 각도 나눠서 배치할 위치 구하기 
            float angle = 360f * index / count;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

            //2. 월드 좌표 기준 위치 배치 
            catAttack.localPosition = dir * 1.5f; //플레이어로부터 1.5f 거리

            //3. 무기 방향 설정(무기가 바깥쪽을 바라보게)
            catAttack.localRotation = Quaternion.Euler(0, 0, angle);

            catAttack.GetComponent<CatAtack>().Init(damage, -100, Vector3.zero); //-1 is Infinite Per.

            *//*Vector3 rotVec = Vector3.forward * 360 * index / count;
            catPaw.Rotate(rotVec);
            catPaw.Translate(catPaw.up * 1.0f, Space.World); //플레이어 위, 거리
            catPaw.GetComponent<CatPaw>().Init(damage, -1); //-1 is Infinity Per.*//*
        } 
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized; //정규화 

        float offsetDistance = 1f; //불화살이 나에게서 얼마나 멀리 떨어져서 날아갈지 

        Vector3 spawnPos = transform.position + dir * offsetDistance;

        Transform fireArrow = GameManager.instance.pool.Get(prefabId).transform;
        fireArrow.position = spawnPos;
        fireArrow.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        fireArrow.GetComponent<CatAtack>().Init(damage, count, dir);


        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
    //CatScratch
    *//*void Batch1()
    {

        for (int index = 0; index < count; index++)
        {

            Debug.Log($"Batch1 index: {index}");

            Transform catScratch = GameManager.instance.pool.Get(prefabId).transform;
            catScratch.parent = transform;

            //1. 무기 각도 나눠서 배치할 위치 구하기 
            float angle = 360f * index / count;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

            //2. 월드 좌표 기준 위치 배치 
            catScratch.localPosition = dir * 1.5f; //플레이어로부터 1.5f 거리

            //3. 무기 방향 설정(무기가 바깥쪽을 바라보게)
            catScratch.localRotation = Quaternion.Euler(0, 0, angle);

            catScratch.GetComponent<CatScratch>().Init(damage, -1); //-1 is Infinite Per.
        }
    }*//*
}
*/