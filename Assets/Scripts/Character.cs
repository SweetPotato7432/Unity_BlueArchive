using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
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
        Reload,
        Dead
    };

  
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
    private Character targetCharacter;

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

    [SerializeField]
    bool isJumping = false;

    private void Awake()
    {
        // ĳ���� ���� ����
        
        stat = Stat.setUnitStat(unitCode);
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        // �ʱ� ���� ���� (Move)
        currentState = States.Move; 
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
            targetTag = "Ally";
            destination = new Vector3(enemyDestination.position.x, transform.position.y, transform.position.z);
        }
        else if (this.tag == "Ally")
        {
            targetTag = "Enemy";
            destination = new Vector3(allyDestination.position.x, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                // ������ �����ϰ� �̵� ���·� ��ȯ�Ѵ�.
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
        bool isCritical;
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

            // ���� ����� ���
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
            // ȸ��
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
            targetCharacter = closestEnemy != null ? closestTarget.GetComponent<Character>() : null;
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
