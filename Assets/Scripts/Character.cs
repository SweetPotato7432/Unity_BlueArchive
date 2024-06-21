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
    private Transform destination;

    [SerializeField]
    private GameObject closestTarget;
    [SerializeField]
    private float closestDistance;

    private LayerMask targetLayer;

    private Collider[] cols;

    // 사거리
    private float range;

    // 공격 쿨타임
    private float attackCooltime = 0f;

    private void Awake()
    {
        // 캐릭터 스탯 구현
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
                    // 공격
                    Attack();
                    
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
        agent.SetDestination(new Vector3(destination.position.x, transform.position.y, transform.position.z));

        // 적 탐지
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
            // 엄폐 가능 확인

            // 엄폐 확인 후 공격
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
            // 적에게 대미지
            
            attackCooltime -= stat.AttackCoolTime;
            // 탄약 제외
            stat.CurMag--;
            Debug.Log(stat.Name + "이 " + stat.Atk + "의 데미지, 남은 장탄수 : " + stat.CurMag);

        }

        // 탐색으로 전환
        if (closestTarget == null || closestDistance > range || stat.CurMag <= 0)
        {
            attackCooltime = 0f;
            currentState = States.Move;
        }
    }
}
