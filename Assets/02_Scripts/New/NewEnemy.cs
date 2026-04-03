using System.Collections;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    [Header("적 정보")]
    private float enemySpeed;
    private float enemyHp;
    private float enemyMaxHp;
    [SerializeField] private RuntimeAnimatorController[] animCon;
    [SerializeField] private int animConIdx;
    [SerializeField] private Rigidbody2D target; //플레이어
    [SerializeField] private bool isLive;
    private Animator anim;
    private SpriteRenderer spriter;
    private Collider2D coll;
    private Rigidbody2D rigid;
    private WaitForFixedUpdate wait;

    [Header("드롭 설정")]
    [SerializeField, Range(0f, 1f)] private float goldChance = 0.10f;
    [SerializeField, Range(0f, 1f)] private float goldBoxChance = 0.009f;
    [SerializeField, Range(0f, 1f)] private float healChance = 0.05f;
    [SerializeField, Range(0f, 1f)] private float magnetChance = 0.03f;
    [SerializeField] private int dropCountMin = 1;
    [SerializeField] private int dropCountMax = 2;

    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockBackDuration = 0.2f;
    [SerializeField] private bool isKnockBack = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        wait = new WaitForFixedUpdate(); // new �� ���ֱ�
    }

    private void Start()
    {
        RefreshTarget();
    }

    void RefreshTarget()
    {
        if (GameFacade.Instance != null && GameFacade.Instance.Player != null)
        {
            target = GameFacade.Instance.Player.GetComponent<Rigidbody2D>();
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void LateUpdate()
    {
        Flip();
    }

    private void OnEnable()
    {
        InitSetting();
        RefreshTarget();

       /* if(anim != null)
        {
            anim.ResetTrigger("Hit");
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("CatAttack") || !isLive) return;

        enemyHp -= collision.GetComponent<CatAtack>().damage;

        if(isLive && enemyHp > 0)
        {
            StartCoroutine(KnockBack()); //�˹�
        }
        if(enemyHp > 0)
        {
            anim.SetTrigger("Hit");            
            GameFacade.Instance.PlaySfx(Sfx.Hit);
        }
        else
        {
            Dead();
        }
    }

    IEnumerator KnockBack()
    {
        isKnockBack = true;

        yield return wait;

        if(!isLive || !GameFacade.Instance.IsLive)
        {
            isKnockBack =false;
            yield break;    
        }

        Vector3 playerPos = GameFacade.Instance.Player.transform.position;
        Vector3 knockDir = transform.position - playerPos;

        rigid.AddForce(knockDir.normalized * knockbackForce, ForceMode2D.Impulse);

        float elapsed = 0f;
        while(elapsed < knockBackDuration && isLive && GameFacade.Instance.IsLive)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if(rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
        }

        isKnockBack =false;

    }
    public void Init(NewSpawnData data)
    {
      
        animConIdx = data.animConIdx;
        enemySpeed = data.enemySpeed;
        enemyMaxHp = data.enemyHp;
        enemyHp = data.enemyHp;

        if (animCon != null && data.animConIdx >= 0 && data.animConIdx < animCon.Length)
        {
            anim.runtimeAnimatorController = animCon[animConIdx];
        }

    }

    public void OnHit(float damage)
    {
        if (!isLive) return;

        enemyHp -= damage;

        if(enemyHp <= 0)
        {
            Dead();
        }
    }
    private void Dead()
    {
        if (!isLive) return;

        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);
        GameFacade.Instance.AddKill();

        if(isKnockBack)
        {
            StopCoroutine(KnockBack());
            isKnockBack = false;
        }

        if(rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
        }

        if (GameFacade.Instance.IsLive)
            GameFacade.Instance.PlaySfx(Sfx.Dead);

        DropItems();
        GameFacade.Instance.Return<EnemyEnumId>(EnemyEnumId.EnemyBird, this.gameObject);
    }

    private void DropItems()
    {
        int count = Random.Range(dropCountMin, dropCountMax + 1);
        for (int i = 0; i < count; i++)
        {
            bool gold = Random.value < goldChance;
            var coinKey = gold ? ItemEnumId.Exp_2 : ItemEnumId.Exp_1;
            int val = gold ? 5 : 1;
            var coin = GameFacade.Instance.Get(coinKey);
            if (coin == null) continue;
            coin.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.4f;
            coin.GetComponent<ExpItem>()?.Init(val);
        }

        if (Random.value < goldBoxChance)
        {
            var box = GameFacade.Instance.Get(ItemEnumId.GoldBox);
            box.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
            box.GetComponent<ExpItem>()?.Init(20);
        }

        if (Random.value < healChance)
        {
            var heal = GameFacade.Instance.Get(ItemEnumId.Chur);
            heal.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
        }

        if (Random.value < magnetChance)
        {
            var mag = GameFacade.Instance.Get(ItemEnumId.Mag);
            mag.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
        }
    }
    private void InitSetting()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 3;
        anim.SetBool("Dead", false);
        enemyHp = enemyMaxHp;
        isKnockBack = false;
    }

    private void Move()
    {
        if (!GameFacade.Instance.IsLive) return;
        if (!isLive) return;

        if (target == null)
        {
            RefreshTarget();
            if (target == null) return;
        }

        if (isKnockBack || (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")))
            return;

        Vector2 dir = target.position - rigid.position;
        Vector2 nextVect = dir.normalized * enemySpeed * Time.deltaTime;

        rigid.MovePosition(rigid.position + nextVect); 
        rigid.linearVelocity = Vector2.zero; 

    }

    void Flip()
    {
        if(!GameFacade.Instance.IsLive || !isLive) return;

        spriter.flipX = target.position.x > rigid.position.x; 
    }
}
