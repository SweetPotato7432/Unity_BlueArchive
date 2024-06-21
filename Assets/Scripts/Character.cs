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

    [SerializeField]
    private GameObject closestTarget;
    [SerializeField]
    private float closestDistance;

    private LayerMask targetLayer;

    private Collider[] cols;

    // ��Ÿ�
    private float range;

    // ���� ��Ÿ��
    private float attackCooltime = 0f;

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
        targetLayer = LayerMask.NameToLayer("Character");
        range = stat.Range * 0.025f;
    }

    // Update is called once per frame
    void Update()
    {
        if (closestTarget != null)
        {
            closestDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
        }
        switch (currentState)
        {
            case States.Move:
                {
                    Move();
                    break;
                }
            case States.Cover:
                {
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
    }

    private void Move()
    {
        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
        closestDistance = Mathf.Infinity;
        closestTarget = null;

        // �̵�
        agent.SetDestination(new Vector3(destination.position.x, transform.position.y, transform.position.z));

        // �� Ž��
        int layerMask = (1 << targetLayer);

        cols = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach (Collider col in cols)
        {
            if(col.tag == "Enemy")
            {
                float distance = Vector3.Distance(col.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = col.gameObject;
                }
            }
            
        }
        if (closestTarget != null)
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
        if(attackCooltime >= stat.AttackCoolTime)
        {
            // ������ �����
            
            attackCooltime -= stat.AttackCoolTime;
            // ź�� ����
            stat.CurMag--;
            Debug.Log(stat.Name + "�� " + stat.Atk + "�� ������, ���� ��ź�� : " + stat.CurMag);

        }

        // Ž������ ��ȯ
        if (closestTarget == null || closestDistance > range || stat.CurMag <= 0)
        {
            attackCooltime = 0f;
            currentState = States.Move;
        }
    }
}
