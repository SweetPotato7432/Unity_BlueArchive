using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{// Start is called before the first frame update

    // 스탯 생성
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;
    
    // 현재 상태
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
        // 캐릭터 스탯 구현
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
                    // 공격

                    // 탐색으로 전환
                    if (closestTarget == null)
                    {
                        currentState = States.Move;
                    }
                    break;
                }
            case States.Reload:
                {
                    // 재장전
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

        // 이동
        agent.SetDestination(new Vector3(target.position.x, transform.position.y, transform.position.z));

        // 적 탐지
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
            // 엄폐 가능 확인

            // 엄폐 확인 후 공격
            currentState = States.Attack;
        }
    }
}
