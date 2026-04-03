using UnityEngine;

public class CatAtack : MonoBehaviour
{
    public float damage;
    public int per;
    public int speed;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0)
        {
            rigid.linearVelocity = dir * speed; //�Ѿ��� ���ư��� �ӵ� 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100) // per -100관통형이 아님 
            return;

        per--;

        if (per < 0)
        {
            if(rigid != null) 
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100) //플레이어가 가지고 있는 영역이 아니고, 근접무기일 떄
            return; //나가라

        gameObject.SetActive(false); //원거리 무기일 때는 영역 밖으로 나가면 비활성화 합시다 
     }
}
