using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{// Start is called before the first frame update

    // ½ºÅÈ »ı¼º
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;
    
    // ÇöÀç »óÅÂ
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

    // °¡Àå °¡±î¿î ¸ñÇ¥
    [SerializeField]
    private GameObject closestTarget;
    // ÇöÀç ¸ñÇ¥¿ÍÀÇ °Å¸®
    [SerializeField]
    private float targetDistance;
    // ÇöÀç ¸ñÇ¥ÀÇ Character ½ºÅ©¸³Æ® ÄÄÆ÷³ÍÆ®
    private Character targetCharacter;

    private LayerMask targetLayer;

    private LayerMask coverLayer;

    // ¾öÆó¸¦ ½ÃµµÇÏ´Â ¸ñÇ¥¹°
    [SerializeField]
    private CoverObject currentCoverObject;

    [SerializeField]
    private Vector3 coverPosition;

    // »ç°Å¸®
    private float range;

    // °ø°İ ÄğÅ¸ÀÓ
    private float calCooltime = 0f;

    // Å¸°Ù ÅÂ±×
    private string targetTag;

    [SerializeField]
    bool test;

    private void Awake()
    {
        // Ä³¸¯ÅÍ ½ºÅÈ ±¸Çö
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
            // ¸ñÇ¥ ¼³Á¤
            targetTag = "Ally";

            // ¸ñÀûÁö ¼³Á¤
            destination = new Vector3(enemyDestination.position.x, transform.position.y, transform.position.z);
        }
        else if (this.tag == "Ally")
        {
            // ¸ñÇ¥ ¼³Á¤
            targetTag = "Enemy";

            // ¸ñÀûÁö ¼³Á¤
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

        // ¾öÆó À§Ä¡¸¦ ¹ş¾î³µ´ÂÁö È®ÀÎ
        LeaveCover();
        

        switch (currentState)
        {
            case States.Move:
                {
                    // ÀÌµ¿ ¹× Àû Å½»ö
                    Move();
                    break;
                }
            case States.Cover:
                {
                    // ¾öÆó½Ãµµ
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
                    // °ø°İ
                    Attack();
                    break;
                }
            case States.Reload:
                {
                    
                    // ÀçÀåÀü
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
        
>>>>>>> parent of 0940890 (ì—„í ê¸°ëŠ¥ êµ¬í˜„ ë° ìˆ˜ì •)

        if (nav.isStopped)
        {
            nav.isStopped = false;
        }
        

        UpdateTarget(25);
        
        if (closestTarget != null)
        {
            if ( targetDistance <= range * 0.8f)
            {
                
                // ¾öÆó °¡´É È®ÀÎ

                if (stat.IsCoverAvailable)
                {
                    currentState = States.Cover;
                }
                // ¾öÆó È®ÀÎ ÈÄ °ø°İ
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
            // Åº¾à Á¦¿Ü
            stat.CurMag--;
            transform.LookAt(targetCharacter.gameObject.transform);
            targetCharacter.TakeDamage(stat);
            //Debug.Log(stat.Name + "ÀÌ " + stat.Atk + "ÀÇ µ¥¹ÌÁö, ³²Àº ÀåÅº¼ö : " + stat.CurMag);
            
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
        // ÀûÁß Ã¼Å©
        int calDodge = stat.Dodge - attakerStat.AccuracyRate;
        if (calDodge < 0)
        {
            calDodge = 0;
        }
        float dodgeCheck = 2000f / (calDodge * 3f + 2000f);

        
        if ( Random.Range(0f,1f) <= dodgeCheck)
        {
            //ÀûÁß

            // Ä¡¸íÅ¸ Ã¼Å©
            int calCriticalRate = attakerStat.CriticalRate - 100;
            if(calCriticalRate < 0)
            {
                calCriticalRate = 0;
            }
            float criticalCheck = (calCriticalRate * 6000f) / (calCriticalRate * 6000f + 4000000f);

            if (Random.Range(0f, 1f) <= criticalCheck)
            {
                //Debug.Log($"{stat.Name}°¡ {attakerStat.Name}ÀÇ Ä¡¸íÅ¸ °ø°İÀ» ÀûÁßÇÔ");
                isCritical = true;
            }
            else
            {
                //Debug.Log($"{stat.Name}°¡ {attakerStat.Name}ÀÇ ÀÏ¹İ °ø°İÀ» ÀûÁßÇÔ");
                isCritical = false;
            }

            // ÃÖÁ¾ ´ë¹ÌÁö °è»ê
            // µ¥¹ÌÁö ºñÀ² °è»ê
            float damageRatio = 1f / (1f + stat.Def / (1000f / 0.6f));

            float damage = attakerStat.Atk*damageRatio;
            if (isCritical)
            {
                damage *= attakerStat.CriticalDamage;
            }
            stat.CurHp -= damage;
            //Debug.Log(stat.Name+" "+stat.CurHp);
            //Debug.Log($"{attakerStat.Name}ÀÌ {stat.Name}¿¡°Ô ÀÔÈù ÃÖÁ¾ ´ë¹ÌÁö {damage}");

        }
        else
        {
            // È¸ÇÇ
            //Debug.Log($"{stat.Name}°¡ {attakerStat.Name}ÀÇ °ø°İ¿¡ È¸ÇÇ");
        }
        
        

        
    }

    private void Reload()
    {
        // Á¤Áö
        if (!nav.isStopped)
        {
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
        }
        calCooltime += Time.deltaTime;
        Debug.Log($"{stat.Name} ÀçÀåÀü Áß...");
        if( calCooltime >= stat.ReloadTime)
        {
            stat.CurMag = stat.MaxMag;
            calCooltime = 0f;
            currentState = States.Attack;
            Debug.Log($"{stat.Name} ÀçÀåÀü ¿Ï·á!");
        }

    }

    private void TryCover()
    {
        if (nav.isStopped)
        {
            nav.isStopped = false;
        }

        // 10¸¸Å­ÀÇ °Å¸®·Î Àå¾Ö¹° Å½Áö
        Collider[] cols = Physics.OverlapSphere(transform.position, 10, coverLayer);
        
        List<GameObject> covers = new List<GameObject>();

        foreach (Collider col in cols)
        {
            CoverObject cover = col.GetComponent<CoverObject>();

            if (cover != null && !cover.isOccupied)
            {

                // XÃà ±âÁØÀ¸·Î Àû°ú ¾Æ±ºÀÇ »çÀÌ¿¡ ¾öÆó¹°ÀÌ ÀÖ´ÂÁö È®ÀÎ
                if(cover.transform.position.x>=Mathf.Min(closestTarget.transform.position.x,transform.position.x) 
                    && cover.transform.position.x <= Mathf.Max(closestTarget.transform.position.x, transform.position.x))
                {
                    covers.Add(col.gameObject);
                    Debug.DrawLine(transform.position,col.transform.position);
                }
            }
        }
        // °Å¸®¼øÀ¸·Î Á¤·Ä
        covers.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        if(covers.Count > 0 )
        {
            //Debug.Log($"{this.gameObject.name}°¡Àå °¡±î¿î ¾öÆó¹°{covers[0].gameObject.name}");
            
            foreach(GameObject tryCoverObject in covers)
            {
                Debug.Log("Å½»ö");
                CoverObject coverObject = tryCoverObject.GetComponent<CoverObject>();
                // Àå¾Ö¹°¿¡¼­ °¡Àå °¡±î¿î ÀûÀÇ °Å¸®°¡ »ç°Å¸®º¸´Ù ÂªÀºÁö È®ÀÎ
                if(coverObject.CanCover(this.gameObject,stat,targetTag))
                {
                    Debug.Log("ÀÌµ¿!");
                    nav.SetDestination(coverObject.coverSpot.transform.position);
                    currentCoverObject = coverObject;
                    break;
                }
                else
                {
<<<<<<< HEAD
                    //Debug.Log("¸øÃ£À½!");
=======
                    Debug.Log("¸øÃ£À½!");
                   
>>>>>>> parent of 0940890 (ì—„í ê¸°ëŠ¥ êµ¬í˜„ ë° ìˆ˜ì •)
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
>>>>>>> parent of 0940890 (ì—„í ê¸°ëŠ¥ êµ¬í˜„ ë° ìˆ˜ì •)
            Debug.Log($"{gameObject.name}ÀÌ {currentCoverObject.gameObject.name}¿¡ µµÂøÇÏ¿© ¾öÆó¸¦ »ç¿ë ÁßÀÔ´Ï´Ù.");
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
                Debug.Log($"{gameObject.name}ÀÌ ¾öÆó À§Ä¡¸¦ ¹ş¾î³µ½À´Ï´Ù.");
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

        // ÀÌµ¿

        //25¸¸Å­ÀÇ °Å¸®·Î Àû Å½Áö
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
        // NavMeshAgent Á¤¸®
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
>>>>>>> parent of 0940890 (ì—„í ê¸°ëŠ¥ êµ¬í˜„ ë° ìˆ˜ì •)
}
