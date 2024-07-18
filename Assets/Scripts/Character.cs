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
    private States currentState;

    private NavMeshAgent nav;

    private Transform allyDestination;
    private Transform enemyDestination;

    [SerializeField]
    private Vector3 destination;

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

    [SerializeField]
    private Vector3 coverPosition;

    // ��Ÿ�
    private float range;

    // Ÿ�� �±�
    private string targetTag;

    [SerializeField]
    bool test;

    private void Awake()
    {
        // ĳ���� ���� ����
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
            // ��ǥ ����
            targetTag = "Ally";

            // ������ ����
            destination = new Vector3(enemyDestination.position.x, transform.position.y, transform.position.z);
        }
        else if (this.tag == "Ally")
        {
            // ��ǥ ����
            targetTag = "Enemy";

            // ������ ����
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

        // ���� ��ġ�� ������� Ȯ��
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
                // ���� �õ�
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
                // ���� ���� Ȯ��
                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // ���� Ȯ�� �� ����
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
            // ź�� ����
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
        // ź�� ����
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
        // ���� ���� üũ
        if (stat.IsCover && currentCoverObject != null)
        {
            if (Random.Range(0f, 1f) <= stat.CoverRate)
            {
                // ���� ����
                Debug.Log("���� ����");
                currentCoverObject.TakeDamage(attakerStat);
                return;
            }
            else
            {
                Debug.Log("���� ����");
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
                // X�� �������� ��ǥ���� ĳ���� ���̿� �ִ� ���� Ž��
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
            Debug.Log($"{gameObject.name}�� {currentCoverObject.gameObject.name}�� �����Ͽ� ���� ��� ���Դϴ�.");

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
        Debug.Log($"{gameObject.name}�� ���� ��ġ�� ������ϴ�.");
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
