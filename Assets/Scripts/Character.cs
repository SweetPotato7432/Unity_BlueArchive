using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{// Start is called before the first frame update

    // ���� ����
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;
    
    // ���� ����
    enum States
    {
        None,
        Move,
        Cover,
        Attack,
        Reload
    };

    [SerializeField]
    private States currentState;

    private NavMeshAgent nav;

    private Transform allyDestination;
    private Transform enemyDestination; 

    [SerializeField]
    private Vector3 destination;

    // ���� ����� ��ǥ
    [SerializeField]
    private GameObject closestTarget;
    // ���� ��ǥ���� �Ÿ�
    [SerializeField]
    private float targetDistance;
    // ���� ��ǥ�� Character ��ũ��Ʈ ������Ʈ
    private Character targetCharacter;

    private LayerMask targetLayer;

    [SerializeField]
    private LayerMask coverLayer;

    // ��Ÿ�
    private float range;

    // ���� ��Ÿ��
    private float calCooltime = 0f;

    // Ÿ�� �±�
    private string targetTag;

    public Collider[] coltest;
    private void Awake()
    {
        // ĳ���� ���� ����
        stat = new Stat();
        stat = stat.setUnitStat(unitCode);
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        currentState = States.Move;
        targetLayer = LayerMask.GetMask("Character");
        coverLayer = LayerMask.GetMask("Cover");
        range = stat.Range * 0.035f;

        allyDestination = GameObject.Find("AllyDestination").transform;
        enemyDestination = GameObject.Find("EnemyDestination").transform;

        if(this.tag == "Enemy")
        {
            // ��ǥ ����
            targetTag = "Ally";

            // ������ ����
            destination = new Vector3(enemyDestination.position.x, transform.position.y,transform.position.z);
        }
        else if(this.tag == "Ally")
        {
            // ��ǥ ����
            targetTag = "Enemy";

            // ������ ����
            destination = new Vector3(allyDestination.position.x, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stat.CurHp <= 0)
        {
            Destroy(gameObject);
        }

        switch (currentState)
        {
            case States.Move:
                {
                    // �̵� �� �� Ž��
                    Move();
                    break;
                }
            case States.Cover:
                {
                    // ����õ�
                    Cover();

                    break;
                }
            case States.Attack:
                {
                    // ����
                    Attack();
                    
                    break;
                }
            case States.Reload:
                {
                    // ������
                    Reload();
                    break;
                }
        }

        if (closestTarget != null)
        {
            targetDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
        }
    }

    private void Move()
    {
        
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }
        

        UpdateTarget(25);
        
        if (closestTarget != null)
        {
            if ( targetDistance <= range * 0.8f)
            {
                if (!nav.isStopped)
                {
                    nav.isStopped = true;
                    nav.velocity = Vector3.zero;
                }
                // ���� ���� Ȯ��

                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // ���� Ȯ�� �� ����
                else if (stat.CurMag <= 0)
                {
                    currentState = States.Reload;
                }
                else
                {
                    currentState = States.Attack;
                }
            }
            else
            {
                nav.SetDestination(closestTarget.transform.position);
            }
        }
        else
        {
            nav.SetDestination(destination);
        }
    }

    private void Attack()
    {
        if(closestTarget == null || targetDistance > range)
        {
            UpdateTarget(range);
            if (closestTarget == null)
            {
                calCooltime = 0f;
                currentState = States.Move;
                return;
            }
        }
        
        calCooltime += Time.deltaTime;
        if (calCooltime >= stat.AttackCoolTime && targetDistance<=range)
        {

            calCooltime -= stat.AttackCoolTime;
            // ź�� ����
            stat.CurMag--;
            targetCharacter.TakeDamage(stat);
            //Debug.Log(stat.Name + "�� " + stat.Atk + "�� ������, ���� ��ź�� : " + stat.CurMag);
            
            if(closestTarget == null || targetDistance>range|| stat.CurMag <= 0)
            {
                calCooltime = 0f;
                currentState = States.Move;
            }

        }
    }

    public void TakeDamage(Stat attakerStat)
    {
        bool isCritical;
        // ���� üũ
        int calDodge = stat.Dodge - attakerStat.AccuracyRate;
        if (calDodge < 0)
        {
            calDodge = 0;
        }
        float dodgeCheck = 2000f / (calDodge * 3f + 2000f);

        
        if ( Random.Range(0f,1f) <= dodgeCheck)
        {
            //����

            // ġ��Ÿ üũ
            int calCriticalRate = attakerStat.CriticalRate - 100;
            if(calCriticalRate < 0)
            {
                calCriticalRate = 0;
            }
            float criticalCheck = (calCriticalRate * 6000f) / (calCriticalRate * 6000f + 4000000f);

            if (Random.Range(0f, 1f) <= criticalCheck)
            {
                //Debug.Log($"{stat.Name}�� {attakerStat.Name}�� ġ��Ÿ ������ ������");
                isCritical = true;
            }
            else
            {
                //Debug.Log($"{stat.Name}�� {attakerStat.Name}�� �Ϲ� ������ ������");
                isCritical = false;
            }

            // ���� ����� ���
            // ������ ���� ���
            float damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));

            float damage = attakerStat.Atk*damageRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
            //Debug.Log(stat.Name+" "+stat.CurHp);
            //Debug.Log($"{attakerStat.Name}�� {stat.Name}���� ���� ���� ����� {damage}");

        }
        else
        {
            // ȸ��
            //Debug.Log($"{stat.Name}�� {attakerStat.Name}�� ���ݿ� ȸ��");
        }
        
        

        
    }

    private void Reload()
    {
        
        calCooltime += Time.deltaTime;
        Debug.Log($"{stat.Name} ������ ��...");
        if( calCooltime >= stat.ReloadTime)
        {
            stat.CurMag = stat.MaxMag;
            calCooltime = 0f;
            currentState = States.Attack;
            Debug.Log($"{stat.Name} ������ �Ϸ�!");
        }

    }

    private void Cover()
    {
        // 13��ŭ�� �Ÿ��� ��ֹ� Ž��
        Collider[] cols = Physics.OverlapSphere(transform.position, 13, coverLayer);

        List<GameObject> covers = new List<GameObject>();

        foreach (Collider col in cols)
        {
            Debug.Log("Detected cover object: " + col.gameObject.name);
            CoverObject cover = col.GetComponent<CoverObject>();

            if (cover != null && !cover.isOccupied)
            {
                covers.Add(col.gameObject);
                Debug.Log("Detected cover object: " + col.gameObject.name);
            }
        }
        // �Ÿ������� ����
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        if(covers.Count > 0 )
        {
            //Debug.Log($"{this.gameObject.name}���� ����� ����{covers[0].gameObject.name}");
            
            foreach(GameObject tryCoverObject in covers)
            {
                // ��ֹ����� ���� ����� ���� �Ÿ��� ��Ÿ����� ª���� Ȯ��
            }
        }
    }

    private void UpdateTarget(float range)
    {
        targetDistance = Mathf.Infinity;
        closestTarget = null;
        GameObject closestEnemy = null;

        // �̵�

        //25��ŭ�� �Ÿ��� �� Ž��
        Collider[] cols = Physics.OverlapSphere(transform.position, range, targetLayer);
        foreach (Collider col in cols)
        {
            if (col.tag == targetTag)
            {
                float distance = Vector3.Distance(col.transform.position, transform.position);
                if (distance < targetDistance)
                {
                    targetDistance = distance;
                    closestEnemy = col.gameObject;
                    //targetCharacter = closestTarget.GetComponent<Character>();
                }
            }
        }
        if (closestEnemy != closestTarget)
        {
            closestTarget = closestEnemy;
            targetCharacter = closestEnemy != null ? closestTarget.GetComponent<Character>() : null;
        }


    }
}
