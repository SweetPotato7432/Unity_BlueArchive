using System.Collections;
using System.Collections.Generic;
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

    private NavMeshAgent agent;

    [SerializeField]
    private Transform destination;

    // ���� ����� ��ǥ
    [SerializeField]
    private GameObject closestTarget;
    // ���� ��ǥ���� �Ÿ�
    [SerializeField]
    private float targetDistance;
    // ���� ��ǥ�� Character ��ũ��Ʈ ������Ʈ
    private Character targetCharacter;

    private LayerMask targetLayer;

    private Collider[] cols;

    // ��Ÿ�
    private float range;

    // ���� ��Ÿ��
    private float attackCooltime = 0f;

    // Ÿ�� �±�
    private string targetTag;

    


    private void Awake()
    {
        // ĳ���� ���� ����
        stat = new Stat();
        stat = stat.setUnitStat(unitCode);

        

        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentState = States.Move;
        targetLayer = 1 << LayerMask.NameToLayer("Character");
        range = stat.Range * 0.025f;

        if(this.tag == "Enemy")
        {
            targetTag = "Ally";
        }
        else if(this.tag == "Ally")
        {
            targetTag = "Enemy";
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
        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
        targetDistance = Mathf.Infinity;
        closestTarget = null;

        // �̵�
        agent.SetDestination(new Vector3(destination.position.x, transform.position.y, transform.position.z));


        cols = Physics.OverlapSphere(transform.position, range, targetLayer);
        foreach (Collider col in cols)
        {
            if(col.tag == targetTag)
            {
                float distance = Vector3.Distance(col.transform.position, transform.position);
                if (distance < targetDistance)
                {
                    targetDistance = distance;
                    closestTarget = col.gameObject;
                    targetCharacter = closestTarget.GetComponent<Character>();
                }
            }
            
        }

        if (closestTarget != null && targetDistance <= range)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }
            // ���� ���� Ȯ��

            // ���� Ȯ�� �� ����
            if (stat.CurMag <= 0)
            {
                currentState = States.Reload;
            }
            else
            {
                currentState = States.Attack;
            }
        }
    }

    private void Attack()
    {
        attackCooltime += Time.deltaTime;
        if (attackCooltime >= stat.AttackCoolTime)
        {

            attackCooltime -= stat.AttackCoolTime;
            // ź�� ����
            stat.CurMag--;
            targetCharacter.TakeDamage(stat);
            //Debug.Log(stat.Name + "�� " + stat.Atk + "�� ������, ���� ��ź�� : " + stat.CurMag);
            // ������ �����

        }

        // Ž������ ��ȯ
        if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
        {
            //attackCooltime = 0f;
            currentState = States.Move;
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
            Debug.Log(stat.Name+" "+stat.CurHp);
            Debug.Log($"{attakerStat.Name}�� {stat.Name}���� ���� ���� ����� {damage}");

        }
        else
        {
            // ȸ��
            Debug.Log($"{stat.Name}�� {attakerStat.Name}�� ���ݿ� ȸ��");
        }
        
        

        
    }

    private void Reload()
    {
        stat.CurMag = stat.MaxMag;

    }

}
