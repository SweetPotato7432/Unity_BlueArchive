using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class StrikerCharacter : MonoBehaviour
{
    [SerializeField]
    HpBar hpBar;

    // ���� ����
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;
  
    [SerializeField]
    private States currentState;   // ���� ����

    private NavMeshAgent nav; 

    // ���� �Ʊ� ������
    private Transform allyDestination;
    private Transform enemyDestination;

    [SerializeField]
    private Vector3 destination; // �̵� ��ǥ ����

    // ���� ����� ��ǥ
    [SerializeField]
    private GameObject closestTarget;
    // ���� ��ǥ���� �Ÿ�
    [SerializeField]
    private float targetDistance;
    // ���� ��ǥ�� Character ��ũ��Ʈ ������Ʈ
    private StrikerCharacter targetCharacter;

    private LayerMask targetLayer;
    private LayerMask coverLayer;

    // ���� �õ��ϴ� ��ǥ��
    [SerializeField]
    private CoverObject currentCoverObject;

    // ���� ��ġ
    [SerializeField]
    private Vector3 coverPosition; 

    // ��Ÿ�
    private float range;

    // Ÿ�� �±�
    private string targetTag;

    // ������ UI ������
    private GameObject damageUI;

    [SerializeField]
    bool isJumping = false;

    private void Awake()
    {
        // ĳ���� ���� ����
        
        stat = Stat.setUnitStat(unitCode);

        damageUI = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefaps/UI/Damage.prefab", typeof(GameObject));

    }

    void Start()
    { 

        nav = GetComponent<NavMeshAgent>();

        hpBar.SetHp(stat);


        // �ʱ� ���� ���� (Idle)
        currentState = States.Idle; 
        targetLayer = LayerMask.GetMask("Character");
        coverLayer = LayerMask.GetMask("Cover");
        range = stat.Range;

        allyDestination = GameObject.Find("AllyDestination").transform;
        enemyDestination = GameObject.Find("EnemyDestination").transform;

        // OffMeshLink�� �ڵ����� ������� �ʵ��� ����
        nav.autoTraverseOffMeshLink = false;

        // ĳ������ �±׿� ���� �������� ��ǥ ����
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
        //    Debug.Log($"{stat.Name} �κ�ũ ���� : {IsInvoking("InvokeReload")}");
        //}

        // ĳ������ ü���� 0�̵Ǹ� ���� ���·� ��ȯ �� ����
        if (stat.CurHp <= 0)
        {
            currentState = States.Dead;
            CleanupAndDestroy();
        }

        // ������ ���� ĳ���͸� ���� ������ �ʴٸ� �������� �̵�
        if (currentCoverObject != null && !currentCoverObject.CheckUser(gameObject))
        {
            currentCoverObject = null;
            nav.SetDestination(destination);
            return;
        }

        // ���� ��ġ�� ������� Ȯ��
        if (stat.IsCover && coverPosition != transform.position)
        {
            if (!nav.hasPath || nav.velocity.sqrMagnitude != 0f)
            {
                LeaveCover();
            }
        }

        // ���� ���¿� ���� �ൿ ����
        switch (currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Move:
                // �̵�
                Move();
                break;
            case States.Cover:
                // ���� �õ�
                Cover();
                break;
            case States.Attack:
                // ����
                Attack();
                break;
            case States.Reload:
                // ������
                Reload();
                break;
        }

        // ���� ����� ��ǥ���� �Ÿ� ���
        if (closestTarget != null)
        {
            targetDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
        }
    }

    // ���
    private void Idle()
    {
        // ĳ���͸� ������Ų��.
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
    }

    // �̵�
    private void Move()
    {
        // ĳ���� �̵�
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // ĳ���Ͱ� ������ �پ� �������� �Ѵٸ�
        if (nav.isOnOffMeshLink && !isJumping)
        {
            // ����
            StartCoroutine(TryJump());
        }
        // ��ǥ�� ����
        UpdateTarget(25);

        // ���� ����� ��ǥ���� �ִٸ�
        if (closestTarget != null)
        {
            // ��Ÿ��� 80�ۼ�Ʈ ���� ��ǥ���� �ִٸ� ���� ����
            if (targetDistance <= range * 0.8f)
            {
                // ���� �����ϴٸ� ���� ��ȯ
                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // ���� �Ұ��� ������ ź�� ���ٸ� ������ ��ȯ
                else if (stat.CurMag <= 0)
                {
                    currentState = States.Reload;
                }
                // ���� �Ұ����ϰ� ź�� �ִٸ� ����
                else
                {
                    currentState = States.Attack;
                }
            }
            // ��Ÿ� ���� ���� ���ٸ� ���� ����� ��ǥ������ �̵�
            else
            {
                nav.SetDestination(closestTarget.transform.position);
            }
        }
        // ���� ����� ��ǥ���� ���ٸ� �������� �̵�
        else
        {
            nav.SetDestination(destination);
        }
    }
    //���� �ѱ�
    private IEnumerator TryJump()
    {
        // ��ֹ� �پ�ѱ�

        isJumping = true;

        // ��ֹ��� ���� ��ġ�� ���� ��ġ�� ��� �����´�
        OffMeshLinkData linkData = nav.currentOffMeshLinkData;
        Vector3 startPos = nav.transform.position;
        Vector3 endPos = linkData.endPos+Vector3.up*nav.baseOffset;

        // ���� �Ÿ� �� �ð� ���
        float jumpDistance = Vector3.Distance(startPos, endPos);
        float jumpDuration = jumpDistance/nav.speed;
        
        // ���� ����ð�
        float time = 0f;
        // ���� �̵�
        while(time < jumpDuration)
        {
            nav.transform.position = Vector3.Lerp(startPos, endPos, time/jumpDuration);
            time+= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // ������ �Ϸ� �Ǹ� ���� ��ġ�� ĳ���͸� �ű�� ������ �����Ѵ�.
        nav.transform.position = endPos;
        nav.CompleteOffMeshLink();
        isJumping = false;
    }
    // ���� ����
    private void Attack()
    {
        // ĳ���͸� ������Ų��.
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }

        // ���� ����� ��ǥ���� ���ų� ��ǥ������ �Ÿ��� ��Ÿ����� ū ���
        if (closestTarget == null || targetDistance > range)
        {
            // ��Ÿ� ���� ��ǥ���� �ٽ� Ž���Ѵ�.
            UpdateTarget(range);
            // �׷��� ��ǥ���� ���ٸ�
            if (closestTarget == null)
            {
                CancelInvoke("InvokeAttack");
                
                
                currentState = States.Move;
                return;
            }
        }
        
        // ���� �������� �ƴ϶��
        else if (!IsInvoking("InvokeAttack"))
        {
            if (stat.CurMag <= 0)
            {
                currentState = States.Move;
            }
            // ������ �ݺ��Ѵ�.
            InvokeRepeating("InvokeAttack", stat.AttackCoolTime, stat.AttackCoolTime);
        }
    }
    // ���� ����
    private void InvokeAttack()
    {
        // ���� ����� ��ǥ���� ���ų� Ÿ�ٰ��� �Ÿ��� ��Ÿ����� �ְų� źâ�� ź���� ���� ���
        if (closestTarget == null || targetDistance > range || stat.CurMag <= 0)
        {
            // ������ �����ϰ� �̵� ���·� ��ȯ �Ѵ�.
            CancelInvoke("InvokeAttack");
            currentState = States.Move;
            return;
        }
        //Debug.Log($"{stat.Name} : Attack");
        // ź�� ����
        stat.CurMag--;
        // ���ݽ� ��ǥ���� �ٶ󺻴�.
        transform.LookAt(targetCharacter.gameObject.transform);
        // ������ ���ظ� ����
        targetCharacter.TakeDamage(stat);

    }
    // ���ظ� ����
    public void TakeDamage(Stat attakerStat)
    {
        // ġ��Ÿ üũ
        bool isCritical = false;
        // ������ �ð�ȭ
        GameObject DamagePrefab;

        // ���� ���� üũ
        // �������̰�, ���� ����� ������ �����Ҷ�
        if (stat.IsCover && currentCoverObject != null)
        {
            // CoverRate�� Ȯ����
            if (Random.Range(0f, 1f) <= stat.CoverRate)
            {
                // ���� �����ϰ� ������ ������ ��� �޴´�.
                // Debug.Log("���� ����");
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
                //���� ���н� ������ ��� ����
            }

        }

        // ���� üũ
        int calDodge = stat.Dodge - attakerStat.AccuracyRate;
        if (calDodge < 0)
        {
            calDodge = 0;
        }
        float dodgeCheck = 2000f / (calDodge * 3f + 2000f);

        if (Random.Range(0f, 1f) <= dodgeCheck)
        {
            // ����

            // ġ��Ÿ üũ
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

            // ����, ��� �� ���
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
            // ���� ����� ���
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
            // ȸ��
        }
    }
    // ����, ��� �� ���
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
    // ������
    private void Reload()
    {
        // ĳ���� ����
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        // �������ϰ� ���� �ʴٸ� ������ ����
        if (!IsInvoking("InvokeReload"))
        {
            Invoke("InvokeReload", stat.ReloadTime);
        }
    }
    // ������ ����
    private void InvokeReload()
    {
        // ������ �� ���� ���� ��ȯ
        stat.CurMag = stat.MaxMag;
        currentState = States.Attack;
    }
    // ����
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
    // ���� �õ�
    private void TryCover()
    {
        // ĳ���� �̵�
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // ���� ����� �� �������� ������ Ž��
        Vector3 directionToTarget = closestTarget.transform.position - transform.position;
        Collider[] cols = Physics.OverlapSphere(transform.position, 10, coverLayer);
        List<GameObject> covers = new List<GameObject>();

        foreach (Collider col in cols)
        {
            CoverObject cover = col.GetComponent<CoverObject>();

            if (cover != null && !cover.isOccupied)
            {
                // X�� �������� ��ǥ���� ĳ���� ���̿� �ִ� ���� Ž��
                if (cover.transform.position.x >= Mathf.Min(closestTarget.transform.position.x, transform.position.x)
                    && cover.transform.position.x <= Mathf.Max(closestTarget.transform.position.x, transform.position.x))
                {
                    covers.Add(col.gameObject);
                    Debug.DrawLine(transform.position, col.transform.position);
                }
            }
        }
        // ������ �Ÿ������� ����
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        // ������ �ִٸ�
        if (covers.Count > 0)
        {
            // ���� �������� üũ
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
        // ������ ���ٸ�
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
    // ���󹰿� ���޽�
    private void OnReachCover()
    {
        if (currentCoverObject != null)
        {
            coverPosition = transform.position;
            stat.IsCover = true;
            //Debug.Log($"{gameObject.name}�� {currentCoverObject.gameObject.name}�� �����Ͽ� ���� ��� ���Դϴ�.");

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
    // ���󹰿��� ��� ��
    public void LeaveCover()
    {
        stat.IsCover = false;
        //Debug.Log($"{gameObject.name}�� ���� ��ġ�� ������ϴ�.");
        currentCoverObject.StopCover();
        currentCoverObject = null;
    }
    // ���� ����� ��ǥ�� Ž��
    private void UpdateTarget(float range)
    {
        targetDistance = Mathf.Infinity;
        closestTarget = null;
        GameObject closestEnemy = null;

        //��Ÿ� ��ŭ�� �������� ��ǥ���� Ž��
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
    //����� �л� ���� ���
    public void SpecialCharacterStatCalculate(Stat stat)
    {
        //Debug.Log($"{this.stat.Name}�� ���� ����!");
        //Debug.Log($"{Mathf.FloorToInt(stat.MaxHp * 0.1f)}, {Mathf.FloorToInt(stat.MaxHp * 0.1f)},{Mathf.FloorToInt(stat.Atk * 0.1f)},{Mathf.FloorToInt(stat.Def * 0.05f)}");
        this.stat.MaxHp += Mathf.FloorToInt(stat.MaxHp*0.1f);
        this.stat.CurHp += Mathf.FloorToInt(stat.MaxHp*0.1f);
        this.stat.Atk += Mathf.FloorToInt(stat.Atk * 0.1f);
        this.stat.Def += Mathf.FloorToInt(stat.Def * 0.05f);
    }
    // �� ���� ����
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
    // �� ���� ����
    public void ChangeState(States state)
    {
        if(currentState != state)
        {
            currentState = state;
        }
    }
    // ĳ���� ����
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
    // ĳ���� �ı�
    private void OnDestroy()
    {
        if (currentCoverObject != null)
        {
            currentCoverObject.isOccupied = false;
            currentCoverObject.StopCover();
        }
    }
}
