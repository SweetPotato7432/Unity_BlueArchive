using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // 공격 쿨타임
    private float calCooltime = 0f;

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
            Destroy(gameObject);
        }

        if (currentCoverObject != null && !currentCoverObject.CheckUser(gameObject))
        {
            currentCoverObject = null;
            nav.SetDestination(destination);
            return;
        }

        // 엄폐 위치를 벗어났는지 확인
        LeaveCover();
        

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
                    if(currentCoverObject == null)
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
<<<<<<< HEAD
        //if (stat.IsCover)
        //{
        //    stat.IsCover = false;
        //}
=======
        
>>>>>>> parent of 0940890 (�뾼�룓 湲곕뒫 援ы쁽 諛� �닔�젙)

        if (nav.isStopped)
        {
            nav.isStopped = false;
        }
        

        UpdateTarget(25);
        
        if (closestTarget != null)
        {
            if ( targetDistance <= range * 0.8f)
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
                calCooltime = 0f;
                currentState = States.Move;
                return;
            }
        }
        
        calCooltime += Time.deltaTime;
        if (calCooltime >= stat.AttackCoolTime && targetDistance<=range)
        {

            calCooltime -= stat.AttackCoolTime;
            // 탄약 제외
            stat.CurMag--;
            transform.LookAt(targetCharacter.gameObject.transform);
            targetCharacter.TakeDamage(stat);
            //Debug.Log(stat.Name + "이 " + stat.Atk + "의 데미지, 남은 장탄수 : " + stat.CurMag);
            
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
            // 데미지 비율 계산
            float damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));

            float damage = attakerStat.Atk*damageRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
            //Debug.Log(stat.Name+" "+stat.CurHp);
            //Debug.Log($"{attakerStat.Name}이 {stat.Name}에게 입힌 최종 대미지 {damage}");

        }
        else
        {
            // 회피
            //Debug.Log($"{stat.Name}가 {attakerStat.Name}의 공격에 회피");
        }
        
        

        
    }

    private void Reload()
    {
        // 정지
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        calCooltime += Time.deltaTime;
        Debug.Log($"{stat.Name} 재장전 중...");
        if( calCooltime >= stat.ReloadTime)
        {
            stat.CurMag = stat.MaxMag;
            calCooltime = 0f;
            currentState = States.Attack;
            Debug.Log($"{stat.Name} 재장전 완료!");
        }

    }

    private void TryCover()
    {
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // 10만큼의 거리로 장애물 탐지
        Collider[] cols = Physics.OverlapSphere(transform.position, 10, coverLayer);
        
        List<GameObject> covers = new List<GameObject>();

        foreach (Collider col in cols)
        {
            CoverObject cover = col.GetComponent<CoverObject>();

            if (cover != null && !cover.isOccupied)
            {

                // X축 기준으로 적과 아군의 사이에 엄폐물이 있는지 확인
                if(cover.transform.position.x>=Mathf.Min(closestTarget.transform.position.x,transform.position.x) 
                    && cover.transform.position.x <= Mathf.Max(closestTarget.transform.position.x, transform.position.x))
                {
                    covers.Add(col.gameObject);
                    Debug.DrawLine(transform.position,col.transform.position);
                }
            }
        }
        // 거리순으로 정렬
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        if(covers.Count > 0 )
        {
            //Debug.Log($"{this.gameObject.name}가장 가까운 엄폐물{covers[0].gameObject.name}");
            
            foreach(GameObject tryCoverObject in covers)
            {
                Debug.Log("탐색");
                CoverObject coverObject = tryCoverObject.GetComponent<CoverObject>();
                // 장애물에서 가장 가까운 적의 거리가 사거리보다 짧은지 확인
                if(coverObject.CanCover(this.gameObject,stat,targetTag))
                {
                    Debug.Log("이동!");
                    nav.SetDestination(coverObject.coverSpot.transform.position);
                    currentCoverObject = coverObject;
                    break;
                }
                else
                {
<<<<<<< HEAD
                    //Debug.Log("못찾음!");
=======
                    Debug.Log("못찾음!");
                   
>>>>>>> parent of 0940890 (�뾼�룓 湲곕뒫 援ы쁽 諛� �닔�젙)
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
<<<<<<< HEAD
            currentCoverObject.GetUsedCharacter(this.gameObject);
            coverPosition = transform.position;

            stat.IsCover = true;
=======
            currentCoverObject.isOccupied = true;
>>>>>>> parent of 0940890 (�뾼�룓 湲곕뒫 援ы쁽 諛� �닔�젙)
            Debug.Log($"{gameObject.name}이 {currentCoverObject.gameObject.name}에 도착하여 엄폐를 사용 중입니다.");
            currentState = States.Attack;
        }
         
    }

    private void LeaveCover()
    {
        if (stat.IsCover && coverPosition != transform.position)
        {
            if (!nav.hasPath || nav.velocity.sqrMagnitude != 0f)
            {
                stat.IsCover = false;
                Debug.Log($"{gameObject.name}이 엄폐 위치를 벗어났습니다.");
                currentCoverObject.StopCover();
                currentCoverObject = null;
            }
        }
    }

    private void UpdateTarget(float range)
    {
        targetDistance = Mathf.Infinity;
        closestTarget = null;
        GameObject closestEnemy = null;

        // 이동

        //25만큼의 거리로 적 탐지
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
<<<<<<< HEAD

    private void CleanupAndDestroy()
    {
        // NavMeshAgent 정리
        if (nav != null && nav.isOnNavMesh)
        {
            nav.isStopped = true;
            nav.ResetPath();
        }

        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        if(currentCoverObject != null)
        {
            currentCoverObject.isOccupied = false;
            currentCoverObject.StopCover();
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, 7);
    //}
=======
>>>>>>> parent of 0940890 (�뾼�룓 湲곕뒫 援ы쁽 諛� �닔�젙)
}
