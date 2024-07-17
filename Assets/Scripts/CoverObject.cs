using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐물과 관련된 스크립트

public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // 엄폐자 있는지 체크
    public bool isOccupied;

    // 캐릭터 탐지 레이어
    private LayerMask targetLayer;

    // 가까운 목표와의 거리
    private float targetDistance;

    // 엄폐 위치
    [SerializeField]
    private GameObject backSpot;
    [SerializeField]
    private GameObject frontSpot;

    public GameObject coverSpot;

    [SerializeField]
    private GameObject useCharacter;

    // Start is called before the first frame update
    void Start()
    {
        targetLayer = LayerMask.GetMask("Character");

        isOccupied = false;

        // 위치 저장
        backSpot = transform.GetChild(0).gameObject;
        frontSpot = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 엄폐 위치 지정
    private GameObject SelectSpot(Transform characterTransform)
    {
        GameObject spot;

        float backDistance = Vector3.Distance(characterTransform.position, backSpot.transform.position);
        float frontDistance = Vector3.Distance(characterTransform.position, frontSpot.transform.position);
        if (backDistance < frontDistance)
        {
            spot = backSpot;
        }
        else
        {
            spot = frontSpot;
        }
        return spot;
    }

    // 엄폐물에서 가장 가까운 적 확인
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        targetDistance = Mathf.Infinity;
        // 엄폐 위치
        coverSpot = SelectSpot(coverUser.transform);

        // 가까운 적 확인
        GameObject closestEnemy = null;

        Collider[] cols = Physics.OverlapSphere(coverSpot.transform.position, userStat.Range, targetLayer);
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
        if (closestEnemy == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }
<<<<<<< Updated upstream
=======

    public bool CheckUser(GameObject character)
    {
        if(isOccupied && useCharacter == character)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }
>>>>>>> Stashed changes
}