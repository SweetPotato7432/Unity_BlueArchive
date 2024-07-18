using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
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
        Reload,
        Dead
    };

    [SerializeField]
    private States currentState;

    private NavMeshAgent nav;

    private Transform allyDestination;
    private Transform enemyDestination;

    [SerializeField]
    private Vector3 destination;

    // 가장 가까운 목표
    [SerializeField]
    private GameObject closestTarget;
    // 현재 목표와의 거리
    [SerializeField]
    private float targetDistance;
    // 현재 목표의 Character 스크립트 컴포넌트
    private Character targetCharacter;

    private LayerMask targetLayer;
    private LayerMask coverLayer;

    // 엄폐를 시도하는 목표물
    [SerializeField]
    private CoverObject currentCoverObject;

    [SerializeField]
    private Vector3 coverPosition;

    // 사거리
    private float range;

    // 타겟 태그
    private string targetTag;

    [SerializeField]
    bool test;

    private void Awake()
    {
        // 캐릭터 스탯 구현
        stat = new Stat();
        stat = stat.setUnitStat(unitCode);
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        currentState = States.Move;
        targetLayer = LayerMask.GetMask("Character");
        coverLayer = LayerMask.GetMask("Cover");
        range = stat.Range;

        allyDestination = GameObject.Find("AllyDestination").transform;
        enemyDestination = GameObject.Find("EnemyDestination").transform;

        if (this.tag == "Enemy")
        {
            // 목표 설정
            targetTag = "Ally";

            // 목적지 설정
            destination = new Vector3(enemyDestination.position.x, transform.position.y, transform.position.z);
        }
        else if (this.tag == "Ally")
        {
            // 목표 설정
            targetTag = "Enemy";

            // 목적지 설정
            destination = new Vector3(allyDestination.position.x, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        test = stat.IsCover;
        if (stat.CurHp <= 0)
        {
            currentState = States.Dead;
            CleanupAndDestroy();
        }

        if (currentCoverObject != null && !currentCoverObject.CheckUser(gameObject))
        {
            currentCoverObject = null;
            nav.SetDestination(destination);
            return;
        }

        // 엄폐 위치를 벗어났는지 확인
        if (stat.IsCover && coverPosition != transform.position)
        {
            if (!nav.hasPath || nav.velocity.sqrMagnitude != 0f)
            {
                LeaveCover();
            }
        }

        switch (currentState)
        {
            case States.Move:
                Move();
                break;
            case States.Cover:
                // 엄폐 시도
                if (currentCoverObject == null)
                {
                    TryCover();
                }
                else if (nav.pathPending == false && nav.remainingDistance <= nav.stoppingDistance)
                {
                    if (nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                    {
                        OnReachCover();
                    }
                }
                break;
            case States.Attack:
                Attack();
                break;
            case States.Reload:
                Reload();
                break;
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
            if (targetDistance <= range * 0.8f)
            {
                // 엄폐 가능 확인
                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // 엄폐 확인 후 공격
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

   /* private void Attack()
    {
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        if (closestTarget == null || targetDistance > range)
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
        if (calCooltime >= stat.AttackCoolTime && targetDistance <= range)
        {
            calCooltime -= stat.AttackCoolTime;
            // 탄약 제외
            stat.CurMag--;
            transform.LookAt(targetCharacter.gameObject.transform);
            targetCharacter.TakeDamage(stat);

            if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
            {
                calCooltime = 0f;
                currentState = States.Move;
            }
        }

    }*/
    private void Attack()
    {
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }

        if (closestTarget == null || targetDistance > range)
        {
            UpdateTarget(range);
            if (closestTarget == null)
            {
                CancelInvoke("InvokeAttack");
                currentState = States.Move;
                return;
            }
        }

        if (!IsInvoking("InvokeAttack"))
        {
            InvokeRepeating("InvokeAttack", stat.AttackCoolTime, stat.AttackCoolTime);
        }
    }

    private void InvokeAttack()
    {
        if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
        {
            CancelInvoke("PerformAttack");
            currentState = States.Move;
            return;
        }
        Debug.Log($"{stat.Name} : Attack");
        // 탄약 제외
        stat.CurMag--;
        transform.LookAt(targetCharacter.gameObject.transform);
        targetCharacter.TakeDamage(stat);

        if (stat.CurMag <= 0)
        {
            currentState = States.Reload;
        }
    }


    public void TakeDamage(Stat attakerStat)
    {
        bool isCritical;
        // 엄폐 성공 체크
        if (stat.IsCover && currentCoverObject != null)
        {
            if (Random.Range(0f, 1f) <= stat.CoverRate)
            {
                // 엄폐 성공
                Debug.Log("엄폐 성공");
                currentCoverObject.TakeDamage(attakerStat);
                return;
            }
            else
            {
                Debug.Log("엄폐 실패");
            }
        }

        // 적중 체크
        int calDodge = stat.Dodge - attakerStat.AccuracyRate;
        if (calDodge < 0)
        {
            calDodge = 0;
        }
        float dodgeCheck = 2000f / (calDodge * 3f + 2000f);

        if (Random.Range(0f, 1f) <= dodgeCheck)
        {
            // 적중

            // 치명타 체크
            int calCriticalRate = attakerStat.CriticalRate - 100;
            if (calCriticalRate < 0)
            {
                calCriticalRate = 0;
            }
            float criticalCheck = (calCriticalRate * 6000f) / (calCriticalRate * 6000f + 4000000f);

            if (Random.Range(0f, 1f) <= criticalCheck)
            {
                isCritical = true;
            }
            else
            {
                isCritical = false;
            }

            // 최종 대미지 계산
            float damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));
            float damage = attakerStat.Atk * damageRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
        }
        else
        {
            // 회피
        }
    }

    /*private void Reload()
    {
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        calCooltime += Time.deltaTime;

        if (calCooltime >= stat.ReloadTime)
        {
            stat.CurMag = stat.MaxMag;
            calCooltime = 0f;
            currentState = States.Attack;
        }
    }*/

    private void Reload()
    {
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }

        if (!IsInvoking("InvokeReload"))
        {
            Invoke("InvokeReload", stat.ReloadTime);
        }
    }

    private void InvokeReload()
    {
        stat.CurMag = stat.MaxMag;
        currentState = States.Attack;
    }

    private void TryCover()
    {
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        Vector3 directionToTarget = closestTarget.transform.position - transform.position;

        Collider[] cols = Physics.OverlapSphere(transform.position, 10, coverLayer);
        List<GameObject> covers = new List<GameObject>();

        foreach (Collider col in cols)
        {
            CoverObject cover = col.GetComponent<CoverObject>();

            if (cover != null && !cover.isOccupied)
            {
                // X축 범위에서 목표물과 캐릭터 사이에 있는 엄폐물 탐지
                if (cover.transform.position.x >= Mathf.Min(closestTarget.transform.position.x, transform.position.x)
                    && cover.transform.position.x <= Mathf.Max(closestTarget.transform.position.x, transform.position.x))
                {
                    covers.Add(col.gameObject);
                    Debug.DrawLine(transform.position, col.transform.position);
                }
            }
        }
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        if (covers.Count > 0)
        {
            foreach (GameObject tryCoverObject in covers)
            {
                CoverObject coverObject = tryCoverObject.GetComponent<CoverObject>();
                if (coverObject.CanCover(this.gameObject, stat, targetTag))
                {
                    nav.SetDestination(coverObject.coverSpot.transform.position);
                    currentCoverObject = coverObject;
                    currentCoverObject.GetUsedCharacter(this.gameObject);

                    break;
                }
            }
        }
        else
        {
            currentState = States.Attack;
        }
    }

    private void OnReachCover()
    {
        if (currentCoverObject != null)
        {
            coverPosition = transform.position;
            stat.IsCover = true;
            Debug.Log($"{gameObject.name}이 {currentCoverObject.gameObject.name}에 도착하여 엄폐를 사용 중입니다.");

            if(stat.CurMag <= 0)
            {
                currentState = States.Reload;
            }
            else
            {
                currentState = States.Attack;
            }
            
        }
    }

    public void LeaveCover()
    {
        stat.IsCover = false;
        Debug.Log($"{gameObject.name}이 엄폐 위치를 벗어났습니다.");
        currentCoverObject.StopCover();
        currentCoverObject = null;
    }

    private void UpdateTarget(float range)
    {
        targetDistance = Mathf.Infinity;
        closestTarget = null;
        GameObject closestEnemy = null;

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
                }
            }
        }
        if (closestEnemy != closestTarget)
        {
            closestTarget = closestEnemy;
            targetCharacter = closestEnemy != null ? closestTarget.GetComponent<Character>() : null;
        }
    }

    private void CleanupAndDestroy()
    {
        if (nav != null && nav.isOnNavMesh)
        {
            nav.isStopped = true;
            nav.ResetPath();
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (currentCoverObject != null)
        {
            currentCoverObject.isOccupied = false;
            currentCoverObject.StopCover();
        }
    }
}
