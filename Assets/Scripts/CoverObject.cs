using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐물에 부착된 스크립트
public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // 엄폐물이 사용 중인지 확인
    public bool isOccupied;

    // 캐릭터 탐지 레이어
    private LayerMask targetLayer;

    // 적과의 거리
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

        // 위치 설정
        backSpot = transform.GetChild(0).gameObject;
        frontSpot = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 엄폐 위치 선택
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

    // 엄폐물에 엄폐가 가능한지 확인
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        targetDistance = Mathf.Infinity;
        // 엄폐 위치 선택
        coverSpot = SelectSpot(coverUser.transform);

        // 엄폐 가능 여부 확인
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
                }
            }
        }
        return closestEnemy != null;
    }

    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }

    public bool CheckUser(GameObject character)
    {
        return isOccupied && useCharacter == character;
    }

    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }
}
