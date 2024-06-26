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

    // 가장 가까운 목표
    [SerializeField]
    private GameObject closestTarget;
    // 현재 목표와의 거리
    [SerializeField]
    private float targetDistance;
    // 현재 목표의 Character 스크립트 컴포넌트
    private Character targetCharacter;

    private LayerMask targetLayer;

    private Collider[] cols;

    // 사거리
    private float range;

    // 공격 쿨타임
    private float attackCooltime = 0f;

    // 타겟 태그
    private string targetTag;

    float damageRatio;

    private void Awake()
    {
        // 캐릭터 스탯 구현
        stat = new Stat();
        stat = stat.setUnitStat(unitCode);

        // 데미지 비율 미리 계산
        damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));

        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentState = States.Move;
        targetLayer = LayerMask.NameToLayer("Character");
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
                    // 이동 및 적 탐색
                    Move();
                    break;
                }
            case States.Cover:
                {
                    // 엄폐시도
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

        // 이동
        agent.SetDestination(new Vector3(destination.position.x, transform.position.y, transform.position.z));

        // 적 탐지
        int layerMask = (1 << targetLayer);

        cols = Physics.OverlapSphere(transform.position, range, layerMask);
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
        if(targetDistance > range)
        {
            closestTarget = null;
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
            
            attackCooltime -= stat.AttackCoolTime;
            // 탄약 제외
            stat.CurMag--;
            targetCharacter.TakeDamage(stat);
            //Debug.Log(stat.Name + "이 " + stat.Atk + "의 데미지, 남은 장탄수 : " + stat.CurMag);
            // 적에게 대미지

        }

        // 탐색으로 전환
        if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
        {
            attackCooltime = 0f;
            currentState = States.Move;
        }
    }

    public void TakeDamage(Stat attakerStat)
    {
        bool isCritical;
        // 적중 체크
        int calDodge = stat.Dodge - attakerStat.AccuracyRate;
        if (calDodge < 0)
        {
            calDodge = 0;
        }
        float dodgeCheck = 2000f / (calDodge * 3f + 2000f);

        
        if ( Random.Range(0f,1f) <= dodgeCheck)
        {
            //적중

            // 치명타 체크
            int calCriticalRate = attakerStat.CriticalRate - 100;
            if(calCriticalRate < 0)
            {
                calCriticalRate = 0;
            }
            float criticalCheck = (calCriticalRate * 6000f) / (calCriticalRate * 6000f + 4000000f);

            if (Random.Range(0f, 1f) <= criticalCheck)
            {
                //Debug.Log($"{stat.Name}가 {attakerStat.Name}의 치명타 공격을 적중함");
                isCritical = true;
            }
            else
            {
                //Debug.Log($"{stat.Name}가 {attakerStat.Name}의 일반 공격을 적중함");
                isCritical = false;
            }

            // 최종 대미지 계산
            
            float damage = attakerStat.Atk*damageRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
            Debug.Log(stat.Name+" "+stat.CurHp);
            Debug.Log($"{attakerStat.Name}이 {stat.Name}에게 입힌 최종 대미지 {damage}");

        }
        else
        {
            // 회피
            Debug.Log($"{stat.Name}가 {attakerStat.Name}의 공격에 회피");
        }
        
        

        
    }
}
