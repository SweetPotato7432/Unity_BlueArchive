using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class StrikerCharacter : MonoBehaviour
{
    [SerializeField]
    HpBar hpBar;

    // 스탯 생성
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;
  
    [SerializeField]
    private States currentState;   // 현재 상태

    private NavMeshAgent nav; 

    // 적과 아군 종착지
    private Transform allyDestination;
    private Transform enemyDestination;

    [SerializeField]
    private Vector3 destination; // 이동 목표 지점

    // 가장 가까운 목표
    [SerializeField]
    private GameObject closestTarget;
    // 현재 목표와의 거리
    [SerializeField]
    private float targetDistance;
    // 현재 목표의 Character 스크립트 컴포넌트
    private StrikerCharacter targetCharacter;

    private LayerMask targetLayer;
    private LayerMask coverLayer;

    // 엄폐를 시도하는 목표물
    [SerializeField]
    private CoverObject currentCoverObject;

    // 엄폐 위치
    [SerializeField]
    private Vector3 coverPosition; 

    // 사거리
    private float range;

    // 타겟 태그
    private string targetTag;

    // 데미지 UI 프리팹
    private GameObject damageUI;

    [SerializeField]
    bool isJumping = false;

    private void Awake()
    {
        // 캐릭터 스탯 구현
        
        stat = Stat.setUnitStat(unitCode);

        damageUI = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefaps/UI/Damage.prefab", typeof(GameObject));

    }

    void Start()
    { 

        nav = GetComponent<NavMeshAgent>();

        hpBar.SetHp(stat);


        // 초기 상태 설정 (Idle)
        currentState = States.Idle; 
        targetLayer = LayerMask.GetMask("Character");
        coverLayer = LayerMask.GetMask("Cover");
        range = stat.Range;

        allyDestination = GameObject.Find("AllyDestination").transform;
        enemyDestination = GameObject.Find("EnemyDestination").transform;

        // OffMeshLink를 자동으로 통과하지 않도록 설정
        nav.autoTraverseOffMeshLink = false;

        // 캐릭터의 태그에 따라 목적지와 목표 설정
        if (this.tag == "Enemy")
        {
            hpBar.setEnemyColor(stat);
            currentState = States.Move;
            targetTag = "Ally";
            destination = new Vector3(enemyDestination.position.x, transform.position.y, transform.position.z);
        }
        else if (this.tag == "Ally")
        {
            hpBar.setAllyColor();
            targetTag = "Enemy";
            destination = new Vector3(allyDestination.position.x, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(this.tag == "Ally")
        //{
        //    Debug.Log($"{stat.Name} 인보크 여부 : {IsInvoking("InvokeReload")}");
        //}

        // 캐릭터의 체력이 0이되면 죽은 상태로 전환 및 삭제
        if (stat.CurHp <= 0)
        {
            currentState = States.Dead;
            CleanupAndDestroy();
        }

        // 엄폐물이 현재 캐릭터를 엄폐 중이지 않다면 목적지로 이동
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

        // 현재 상태에 따라 행동 결정
        switch (currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Move:
                // 이동
                Move();
                break;
            case States.Cover:
                // 엄폐 시도
                Cover();
                break;
            case States.Attack:
                // 공격
                Attack();
                break;
            case States.Reload:
                // 재장전
                Reload();
                break;
        }

        // 가장 가까운 목표와의 거리 계산
        if (closestTarget != null)
        {
            targetDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
        }
    }

    // 대기
    private void Idle()
    {
        // 캐릭터를 정지시킨다.
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
    }

    // 이동
    private void Move()
    {
        // 캐릭터 이동
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // 캐릭터가 엄폐물을 뛰어 넘으려고 한다면
        if (nav.isOnOffMeshLink && !isJumping)
        {
            // 점프
            StartCoroutine(TryJump());
        }
        // 목표물 갱신
        UpdateTarget(25);

        // 가장 가까운 목표물이 있다면
        if (closestTarget != null)
        {
            // 사거리의 80퍼센트 내에 목표물이 있다면 상태 변경
            if (targetDistance <= range * 0.8f)
            {
                // 엄폐 가능하다면 엄폐 전환
                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // 엄폐가 불가능 하지만 탄이 없다면 재장전 전환
                else if (stat.CurMag <= 0)
                {
                    currentState = States.Reload;
                }
                // 엄폐가 불가능하고 탄이 있다면 공격
                else
                {
                    currentState = States.Attack;
                }
            }
            // 사거리 내에 적이 없다면 가장 가까운 목표물에게 이동
            else
            {
                nav.SetDestination(closestTarget.transform.position);
            }
        }
        // 가장 가까운 목표물이 없다면 목적지로 이동
        else
        {
            nav.SetDestination(destination);
        }
    }
    //엄폐물 넘기
    private IEnumerator TryJump()
    {
        // 장애물 뛰어넘기

        isJumping = true;

        // 장애물의 점프 위치와 착지 위치를 모두 가져온다
        OffMeshLinkData linkData = nav.currentOffMeshLinkData;
        Vector3 startPos = nav.transform.position;
        Vector3 endPos = linkData.endPos+Vector3.up*nav.baseOffset;

        // 점프 거리 및 시간 계산
        float jumpDistance = Vector3.Distance(startPos, endPos);
        float jumpDuration = jumpDistance/nav.speed;
        
        // 점프 진행시간
        float time = 0f;
        // 점프 이동
        while(time < jumpDuration)
        {
            nav.transform.position = Vector3.Lerp(startPos, endPos, time/jumpDuration);
            time+= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // 점프가 완료 되면 종료 위치로 캐릭터를 옮기고 점프를 종료한다.
        nav.transform.position = endPos;
        nav.CompleteOffMeshLink();
        isJumping = false;
    }
    // 공격 돌입
    private void Attack()
    {
        // 캐릭터를 정지시킨다.
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }

        // 가장 가까운 목표물이 없거나 목표물과의 거리가 사거리보다 큰 경우
        if (closestTarget == null || targetDistance > range)
        {
            // 사거리 내에 목표물을 다시 탐색한다.
            UpdateTarget(range);
            // 그래도 목표물이 없다면
            if (closestTarget == null)
            {
                CancelInvoke("InvokeAttack");
                
                
                currentState = States.Move;
                return;
            }
        }
        
        // 만약 공격중이 아니라면
        else if (!IsInvoking("InvokeAttack"))
        {
            if (stat.CurMag <= 0)
            {
                currentState = States.Move;
            }
            // 공격을 반복한다.
            InvokeRepeating("InvokeAttack", stat.AttackCoolTime, stat.AttackCoolTime);
        }
    }
    // 공격 실행
    private void InvokeAttack()
    {
        // 가장 가까운 목표물이 없거나 타겟과의 거리가 사거리보다 멀거나 탄창에 탄약이 없는 경우
        if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
        {
            // 공격을 종료하고 이동 상태로 전환 한다.
            CancelInvoke("InvokeAttack");
            currentState = States.Move;
            return;
        }
        //Debug.Log($"{stat.Name} : Attack");
        // 탄약 제외
        stat.CurMag--;
        // 공격시 목표물을 바라본다.
        transform.LookAt(targetCharacter.gameObject.transform);
        // 적에게 피해를 입힘
        targetCharacter.TakeDamage(stat);

    }
    // 피해를 입음
    public void TakeDamage(Stat attakerStat)
    {
        // 치명타 체크
        bool isCritical = false;
        // 데미지 시각화
        GameObject DamagePrefab;

        // 엄폐 성공 체크
        // 엄폐중이고, 가장 가까운 엄폐물이 존재할때
        if (stat.IsCover && currentCoverObject != null)
        {
            // CoverRate의 확률로
            if (Random.Range(0f, 1f) <= stat.CoverRate)
            {
                // 엄폐 성공하고 엄폐물이 공격을 대신 받는다.
                // Debug.Log("엄폐 성공");
                currentCoverObject.TakeDamage(attakerStat);
                DamagePrefab = Instantiate(damageUI);
                DamagePrefab.transform.position = currentCoverObject.transform.position;
                //DamagePrefab.transform.position = Camera.main.WorldToScreenPoint(currentCoverObject.transform.position);

                DamageUI DamageScr = DamagePrefab.GetComponent<DamageUI>();
                DamageScr.SetData("Block", isCritical, DamageCode.None);
                return;
            }
            else
            {
                //엄폐 실패시 데미지 계산 시작
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

            // 공격, 방어 상성 계산
            DamageCode damageEffective = TypeCompatibilityCal(attakerStat, this.stat);
            float effectiveRatio = 1f;

            switch (damageEffective)
            {
                case DamageCode.Normal:
                    effectiveRatio = 1f; 
                    break;
                case DamageCode.Resist:
                    effectiveRatio = 0.5f;
                    break;
                case DamageCode.Effective:
                    effectiveRatio = 1.5f;
                    break;
                case DamageCode.Weak:
                    effectiveRatio = 2f;
                    break;
                case DamageCode.None:
                    effectiveRatio = 1f;
                    break;
            }
            // 최종 대미지 계산
            float damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));
            float damage = attakerStat.Atk * damageRatio * effectiveRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
            DamagePrefab = Instantiate(damageUI);
            DamagePrefab.transform.position = transform.position;
            //DamagePrefab.transform.position = Camera.main.WorldToScreenPoint(transform.position);

            DamageUI DamageScr = DamagePrefab.GetComponent<DamageUI>();
            DamageScr.SetData(((int)damage).ToString(), isCritical, damageEffective);

            hpBar.SetHp(stat);
        }
        else
        {
            DamagePrefab = Instantiate(damageUI);
            DamagePrefab.transform.position = transform.position;
            //DamagePrefab.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            DamageUI DamageScr = DamagePrefab.GetComponent<DamageUI>();
            DamageScr.SetData("Miss", isCritical, DamageCode.None);
            // 회피
        }
    }
    // 공격, 방어 상성 계산
    private DamageCode TypeCompatibilityCal(Stat attakerStat, Stat defenderStat)
    {
        
        switch (attakerStat.DamageTypeCode)
        {
            case AttackTypeCode.Normal:
                return DamageCode.Normal;
            case AttackTypeCode.Explosive:
                switch (defenderStat.DefendTypeCode)
                {
                    case DefendTypeCode.Normal:
                        return DamageCode.Normal;
                    case DefendTypeCode.Light:
                        return DamageCode.Weak;
                    case DefendTypeCode.Heavy:
                        return DamageCode.Normal;
                    case DefendTypeCode.Special:
                        return DamageCode.Resist;
                    case DefendTypeCode.Elastic:
                        return DamageCode.Resist;
                    default:
                        return DamageCode.None;
                }
            case AttackTypeCode.Piercing:
                switch (defenderStat.DefendTypeCode)
                {
                    case DefendTypeCode.Normal:
                        return DamageCode.Normal;
                    case DefendTypeCode.Light:
                        return DamageCode.Resist;
                    case DefendTypeCode.Heavy:
                        return DamageCode.Weak;
                    case DefendTypeCode.Special:
                        return DamageCode.Normal;
                    case DefendTypeCode.Elastic:
                        return DamageCode.Normal;
                    default:
                        return DamageCode.None;
                }
                
            case AttackTypeCode.Mystic:
                switch (defenderStat.DefendTypeCode)
                {
                    case DefendTypeCode.Normal:
                        return DamageCode.Normal;
                    case DefendTypeCode.Light:
                        return DamageCode.Normal;
                    case DefendTypeCode.Heavy:
                        return DamageCode.Resist;
                    case DefendTypeCode.Special:
                        return DamageCode.Weak;
                    case DefendTypeCode.Elastic:
                        return DamageCode.Normal;
                    default:
                        return DamageCode.None;
                }
                
            case AttackTypeCode.Sonic:
                switch (defenderStat.DefendTypeCode)
                {
                    case DefendTypeCode.Normal:
                        return DamageCode.Normal;
                    case DefendTypeCode.Light:
                        return DamageCode.Normal;
                    case DefendTypeCode.Heavy:
                        return DamageCode.Resist;
                    case DefendTypeCode.Special:
                        return DamageCode.Effective;
                    case DefendTypeCode.Elastic:
                        return DamageCode.Weak;
                    default:
                        return DamageCode.None;
                }
            default: 
                return DamageCode.None;
        }
    }
    // 재장전
    private void Reload()
    {
        // 캐릭터 정지
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        // 재장전하고 있지 않다면 재장전 시작
        if (!IsInvoking("InvokeReload"))
        {
            Invoke("InvokeReload", stat.ReloadTime);
        }
    }
    // 재장전 실행
    private void InvokeReload()
    {
        // 재장전 후 공격 상태 전환
        stat.CurMag = stat.MaxMag;
        currentState = States.Attack;
    }
    // 엄폐
    private void Cover()
    {
        if(closestTarget == null)
        {
            currentState = States.Move;
        }
        else if (currentCoverObject == null)
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
    }
    // 엄폐 시도
    private void TryCover()
    {
        // 캐릭터 이동
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // 가장 가까운 적 방향으로 엄폐물을 탐지
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
        // 엄폐물을 거리순으로 정렬
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        // 엄폐물이 있다면
        if (covers.Count > 0)
        {
            // 엄폐 가능한지 체크
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
        // 엄폐물이 없다면
        else
        {
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
    // 엄폐물에 도달시
    private void OnReachCover()
    {
        if (currentCoverObject != null)
        {
            coverPosition = transform.position;
            stat.IsCover = true;
            //Debug.Log($"{gameObject.name}이 {currentCoverObject.gameObject.name}에 도착하여 엄폐를 사용 중입니다.");

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
    // 엄폐물에서 벗어날 시
    public void LeaveCover()
    {
        stat.IsCover = false;
        //Debug.Log($"{gameObject.name}이 엄폐 위치를 벗어났습니다.");
        currentCoverObject.StopCover();
        currentCoverObject = null;
    }
    // 가장 가까운 목표물 탐색
    private void UpdateTarget(float range)
    {
        targetDistance = Mathf.Infinity;
        closestTarget = null;
        GameObject closestEnemy = null;

        //사거리 만큼의 범위에서 목표물을 탐색
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
            targetCharacter = closestEnemy != null ? closestTarget.GetComponent<StrikerCharacter>() : null;
        }
    }
    //스페셜 학생 스탯 계산
    public void SpecialCharacterStatCalculate(Stat stat)
    {
        //Debug.Log($"{this.stat.Name}의 스탯 증가!");
        //Debug.Log($"{Mathf.FloorToInt(stat.MaxHp * 0.1f)}, {Mathf.FloorToInt(stat.MaxHp * 0.1f)},{Mathf.FloorToInt(stat.Atk * 0.1f)},{Mathf.FloorToInt(stat.Def * 0.05f)}");
        this.stat.MaxHp += Mathf.FloorToInt(stat.MaxHp*0.1f);
        this.stat.CurHp += Mathf.FloorToInt(stat.MaxHp*0.1f);
        this.stat.Atk += Mathf.FloorToInt(stat.Atk * 0.1f);
        this.stat.Def += Mathf.FloorToInt(stat.Def * 0.05f);
    }
    // 현 상태 전송
    public bool IsIdle()
    {
        if(currentState == States.Idle)
        {
            return true;
        }
        else
        {
            //currentState = States.Idle;
            return false;
        }
        
    }
    // 현 상태 변경
    public void ChangeState(States state)
    {
        if(currentState != state)
        {
            currentState = state;
        }
    }
    // 캐릭터 삭제
    private void CleanupAndDestroy()
    {
        if (nav != null && nav.isOnNavMesh)
        {
            nav.isStopped = true;
            nav.ResetPath();
        }

        GameManager.Instance.CharacterDead(this.tag);

        Destroy(gameObject);
    }
    // 캐릭터 파괴
    private void OnDestroy()
    {
        if (currentCoverObject != null)
        {
            currentCoverObject.isOccupied = false;
            currentCoverObject.StopCover();
        }
    }
}
