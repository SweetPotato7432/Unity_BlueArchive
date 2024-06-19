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
    private Transform target;

    [SerializeField]
    private GameObject closestTarget;
    [SerializeField]
    private float closestDistance;

    private LayerMask targetLayer;

    public Collider[] cols;

    private void Awake()
    {
        // ĳ���� ���� ����
        stat = new Stat();
        stat.setUnitStat(unitCode);


        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {

        currentState = States.Move;
        targetLayer = LayerMask.NameToLayer("Character");
    }

    // Update is called once per frame
    void Update()
    {
        
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

                    // Ž������ ��ȯ
                    if (closestTarget == null)
                    {
                        currentState = States.Move;
                    }
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
        agent.SetDestination(new Vector3(target.position.x, transform.position.y, transform.position.z));

        // �� Ž��
        int layerMask = (1 << targetLayer);

        cols = Physics.OverlapSphere(transform.position, 15, layerMask);
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
            currentState = States.Attack;
        }
    }
}
